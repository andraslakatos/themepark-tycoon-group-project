using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MapModel;
using MapModel.Building;
using MapModel.Facility;
using MapModel.Visitors;
using Model.BuildingConstructors;
using Persistence;
using Activity = MapModel.Visitors.Activity;
using System.Xml.Serialization;

namespace Model
{

    /// <summary>
    /// GameModel típusa
    /// </summary>
    public partial class GameModel
    {
        #region Constants

        private readonly int[] _rowInts = { 0, -1, 0, 1 };
        private readonly int[] _colInts = { -1, 0, 1, 0 };

        #endregion

        #region Fields

        private SpeedEnum _gameSpeed;

        #endregion

        #region Properties
        public BuildingData BuildingData { get; set; }
        public int BuildingIds { get; set; }
        public GameTable GameTable { get; set; }
        public DataAccess DataAccess { get; set; }
        public SpeedEnum GameSpeed
        {
            get { return _gameSpeed;}
            set
            {
                if (_gameSpeed == value) return;
                _gameSpeed = value;
            }
        }
        public TimeSpan Time { get; set; }
        public PathFinding PathFinding { get; set; }

        public bool IsGameRunning { get; set; } //jelzés a view-nak hogy váltson a table megjelenítésre
        public int MaxVisitors { get; set; }

        public Random Rand { get; set; }

        #endregion

        #region Contructor

