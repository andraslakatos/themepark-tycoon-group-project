using MapModel.Facility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace VidamParkTest
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void TestBuild()
        {
            //Arrange
            GameModel model = new GameModel();
            model.NewGame("TestPark", 10);
            Persistence.TileType expectedRoad = Persistence.TileType.Road;
            Persistence.TileType expectedEntrance = Persistence.TileType.Entrance;

            //Act
            model.Build(0, 5, 3);
            model.Build(0, 4, 8);

            //Assert
            Persistence.TileType tile = model.GameTable.Tiles[5, 3].BuildingType;
            Assert.AreEqual(tile, expectedRoad);

            tile = model.GameTable.Tiles[4, 8].BuildingType;
            Assert.AreEqual(tile, expectedEntrance);
        }

        [TestMethod]
        public void TestDestroy()
        {
            //Arrange
            GameModel model = new GameModel();
            model.NewGame("TestPark", 10);
            Persistence.TileType expectedEmpty = Persistence.TileType.Empty;
            Persistence.TileType expectedEntrance = Persistence.TileType.Entrance;
            model.Build(0, 5, 3);

            //Act
            model.Destroy(5, 3);
            model.Destroy(4, 8);

            //Assert
            Persistence.TileType tile = model.GameTable.Tiles[5, 3].BuildingType;
            Assert.AreEqual(tile, expectedEmpty);

            tile = model.GameTable.Tiles[4, 8].BuildingType;
            Assert.AreEqual(tile, expectedEntrance);
        }

        [TestMethod]
        public void TestPayMaintance()
        {
            //Arrange
            GameModel model = new GameModel();
            model.NewGame("TestPark", 10);
            model.DayTick = 299;
            model.Build(10, 5, 3);
            ((Facilities)model.GameTable.UnderConstructionList[0]).BuildingTick = 149;
            model.CheckConstructions();

            int expected = 9000;

            //Act
            model.Tick();

            //Assert
            Assert.AreEqual(model.GameTable.Balance, expected);
        }

        [TestMethod]
        public void TestGetDestinationCords()
        {
            //Arrange
            GameModel model = new GameModel();
            model.NewGame("TestPark", 10);
            model.Build(10, 5, 6);
            Assert.AreEqual(model.Build(10, 6, 7), true);
            while (model.GameTable.UnderConstructionList.Count != 0)
            {
                model.Tick();
            }
            model.OpenPark();
            while (!model.GenerateVisitors())
            {
            }
            HashSet<Point> outcords = new HashSet<Point>();
            bool expected = true;
            bool found;
            
            //Act
            found = model.GetDestinationCords(model.GameTable.Visitors[0], out outcords);

            //Assert
            Assert.AreEqual(expected, found);
        }

        [TestMethod]
        public void TestEnterFacility()
        {
            //Arrange
            GameModel model = new GameModel();
            model.NewGame("TestPark", 10);
            Assert.AreEqual(model.Build(10, 5, 6), true);
            while(model.GameTable.UnderConstructionList.Count!=0)
            {
                model.Tick();
            }
            model.OpenPark();
            while (!model.GenerateVisitors())
            {
            }
            bool expected = true;
            bool enters;
            

            //Act
            enters = model.EnterFacility(model.GameTable.Visitors[0], model.GameTable.Buildings[1]);

            //Assert
            Assert.AreEqual(expected, enters);
        }

        [TestMethod]
        public void TestCheckConstruction()
        {
            //Arrange
            GameModel model = new GameModel();
            model.NewGame("TestPark", 10);
            model.Build(10, 5, 6);
            ((Facilities)model.GameTable.UnderConstructionList[0]).BuildingTick = 149;

            //Act
            model.CheckConstructions();

            //Assert
            Assert.IsTrue(!model.GameTable.UnderConstructionList.Any());
            Assert.IsTrue(!(model.GameTable.Buildings[1] == null));
        }
    }
}
