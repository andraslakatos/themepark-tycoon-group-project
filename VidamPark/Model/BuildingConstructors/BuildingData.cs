using System.Collections.Generic;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Adatbázis konteksztusa, épületek számontartására
    /// </summary>
	public class BuildingData
    {
        #region Properties
        public int RideCount { get; set; }
        public int ShopCount { get; set; }
        public int GeneratorCount { get; set; }
        public Dictionary<int,ShopData> ShopData { get; set; }
        public Dictionary<int, RideData> RideData { get; set; }
        public Dictionary<int, GeneratorData> GeneratorData { get; set; }
        public RoadData RoadData { get; set; }
        public PierData PierData { get; set; }
        public WaterData WaterData { get; set; }
        public GrassData GrassData { get; set; }
        public RestRoomData RestRoomData { get; set; }

        #endregion

        #region Constructor

        public BuildingData()
        {
            ShopData = new Dictionary<int, ShopData>();
            RideData = new Dictionary<int, RideData>();
            GeneratorData = new Dictionary<int, GeneratorData>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Building típus lekérdezése
        /// </summary>
        /// <param name="buildingId">Épület id azonosítója</param>
        /// <returns>Épület adat típus</returns>
        public BuildingBaseData GetBuildingType(int buildingId) 
        {
            if (buildingId == 0) return RoadData;
            if (buildingId == 1) return PierData;
            if (buildingId == 2) return WaterData;
            if (buildingId == 3) return GrassData;
            if (buildingId == 4) return RestRoomData;
            if (buildingId >= 10 && buildingId < 100) return RideData[buildingId];
            if (buildingId >= 100 && buildingId < 200) return ShopData[buildingId];
            if (buildingId >= 300 && buildingId < 400) return GeneratorData[buildingId];
            return null;
        }
        /// <summary>
        /// Épület ár lekérdezése
        /// </summary>
        /// <param name="name">Épület id azonosítója</param>
        /// <returns>Épület ára</returns>
        public int GetPrice(string name)
        {
            BuildingData buildings = new BuildingData();
            BuildingInitializer.Initialize(buildings);

            if (name == buildings.RoadData.Name) return buildings.RoadData.Price;
            if (name == buildings.PierData.Name) return buildings.PierData.Price;
            if (name == buildings.WaterData.Name) return buildings.WaterData.Price;
            if (name == buildings.GrassData.Name) return buildings.GrassData.Price;

            if (name == buildings.RestRoomData.Name) return buildings.RestRoomData.Price;

            for (int i =10;i<10+buildings.RideCount;i++)
            {
                if (name == buildings.RideData[i].Name) return buildings.RideData[i].Price;
            }
            for (int i = 100; i <100+ buildings.ShopCount; i++)
            {
                if (name == buildings.ShopData[i].Name) return buildings.ShopData[i].Price;
            }
            for (int i = 301; i < 301 + buildings.GeneratorCount; i++)
            {
                if (name == buildings.GeneratorData[i].Name) return buildings.GeneratorData[i].Price;
            }
            return 0;
        }
        /// <summary>
        /// Épület méretének lekérdezése
        /// </summary>
        /// <param name="name">Épület neve</param>
        /// <returns>Épület mérete stringben</returns>
        public string GetSize(string name)
        {
            BuildingData buildings = new BuildingData();
            BuildingInitializer.Initialize(buildings);

            if (name == buildings.RestRoomData.Name) return "Size: "+buildings.RestRoomData.Size.X+"x"+ buildings.RestRoomData.Size.Y;

            for (int i = 10; i < 10 + buildings.RideCount; i++)
            {
                if (name == buildings.RideData[i].Name) return "Size: " + buildings.RideData[i].Size.X + "x" + buildings.RideData[i].Size.Y;
            }
            for (int i = 100; i < 100 + buildings.ShopCount; i++)
            {
                if (name == buildings.ShopData[i].Name) return "Size: " + buildings.ShopData[i].Size.X + "x" + buildings.ShopData[i].Size.Y;
            }
            for (int i = 301; i < 301 + buildings.GeneratorCount; i++)
            {
                if (name == buildings.GeneratorData[i].Name) return "Size: " + buildings.GeneratorData[i].Size.X + "x" + buildings.GeneratorData[i].Size.Y;
            }
            return "";
        }
        /// <summary>
        /// Épület férőhejet vagy rádiuszt ad vissza attól függően hogy generátor-e vagy más
        /// </summary>
        /// <param name="name">Épület név</param>
        /// <returns>Épület max férőhelye vagy rádiusza</returns>
        public string GetCapacityORPowerRadius(string name)
        {
            BuildingData buildings = new BuildingData();
            BuildingInitializer.Initialize(buildings);

            if (name == buildings.RestRoomData.Name) return "Capacity: "+buildings.RestRoomData.Capacity.ToString();

            for (int i = 10; i < 10 + buildings.RideCount; i++)
            {
                if (name == buildings.RideData[i].Name) return "Capacity: " + buildings.RideData[i].Capacity.ToString();
            }
            for (int i = 100; i < 100 + buildings.ShopCount; i++)
            {
                if (name == buildings.ShopData[i].Name) return "Capacity: " + buildings.ShopData[i].Capacity.ToString();
            }
            for (int i = 301; i < 301 + buildings.GeneratorCount; i++)
            {
                if (name == buildings.GeneratorData[i].Name) return "PowerRadius: " + buildings.GeneratorData[i].PowerRadius.ToString()+"x"+ buildings.GeneratorData[i].PowerRadius.ToString();
            }
            return "";
        }
        /// <summary>
        /// Épület fenntartási költségének lekérdezése
        /// </summary>
        /// <param name="name">Épület név</param>
        /// <returns>Épület fenntartási költsége</returns>
        public string GetUC(string name)//get upkeep cost
        {
            BuildingData buildings = new BuildingData();
            BuildingInitializer.Initialize(buildings);

            if (name == buildings.RestRoomData.Name) return  "UpkeepCost: "+buildings.RestRoomData.UpkeepCost.ToString();

            for (int i = 10; i < 10 + buildings.RideCount; i++)
            {
                if (name == buildings.RideData[i].Name) return "UpkeepCost: " + buildings.RideData[i].UpkeepCost.ToString();
            }
            for (int i = 100; i < 100 + buildings.ShopCount; i++)
            {
                if (name == buildings.ShopData[i].Name) return "UpkeepCost: " + buildings.ShopData[i].UpkeepCost.ToString();
            }
            return "";
        }
        /// <summary>
        /// Épület építési idejének lekérdezése
        /// </summary>
        /// <param name="name">Épület neve</param>
        /// <returns>Épület építési ideje</returns>
        public string GetBuildTime(string name)
        {
            BuildingData buildings = new BuildingData();
            BuildingInitializer.Initialize(buildings);

            if (name == buildings.RestRoomData.Name) return "BuildTime: " + (buildings.RestRoomData.BuildTime/30).ToString()+"sec";

            for (int i = 10; i < 10 + buildings.RideCount; i++)
            {
                if (name == buildings.RideData[i].Name) return "BuildTime: " + (buildings.RideData[i].BuildTime / 30).ToString() + "sec";
            }
            for (int i = 100; i < 100 + buildings.ShopCount; i++)
            {
                if (name == buildings.ShopData[i].Name) return "BuildTime: " + (buildings.ShopData[i].BuildTime/30).ToString()+"sec";
            }
            for (int i = 301; i < 301 + buildings.GeneratorCount; i++)
            {
                if (name == buildings.GeneratorData[i].Name) return "BuildTime: " + (buildings.GeneratorData[i].BuildTime/30).ToString()+"sec";
            }
            return "";
        }

        #endregion
    }
}

