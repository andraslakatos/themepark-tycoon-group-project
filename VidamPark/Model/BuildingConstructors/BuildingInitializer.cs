using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Az adatbázis feltöltése épületekkel
    /// </summary>
    internal class BuildingInitializer
    {
        public static void Initialize(BuildingData buildingData)
        {
            // Utak
            //price--id--name
            buildingData.RoadData = new RoadData(100, 0, "Road");
            buildingData.PierData = new PierData(150, 1, "Pier");
            buildingData.WaterData = new WaterData(new Point(1,1), 100, 2, "Water");
            buildingData.GrassData = new GrassData(new Point(1, 1), 50, 3, "Grass");


            //Játékok
            buildingData.RideCount = 6;
            ////size--price--id--capacity--happiness--interactionlength--upkeepcost--fee--minimumusage--isreacheable--water--name--buildtime(in ticks)
            ///starter money: 1500
            ///visitor: 0-1500
            
            buildingData.RideData[10] = new RideData(new Point(1,1), 500,  10,  10 , 20,  150,  100,  50,   3, false,false, "Circus", 150);
            buildingData.RideData[11] = new RideData(new Point(2,1), 750,  11,  10,  30,  180,   120, 70,  5, false, false, "PirateShip", 180);
            buildingData.RideData[12] = new RideData(new Point(2, 2),1000, 12, 15,  40,   300,   150,  100, 5, false, false, "FerrisWheel", 210);
            buildingData.RideData[13] = new RideData(new Point(4, 4), 5000, 13, 30,  90,   300,  200, 100, 10, false, false, "RollerCoaster", 300);

            buildingData.RideData[14] = new RideData(new Point(1, 1), 500, 14, 10, 30,    150, 100,  70,  2, false, true, "WaterSlide", 300);
            buildingData.RideData[15] = new RideData(new Point(2,2), 1000, 15, 20, 50,    300, 200, 100, 5 , false, true, "BigWaterSlide", 600);
            //ha több játék->getRideCount++


            //Étterem + Bolt
            buildingData.ShopCount = 3;
            ////size--price--id--capacity--happiness--interactionlength--upkeepcost--fee--mu--isreacheable--water--name--buildtime(in ticks)
            buildingData.ShopData[100] = new ShopData(new Point(1,1),  300, 100,5,20,   60, 100, 10, 2,false,false, "CoffeeShop", 150);
            buildingData.ShopData[101] = new ShopData(new Point(1, 1), 300, 101, 10, 30, 120, 100, 30, 2, false, false, "IceCreamCart", 150);
            buildingData.ShopData[102] = new ShopData(new Point(2, 2), 1500, 102, 30, 60, 300, 500, 100, 10, false, false, "Restaurant", 300);

            //WC
            buildingData.RestRoomData = new RestRoomData(20, 100, 100, 0, 0, 0, false, new Point(1, 1), 300, 4, "Restroom", 200);

            //Bejárat
            //power radious: 3
            buildingData.GeneratorData[300] = new GeneratorData(3, new Point(3, 2), 0, 300, "Entrance", 0);

            //Generátorok
            buildingData.GeneratorCount = 3;
            buildingData.GeneratorData[301] = new GeneratorData(2, new Point(1, 1), 500, 301, "GeneratorSmall", 30);
            buildingData.GeneratorData[302] = new GeneratorData(3, new Point(1, 2), 1000, 302, "GeneratorMedium", 90);
            buildingData.GeneratorData[303] = new GeneratorData(5, new Point(2, 2), 2000, 303, "GeneratorLarge", 300);
        }
    }
}
