using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using MapModel;
using MapModel.Building;
using MapModel.Facility;
using MapModel.Visitors;

namespace Persistence
{
    /// <summary>
    /// Játék tábla osztálya
    /// </summary>
    [Serializable]
    public class GameTable
    {
        #region Constants

        public static readonly int TableSizeConst = 11;

        #endregion

        #region Properties
        public string TableName { get; set; }
        public int TableSize { get; set; }
        [XmlIgnore] 
        public Tile[,] Tiles { get; set; }
        public Tile[][] TilesToSave { get; set; }
        public TimeSpan Day { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public int Tick { get; set; }
        public int Balance { get; set; }
        public int EntranceTicket { get; set; }
        public ParkStatusEnum ParkStatus { get; set; }
        public List<Buildings> Buildings { get; set; }
        public List<Buildings> UnderConstructionList { get; set; }
        public List<Visitor> Visitors { get; set; }

        [XmlIgnore]
        private Random Rand = new Random();

        #endregion

        #region Constructor
        /// <summary>
        /// Játéktábla osztály példányosítása
        /// </summary>
        /// <param name="parkName">park nevének paramétere</param>
        /// <param name="tableSize">tábla méretének paramétere</param>
        public GameTable(string parkName, int tableSize)
        {
            Buildings = new List<Buildings>();
            UnderConstructionList = new List<Buildings>();
            Visitors = new List<Visitor>();
            TableName = parkName;
            TableSize = tableSize;
            Tiles = new Tile[TableSize, TableSize];
            for (int i = 0; i < TableSize; i++)
            for (int j = 0; j < TableSize; j++)
            {
                Tiles[i, j] = new Tile(TileType.Grass, 0, TileType.Empty, false, new Point(i, j), generateGrass());
            }
            Day = new TimeSpan(0, 3, 0);
            CurrentTime = new TimeSpan(0, 0, 0);
            Tick = 0;
            Balance = 1500;
            EntranceTicket = 100;
            ParkStatus = ParkStatusEnum.Closed;
        }
        /// <summary>
        /// Játék tábla üres konstruktor
        /// </summary>
        public GameTable(){ }

        #endregion

        #region Tile Functions
        /// <summary>
        /// Fűgenerálásának randomizálása
        /// </summary>
        /// <returns>Random fű típus-t jelölő int</returns>
        public int generateGrass()
        {
            return Rand.Next(4);
        }
        /// <summary>
        /// Tile visszaadása a pályáról
        /// </summary>
        /// <param name="x">X koordináta</param>
        /// <param name="y">Y koordináta</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Exception</exception>
        public Tile GetTileValue(int x, int y) 
        {
            if (x < 0 || x > TableSize) throw new ArgumentOutOfRangeException();
            if (y < 0 || y > TableSize) throw new ArgumentOutOfRangeException();

            return Tiles[x, y];
        }

        public Tile GetTileValue(Point cords) 
        {
            if (cords.X < 0 || cords.X > TableSize) throw new ArgumentOutOfRangeException();
            if (cords.Y < 0 || cords.Y > TableSize) throw new ArgumentOutOfRangeException();

            return Tiles[cords.X, cords.Y];
        }

        public Tile this[int x, int y]
        {
            get => GetTileValue(x, y);
        }
        /// <summary>
        /// Tile üres-e
        /// </summary>
        /// <param name="x">X koordináta</param>
        /// <param name="y">Y koordináta</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Exception</exception>
        public bool IsTileEmpty(int x, int y)
        {
            if (x < 0 || x > TableSize) throw new ArgumentOutOfRangeException();
            if (y < 0 || y > TableSize) throw new ArgumentOutOfRangeException();

            return Tiles[x, y].BuildingType == TileType.Empty;
        }

        public bool IsTileEmpty(Point cords)
        {
            if (cords.X < 0 || cords.X > TableSize) throw new ArgumentOutOfRangeException();
            if (cords.Y < 0 || cords.Y > TableSize) throw new ArgumentOutOfRangeException();

            return Tiles[cords.X, cords.Y].BuildingType == TileType.Empty;
        }
        /// <summary>
        /// Játéktábla jagged array-re konvertálás sima array-ről, mentés miatt.
        /// </summary>
        /// <returns>Jagged array típusú játék tábla</returns>
        public Tile[][] ConvertToJagged()
        {
            var jagged = new Tile[TableSize][];
            for (int i = 0; i < TableSize; i++)
            {
                jagged[i] = new Tile[TableSize];
                for (int j = 0; j < TableSize; j++)
                    jagged[i][j] = Tiles[i, j];
            }

            return jagged;
        }
        /// <summary>
        /// Játéktábla sima array-re konvertálás jagged array-ről, mentés miatt.
        /// </summary>
        /// <returns>Sima array típusú játék tábla</returns>
        public Tile[,] ConvertFromJagged()
        {
            var normal = new Tile[TableSize,TableSize];
            for (int i = 0; i < TableSize; i++)
            for (int j = 0; j < TableSize; j++)
                normal[i, j] = TilesToSave[i][j];

            return normal;
        }

        /// <summary>
        /// Mentésre alkalmas tábla készítése
        /// </summary>
        /// <returns>Mentésre alkalmas tábla</returns>
        public GameTable GetTable() // ezt haszáljuk fel mentésre
        {
            var gameTable = new GameTable();
            gameTable.TableName = TableName;
            gameTable.TableSize = TableSize;
            gameTable.Day = Day;
            gameTable.CurrentTime = CurrentTime;
            gameTable.Balance = Balance;
            gameTable.EntranceTicket = EntranceTicket;
            gameTable.Buildings = Buildings;
            foreach (var building in Buildings)
            {
                if (building is Facilities)
                {
                    var tmp = (Facilities) building;
                    tmp.SaveQueues();
                }
            }
            gameTable.UnderConstructionList = UnderConstructionList;
            gameTable.TilesToSave = ConvertToJagged();
            gameTable.ParkStatus = ParkStatus;
            gameTable.Visitors = Visitors;
            foreach (var visitor in gameTable.Visitors)
            {
                visitor.SavePath();
            }
            return gameTable;
        }
        /// <summary>
        /// Betöltött tábla átalakítása, játszható táblára
        /// </summary>
        /// <param name="gameTable">Betöltött átéktábla</param>
        public void FixDeserialization(GameTable gameTable)
        {
            Tiles = ConvertFromJagged();

            foreach (var building in gameTable.Buildings)
            {
                if (building is Facilities)
                {
                    var tmp = (Facilities) building;
                    tmp.LoadQueues();
                }
            }

            foreach (var building in gameTable.UnderConstructionList)
            {
                if (building is Facilities)
                {
                    var tmp = (Facilities)building;
                    tmp.LoadQueues();
                }
            }

            if (gameTable.Visitors != null)
                foreach (var visitor in gameTable.Visitors)
                    visitor.LoadPath();
        }
        #endregion
    }
}