        /// <summary>
        /// GameModel példányosítása
        /// </summary>
        public GameModel()
        {
            DataAccess = new DataAccess();
            GameSpeed = SpeedEnum.Pause;
            BuildingData = new BuildingData();
            BuildingInitializer.Initialize(BuildingData);
            Rand = new Random();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Új játék példányosítása
        /// </summary>
        /// <param name="parkName">Park nevének azonosítója.</param>
        /// <param name="size">Város azonosítója.</param>
        public void NewGame(string parkName, int size)
        {
            GameTable = new GameTable(parkName, size);
            PathFinding._table = GameTable;
            PathFinding = new PathFinding();
            IsGameRunning = true;
            GameSpeed = SpeedEnum.Pause;
            BuildingIds = 0;
            MaxVisitors = 10;
            if (!Build(300, new Point(size/2-1, size-2)))
            {
                throw new ArgumentException();
            }
            OnMoneyChangedFunc();
            OnTimeChangedFunc();
            OnGuestNumChangedFunc();
        }

        /// <summary>
        /// Új játék példányosítása
        /// </summary>
        /// <param name="gameTable">Játék tábla azonosítója.</param>
        public void NewGame(GameTable gameTable)
        {
            GameTable = gameTable;
            PathFinding._table = gameTable;
            PathFinding = new PathFinding();
            IsGameRunning = true;
            BuildingIds = gameTable.Buildings.Count + 1;
            MaxVisitors = 10;
            foreach (var building in GameTable.Buildings)
            {
                var tmpData = BuildingData.GetBuildingType(building.BuildingTypeId);
                if (tmpData is FacilityData)
                {
                    var casted = (FacilityData) tmpData;
                    MaxVisitors += casted.Capacity;
                }
            }

            OnMoneyChangedFunc();
            OnTimeChangedFunc();
            OnGuestNumChangedFunc();
        }

        /// <summary>
        /// Épület lebontása
        /// </summary>
        /// <param name="x">Koorináta x tagja</param>
        /// <param name="y">Koorináta y tagja</param>
        public bool Destroy(int x, int y)
        {
            if (GameTable.IsTileEmpty(x, y)) 
                return false;
            var building = GameTable.GetTileValue(x, y).Building;
            if(building==null || building.BuildingTypeId == 300)
                return false;
            var cords = building.Coords;

            BuildingBaseData buildingType = BuildingData.GetBuildingType(building.BuildingTypeId);

            for (int i = cords.X; i < cords.X + buildingType.Size.X; i++)
            for (int j = cords.Y; j < cords.Y + buildingType.Size.Y; j++)
            {
                GameTable.Tiles[i, j].BuildingType = TileType.Empty;
                GameTable.Tiles[i, j].Building = null;
            }

            if (building.BuildingTypeId == 0 || building.BuildingTypeId == 1)
                DeletedRoad(building);

            if (building.BuildingTypeId <= 303 && building.BuildingTypeId > 300)
                PowerNearby(building, p => --p);

            if (building.BuildingTypeId >= 10 && building.BuildingTypeId <= 199 || building.BuildingTypeId == 4)
                DeletedFacility((Facilities)building);
            
            if(GameTable.Buildings.Contains(building))
                GameTable.Buildings.Remove(building);
            if(GameTable.UnderConstructionList.Contains(building))
                GameTable.UnderConstructionList.Remove(building);
            return true;
        }

        /// <summary>
        /// Épület építése két tagú koordinátával
        /// </summary>
        /// <param name="buildingTypeId">épület típusának azonosítója</param>
        /// <param name="x">Koorináta x tagja</param>
        /// <param name="y">Koorináta y tagja</param>
        /// <returns>Igaz ha megépült hamis ha nem.</returns>
        public bool Build(int buildingTypeId, int x, int y)
        {
            return Build(buildingTypeId, new Point(x, y));
        }

        /// <summary>
        /// Épület építése pont koordinátával
        /// </summary>
        /// <param name="buildingTypeId">épület típusának azonosítója</param>
        /// <param name="cords">Koorináta</param>
        /// <returns>Igaz ha megépült hamis ha nem.</returns>
        public bool Build(int buildingTypeId, Point cords)
        {
            BuildingBaseData building = BuildingData.GetBuildingType(buildingTypeId);

            if (cords.X < 0 || cords.X > GameTable.TableSize) throw new ArgumentOutOfRangeException();
            if (cords.Y < 0 || cords.Y > GameTable.TableSize) throw new ArgumentOutOfRangeException();

            if (!GameTable.IsTileEmpty(cords))
                return false;

            if (building.Price > GameTable.Balance)
                return false;

            for (int i = cords.X; i < cords.X + building.Size.X; i++)
            for (int j = cords.Y; j < cords.Y + building.Size.Y; j++)
            {
                if (i >= GameTable.TableSize || j >= GameTable.TableSize) return false;
                if (!GameTable[i,j].IsEmpty()) return false;
            }

            // Lerakás
            
            Buildings newBuilding = null;
            TileType type = TileType.Empty;
            if (buildingTypeId == 300)
            {
                newBuilding = new Entrance(BuildingIds++,buildingTypeId,cords,3);
                type = TileType.Entrance;
                PowerNearby(newBuilding, x => ++x);
                GameTable.Buildings.Add(newBuilding);
            }
            else if (buildingTypeId == 0)
            {
                if (GameTable[cords.X, cords.Y].Base != TileType.Grass) return false;
                newBuilding = new Road(BuildingIds++, buildingTypeId, cords);
                type = TileType.Road;
                GameTable.Buildings.Add(newBuilding);
            }
            else if (buildingTypeId == 1)
            {
                if (GameTable[cords.X, cords.Y].Base != TileType.Water) return false;
                newBuilding = new Pier(BuildingIds++, buildingTypeId, cords);
                type = TileType.Pier;
                GameTable.Buildings.Add(newBuilding);
            } 
            else if (buildingTypeId == 2)
            {
                if (GameTable[cords.X, cords.Y].Base != TileType.Grass) return false;
                GameTable.Tiles[cords.X,cords.Y].Base = TileType.Water;
                GameTable.Balance -= building.Price;
                OnMoneyChangedFunc();
                return true; //speciális eset, itt kilép mert ez nem épület
            }
            else if (buildingTypeId == 3)
            {
                if (GameTable[cords.X, cords.Y].Base != TileType.Water) return false;
                GameTable.Tiles[cords.X, cords.Y].Base = TileType.Grass;
                GameTable.Balance -= building.Price;
                OnMoneyChangedFunc();
                return true; //speciális eset, itt kilép mert ez nem épület
            }
            else if (buildingTypeId == 4)
            {
                if (GameTable[cords.X, cords.Y].Base != TileType.Grass) return false;
                //newBuilding = new Restroom(BuildingIds++, buildingTypeId, cords);
                newBuilding = new Restroom(0, 0, FacilityStatus.Building,new Queue<Visitor>(), new Queue<Visitor>(), new List<Visitor>(), BuildingIds++, buildingTypeId, cords);
                type = TileType.RestRoom;
                GameTable.UnderConstructionList.Add(newBuilding);
            }
            else if (buildingTypeId >= 10 && buildingTypeId < 100)
            {
                if (BuildingData.RideData[buildingTypeId].Water)
                {
                    for (int i = cords.X; i < cords.X + building.Size.X; i++)
                    for (int j = cords.Y; j < cords.Y + building.Size.Y; j++)
                        if (GameTable[i, j].Base != TileType.Water) return false;
                }
                else
                {
                    for (int i = cords.X; i < cords.X + building.Size.X; i++)
                    for (int j = cords.Y; j < cords.Y + building.Size.Y; j++)
                        if (GameTable[i, j].Base != TileType.Grass) return false;
                }
                newBuilding = new Ride(BuildingData.RideData[buildingTypeId].Fee,BuildingData.RideData[buildingTypeId].MinUsage,BuildingIds++,buildingTypeId,cords);
                type = TileType.Ride;
                MaxVisitors += BuildingData.RideData[buildingTypeId].Capacity;
                GameTable.UnderConstructionList.Add(newBuilding);
            }
            else if (buildingTypeId >= 100 && buildingTypeId < 200)
            {
                if (BuildingData.ShopData[buildingTypeId].Water)
                {
                    for (int i = cords.X; i < cords.X + building.Size.X; i++)
                    for (int j = cords.Y; j < cords.Y + building.Size.Y; j++)
                        if (GameTable[i, j].Base != TileType.Water) return false;
                }
                else
                {
                    for (int i = cords.X; i < cords.X + building.Size.X; i++)
                    for (int j = cords.Y; j < cords.Y + building.Size.Y; j++)
                        if (GameTable[i, j].Base != TileType.Grass) return false;
                }
                newBuilding = new Shop(BuildingData.ShopData[buildingTypeId].Fee, BuildingData.ShopData[buildingTypeId].MinUsage, BuildingIds++, buildingTypeId, cords);
                type = TileType.Shop;
                MaxVisitors += BuildingData.ShopData[buildingTypeId].Capacity;
                GameTable.UnderConstructionList.Add(newBuilding);
            }
            else if (buildingTypeId > 300 && buildingTypeId < 400)
            {
                for (int i = cords.X; i < cords.X + building.Size.X; i++)
                for (int j = cords.Y; j < cords.Y + building.Size.Y; j++)
                    if (GameTable[i, j].Base != TileType.Grass) return false;
                if (GameTable[cords.X, cords.Y].Base != TileType.Grass) return false;
                switch (buildingTypeId)
                {
                    case 301:
                        newBuilding = new Generator(BuildingIds++, buildingTypeId, cords, 2);
                        type = TileType.GeneratorSmall;
                        break;
                    case 302:
                        newBuilding = new Generator(BuildingIds++, buildingTypeId, cords, 3);
                        type = TileType.GeneratorMedium;
                        break;
                    case 303:
                        newBuilding = new Generator(BuildingIds++, buildingTypeId, cords, 5);
                        type = TileType.GeneratorLarge;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
                GameTable.UnderConstructionList.Add(newBuilding);
            }
            else
            {
                return false;
            }
            GameTable.Balance -= building.Price;
            OnMoneyChangedFunc();
            for (int i = cords.X; i < cords.X + building.Size.X; i++)
                for (int j = cords.Y; j < cords.Y + building.Size.Y; j++)
                {
                    GameTable.Tiles[i, j].BuildingType = type;
                    GameTable.Tiles[i, j].Building = newBuilding;
                }

            return true;
        }

        /// <summary>
        /// Vidámpark belépő beállítása
        /// </summary>
        /// <param name="newTicketPrice">Új érték azonosítója</param>
        /// <returns>Igaz ha megváltozik, hamis ha nem.</returns>
        public bool SetTicket(int newTicketPrice)
        {
            if (newTicketPrice < 0) return false;
            GameTable.EntranceTicket = newTicketPrice;
            return true;
        }

        /// <summary>
        /// Rendszeres karbantartás kifizettetése
        /// </summary>
        private void PayMaintenance()
        {
            int sum = 0;

            if (!GameTable.Buildings.Any()) return;
            foreach (var building in GameTable.Buildings)
            {
                if (building is Facilities)
                {
                    var buildingData = BuildingData.GetBuildingType(building.BuildingTypeId) as FacilityData;
                    if (buildingData != null)
                        sum += (buildingData.UpkeepCost * 5);
                }
            }
            GameTable.Balance -= sum;
            OnMoneyChangedFunc();
        }

        /// <summary>
        /// Vidámpark megnyitása
        /// </summary>
        public void OpenPark()
        {
            GameTable.ParkStatus = ParkStatusEnum.Open;
            GameSpeed = SpeedEnum.Normal;
        }

        #region Visitor methods

        /// <summary>
        /// A Visitor célt választ az alapján, hogy milyen a kedve
        /// </summary>
        /// <param name="visitor">Látogató azonosítója</param>
        /// <returns>A cél épülettel, ha létezik, különben null</returns>
        public Buildings? GetDestinationByMood(Visitor visitor) // visszatér a megfelelő épület id-val, amire éppen szüksége van és létezik
        {
            var facilities = new List<Facilities>();

            switch (visitor.CurrentMood)
            {
                case Mood.WantsToLeave:
                    foreach (var item in GameTable.Buildings)
                        if (item is Entrance) return item;
                    break;
                case Mood.WantsToPee:
                    foreach (var item in GameTable.Buildings)
                        if (item is Restroom) facilities.Add((Facilities) item);
                    break;
                case Mood.WantsToEat:
                    foreach (var item in GameTable.Buildings)
                        if (item is Shop) facilities.Add((Facilities) item);
                    break;
                default:
                    foreach (var item in GameTable.Buildings)
                        if (item is Ride) facilities.Add((Facilities) item);
                    break;
            }

            facilities.RemoveAll(f => f.Fee >= visitor.Money || f.Status == FacilityStatus.Inactive); // kiszedjük azt ami túl drága

            if (!facilities.Any()) // ha nem marad épület amibe be tudna menni akkor el akar menni
            {
                visitor.CurrentMood = Mood.WantsToLeave;
                foreach (var item in GameTable.Buildings)
                    if (item is Entrance) return item;
            }

            int sum = facilities.Sum(f => f.Fee);
            var invertedFees = facilities.Select(f => sum - f.Fee).ToList();

            var destLimit = Rand.Next(invertedFees.Sum());

            for (var i = 0; i < facilities.Count; i++)
            {
                destLimit -= invertedFees[i];
                if (destLimit <= 0)
                    return facilities[i];
            }

            return null; // ilyenkor mi van?
        }

        /// <summary>
        /// A látogató célja alapján visszatér egy útvonallal
        /// </summary>
        /// <param name="visitor">Látogató azonosítója</param>
        /// <param name="cords">Visszatérő paraméter, az útvonal kordinátáival</param>
        /// <returns>Igazzal ha sikerült útvonalat összeállítani, hamissal ha nem</returns>
        public bool GetDestinationCords(Visitor visitor, out HashSet<Point> cords) // returns coordinates (all of them)
        {
            cords = new HashSet<Point>();

            var building = GetDestinationByMood(visitor);
            if (building == null) return false;

            var buildingSize = BuildingData.GetBuildingType(building.BuildingTypeId).Size;
            for (int x = building.Coords.X; x < building.Coords.X + buildingSize.X; x++)
            for (int y = building.Coords.Y; y < building.Coords.Y + buildingSize.Y; y++)
                cords.Add(new Point(x, y));

            if (building is Facilities)
            {
                visitor.CurrentBuilding = building;
            }

            visitor.CurrentActivity = Activity.Moving;
            return true;
        }
        /// <summary>
        /// Épületekben lévő látogatók lekezlése/frissítése ha lebontjuk az épületet
        /// </summary>
        /// <param name="building">Épület azonosítója</param>
        public void DeletedFacility(Facilities building)
        {
            foreach (var visitor in building.UserList)
            {
                visitor.Coords = visitor.PrevTile;
                visitor.CurrentActivity = Activity.Nothing;
                visitor.CurrentBuilding = null;
            }
            foreach (var visitor in GameTable.Visitors)
            {
                if (visitor.CurrentBuilding == building && visitor.CurrentActivity == Activity.Moving)
                {
                    visitor.Path.Clear();
                    visitor.NextTile = visitor.Coords;
                    visitor.CurrentActivity = Activity.Nothing;
                }
            }
            foreach (var visitor in building.VisitorQueue)
            {
                visitor.Coords = visitor.PrevTile;
                visitor.Path.Clear();
                visitor.CurrentActivity = Activity.Nothing;
            }
        }
        /// <summary>
        /// Ha utat törlünk, ellenőrizze-e hogy van-e visitor aki rajta van vagy átmenne rajta
        /// </summary>
        /// <param name="building">Épület azonosítója</param>
        public void DeletedRoad(Buildings building)
        {
            foreach (var visitor in GameTable.Visitors)
            {
                if (visitor.Path.Contains(building.Coords))
                {
                    visitor.Path.Clear();
                    var newDest = new HashSet<Point>();
                    GetDestinationCords(visitor, out newDest);
                    visitor.Path = PathFinding.PathFind(visitor.Coords, newDest);
                }
            }
        }
        /// <summary>
        /// Áramtalanított épületben lévő látogatók lekezelése/frissítése
        /// </summary>
        /// <param name="building">Épület azonosítója</param>
        /// <returns>Igazzal ha sikeres, hamissal ha nem</returns>
        public bool FacilityDeactivated(Buildings building)
        {

            var facility = building as Facilities;
            if (facility == null) return false;

            foreach (var visitor in facility.UserList)
            {
                visitor.Coords = visitor.PrevTile;
                visitor.CurrentActivity = Activity.Nothing;
                visitor.CurrentBuilding = null;
            }
            foreach (var visitor in GameTable.Visitors)
            {
                if (visitor.CurrentBuilding == building && visitor.CurrentActivity == Activity.Moving)
                {
                    visitor.Path.Clear();
                    visitor.NextTile = visitor.Coords;
                    visitor.CurrentActivity = Activity.Nothing;
                }
            }
            foreach (var visitor in facility.VisitorQueue)
            {
                visitor.Coords = visitor.PrevTile;
                visitor.Path.Clear();
                visitor.CurrentActivity = Activity.Nothing;
            }

            return true;
        }
        /// <summary>
        /// Látogatók generálása
        /// </summary>
        /// <returns>Igazzal ha tudott generálni, hamissal ha nem</returns>
        public bool GenerateVisitors()
        {
            if (GameTable.ParkStatus == ParkStatusEnum.Closed) return false;

            var generate = Rand.Next(0,MaxVisitors);
            if (generate <= GameTable.Visitors.Count)
                return false;

            var newVisitor = new Visitor(GameTable.Visitors.Count, new Point(GameTable.TableSize / 2, GameTable.TableSize - 1));

            bool enters = false;
            foreach (var building in GameTable.Buildings)
            {
                if (building is Facilities)
                {
                    var tmp = (Facilities)building;
                    if (tmp.Fee <= newVisitor.Money && !(tmp is Restroom))
                    {
                        enters = true;
                        break;
                    }
                }
            }

            if (newVisitor.Money > GameTable.EntranceTicket && enters)
            {
                
                newVisitor.MoneyChange(GameTable.EntranceTicket);
                GameTable.Balance += GameTable.EntranceTicket;
                OnMoneyChangedFunc();
                GameTable.Visitors.Add(newVisitor);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Látogatók épületbe történő belépése
        /// </summary>
        /// <param name="visitor">Látogató azonosítója</param>
        /// <param name="building">Épület azonosítója</param>
        /// <returns>Igazzal ha be tudott lépni, hamissal ha nem</returns>
        public bool EnterFacility(Visitor visitor, Buildings building)
        {
            var facility = building as Facilities;
            if (facility == null) return false;

            var facilityData = BuildingData.GetBuildingType(building.BuildingTypeId) as FacilityData;
            if (facilityData == null) return false;


            if (facility.Status == FacilityStatus.Running || facility.UserList.Count == facilityData.Capacity)
            {
                facility.VisitorQueue.Enqueue(visitor);
                visitor.CurrentActivity = Activity.Waiting;
                visitor.CurrentBuilding = building;
            }
            else if ((facility.Status == FacilityStatus.Waiting || facility.Status == FacilityStatus.Operating) && facility.UserList.Count < facilityData.Capacity)
            {
                facility.UserList.Enqueue(visitor);
                visitor.CurrentActivity = Activity.Waiting;
                visitor.CurrentBuilding = building;
            } else if (facility.Status == FacilityStatus.Inactive || facility.Status == FacilityStatus.Building)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Bulding Updating Methods
        /// <summary>
        /// Épülő épületek frissítése
        /// </summary>
        public void CheckConstructions()
        {
            for (int i = 0; i < GameTable.UnderConstructionList.Count; i++)
            {
                var building = GameTable.UnderConstructionList[i];
                building.BuildingTick++;
                if (building.BuildingTick == BuildingData.GetBuildingType(building.BuildingTypeId).BuildTime)
                {
                    building.BuildingTick = 0;
                    GameTable.UnderConstructionList.Remove(building);
                    GameTable.Buildings.Add(building);
                    switch (building)
                    {
                        case Facilities fac:
                            fac.Status = FacilityStatus.Inactive;
                            break;
                        case Generator gen:
                            gen.Status = FacilityStatus.Operating;
                            PowerNearby(building, x => ++x);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Játékok frissítése
        /// </summary>
        /// <param name="building">Épület azonosítója</param>
        /// <returns>Igazzal ha sikeresen frissült, hamissal ha nem</returns>
        public bool UpdateRide(Buildings building)
        {
            var ride = building as Ride;
            if (ride == null) return false;

            var rideData = BuildingData.GetBuildingType(building.BuildingTypeId) as RideData;
            if (rideData == null) return false;

            if (!CheckIfPowered(ride))
            {
                if (ride.Status == FacilityStatus.Running || ride.Status == FacilityStatus.Waiting)
                {
                    FacilityDeactivated(building);
                }
                ride.Status = FacilityStatus.Inactive;
            }
            else if (ride.Status == FacilityStatus.Inactive)
            {
                ride.Status = FacilityStatus.Waiting;
            }

            if (ride.Status == FacilityStatus.Inactive && GameTable.GetTileValue(ride.Coords).Powered > 0)
            {
                ride.Status = FacilityStatus.Waiting;
            }

            if (ride.Status == FacilityStatus.Waiting)
            {
                if (ride.UserList.Count >= rideData.Capacity * (ride.MinUsage / 100.0))
                {
                    ride.Status = FacilityStatus.Running;
                    GameTable.Balance -= rideData.UpkeepCost;
                    OnMoneyChangedFunc();
                    foreach (var item in ride.UserList)
                        item.CurrentActivity = Activity.Using;

                }
            }
            if (ride.Status == FacilityStatus.Running)
            {
                ride.InteractionTick++;
                if (ride.InteractionTick == rideData.InteractionLength)
                {
                    ride.InteractionTick = 0;
                    ride.Status = FacilityStatus.Waiting;
                    while (ride.UserList.Any())
                    {
                        var visitor = ride.UserList.Dequeue();
                        visitor.MoneyChange(ride.Fee);
                        GameTable.Balance += ride.Fee;
                        OnMoneyChangedFunc();
                        visitor.HappinessChange(rideData.ProvideValue);
                        visitor.CurrentActivity = Activity.Resting;
                        visitor.MoodChange();
                        visitor.CurrentBuilding = null;
                        visitor.Path.Enqueue(visitor.PrevTile);
                    }
                    while (ride.VisitorQueue.Any())
                        ride.UserList.Enqueue(ride.VisitorQueue.Dequeue());
                }
            }
            return true;
        }
        /// <summary>
        /// Boltok frissítése
        /// </summary>
        /// <param name="building">Épület azonosítója</param>
        /// <returns>Igazzal ha sikeresen frissült, hamissal ha nem</returns>
        public bool UpdateShopsAndRestroom(Buildings building)
        {
            var fac = building as Facilities;
            if (fac == null) return false;

            var facData = BuildingData.GetBuildingType(building.BuildingTypeId) as FacilityData;
            if (facData == null) return false;

            if (!CheckIfPowered(fac))
            {
                if (fac.Status == FacilityStatus.Operating || fac.Status == FacilityStatus.Waiting)
                {
                    FacilityDeactivated(building);
                }
                fac.Status = FacilityStatus.Inactive;
            }
            else if (fac.Status == FacilityStatus.Inactive)
            {
                fac.Status = FacilityStatus.Waiting;
            }

            if (fac.Status == FacilityStatus.Operating)
                if (fac.UserList.Count >= facData.Capacity * (fac.MinUsage / 100.0))
                {
                    fac.Status = FacilityStatus.Running;
                    foreach (var item in fac.UserList)
                        item.CurrentActivity = Activity.Using;
                }

            foreach (var item in fac.UserList)
            {
                item.InteractionTime++;
                if (item.InteractionTime == facData.InteractionLength)
                {
                    item.InteractionTime = 0;
                    item.MoneyChange(fac.Fee);
                    GameTable.Balance += (int) (fac.Fee / 0.9f);
                    OnMoneyChangedFunc();
                    item.CurrentActivity = Activity.Resting;
                    
                    if (fac is Shop)
                        item.FoodChange(facData.ProvideValue);

                    if (fac is Restroom)
                        item.BladderChange(facData.ProvideValue);
                }
            }
            while (fac.UserList.Any() && fac.UserList.Peek().InteractionTime == 0 && fac.UserList.Peek().CurrentActivity == Activity.Resting)
                fac.UserList.Dequeue();

            while (fac.VisitorQueue.Any() && fac.UserList.Count <= facData.Capacity)
            {
                var tmpVisitor = fac.VisitorQueue.Dequeue();
                tmpVisitor.CurrentActivity = Activity.Using;
                tmpVisitor.InteractionTime = 0;
                fac.UserList.Enqueue(tmpVisitor);
            }
            return true;
        }
        /// <summary>
        /// Szomszédos utak keresése
        /// </summary>
        /// <param name="visitor">Látogató azonosítója</param>
        /// <returns></returns>
        public Point GetNeighbouringRoad(Visitor visitor)
        {
            int x = visitor.Coords.X, y = visitor.Coords.Y;
            List<Point> possibleDest = new List<Point>();
            for (int i = 0; i < 4; i++)
            {
                int adjX = x + _rowInts[i];
                int adjY = y + _colInts[i];

                if (IsValid(adjX, adjY) && (GameTable[adjX, adjY].BuildingType == TileType.Road || GameTable[adjX, adjY].BuildingType == TileType.Pier))
                {
                    possibleDest.Add(new Point(adjX, adjY));
                }
            }
            if (possibleDest.Count == 0)
                return new Point(x, y);
            return possibleDest[Rand.Next(possibleDest.Count)];
        }

        #endregion

        #region Electricity methods
        /// <summary>
        /// Generátor lehelyezésénél a környezű tile-ok áramozása/áramtalanítása
        /// </summary>
        /// <param name="building">Épület azonosítója</param>
        /// <param name="op">Lambda függvény, hogy növelje, vagy csökkentse az áram szintet pl. (x => ++x):</param>
        public void PowerNearby(Buildings building, Func<int, int> op)
        {
            if (building.BuildingTypeId < 300 || building.BuildingTypeId > 303) return;

            int radius = 0;
            var size = new Point(1, 1);
            switch (building.BuildingTypeId)
            {
                case 300:
                    radius = 3;
                    size = new Point(3, 2);
                    break;
                case 301:
                    radius = 2;
                    break;
                case 302:
                    radius = 3;
                    size = new Point(1, 2);
                    break;
                case 303:
                    radius = 5;
                    size = new Point(2, 2);
                    break;
            }

            var minCord = new Point(building.Coords.X - radius, building.Coords.Y - radius);
            var maxCord = new Point(building.Coords.X + size.X + radius, building.Coords.Y + size.Y + radius);

            for (int i = minCord.X; i < maxCord.X; i++)
                for (int j = minCord.Y; j < maxCord.Y; j++)
                    if (!(i < 0 || j < 0 || i >= GameTable.TableSize || j >= GameTable.TableSize))
                    {
                        GameTable[i, j].Powered = op(GameTable[i, j].Powered);
                    }
        }
        /// <summary>
        /// Ellenőrzi, hogy az épület áram alatt van-e vagy sem.
        /// </summary>
        /// <param name="building">Épület azonosítója</param>
        /// <returns>Áram alatt van-e, vagy sem</returns>
        public bool CheckIfPowered(Buildings building)
        {
            var size = BuildingData.GetBuildingType(building.BuildingTypeId).Size;

            for (int i = 0; i < size.X; i++)
            for (int j = 0; j < size.Y; j++)
                if (GameTable[building.Coords.X + i, building.Coords.Y + j].Powered > 0)
                    return true;
            return false;
        }

        #endregion

        #region General methods
        /// <summary>
        /// Kaporr koordináta bent van-e vagy sem a pályán
        /// </summary>
        /// <param name="x">X koodrináta azonosítója</param>
        /// <param name="y">Y koodrináta azonosítója</param>
        /// <returns>Igaz ha belül van, hamis ha nem</returns>
        public bool IsValid(int x, int y)
        {
             if (x < 0 || y < 0 || x >= GameTable.TableSize || y >= GameTable.TableSize) return false;
             return true;
        }

        #endregion

        #region Persistence methods
        /// <summary>
        /// Játék mentése
        /// </summary>
        /// <param name="path">Hova mentse azonosítója</param>
        /// <exception cref="InvalidOperationException">Nem kapott menthető adatot</exception>
        public void SaveGame(string path)
        {
            if (DataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            DataAccess.SaveGame(path,GameTable);
        }
        /// <summary>
        /// Játék betöltése
        /// </summary>
        /// <param name="path">Honnan töltse be</param>
        /// <exception cref="InvalidOperationException">Nem kapott betölthető adatot</exception>
        public void LoadGame(string path)
        {
            if (DataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            XmlSerializer serializer = new XmlSerializer(typeof(GameTable));
            FileStream stream = DataAccess.LoadGame(path);
            var gameTable = (GameTable)serializer.Deserialize(stream);

            if (gameTable == null) return;
            gameTable.FixDeserialization(gameTable);

            foreach (var building in gameTable.Buildings)
            {
                Point bSize = BuildingData.GetBuildingType(building.BuildingTypeId).Size;
                for (int x = building.Coords.X; x < building.Coords.X + bSize.X; x++)
                {
                    for (int y = building.Coords.Y; y < building.Coords.Y + bSize.Y; y++)
                    {
                        gameTable[x, y].Building = building;
                    }
                }
                if (building is Facilities)
                {
                    var facility = (Facilities)building;
                    facility.VisitorQueue.Clear();
                    facility.UserList.Clear();
                }
            }
            foreach (var building in gameTable.UnderConstructionList)
            {
                Point bSize = BuildingData.GetBuildingType(building.BuildingTypeId).Size;
                for (int x = building.Coords.X; x < building.Coords.X + bSize.X; x++)
                {
                    for (int y = building.Coords.Y; y < building.Coords.Y + bSize.Y; y++)
                    {
                        gameTable[x, y].Building = building;
                    }
                }
            }
            foreach (var visitor in gameTable.Visitors)
            {
                if(visitor.CurrentBuilding != null)
                    foreach (var building in gameTable.Buildings)
                    {
                        if(visitor.CurrentBuilding.BuildingId == building.BuildingId)
                        {
                            visitor.CurrentBuilding = building;
                            if(visitor.CurrentBuilding is Facilities)
                            {
                                var facility = (Facilities)visitor.CurrentBuilding;
                                switch (visitor.CurrentActivity)
                                {
                                    case Activity.Waiting:
                                        facility.VisitorQueue.Enqueue(visitor);
                                        break;
                                    case Activity.Using:
                                        facility.UserList.Enqueue(visitor);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        }
                    }
            }
            GameTable = gameTable;
            NewGame(GameTable);
        }


        #endregion

        #endregion

    }
}
