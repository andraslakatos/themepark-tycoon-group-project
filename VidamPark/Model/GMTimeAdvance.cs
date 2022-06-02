using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MapModel.Facility;
using Persistence;
using Activity = MapModel.Visitors.Activity;

namespace Model
{ 
    /// <summary>
    /// GameModel partal osztálya, amiben az idő változása és attól függő folyamatok vannak lekezelve.
    /// </summary>
    public partial class GameModel
    {
        #region Events

        public EventHandler<int> OnMoneyChanged;
        public EventHandler<TimeSpan> OnTimeChanged;
        public EventHandler<int> OnGuestNumChanged;

        /// <summary>
        /// Event caller, ha megváltozik a pénz
        /// </summary>
        private void OnMoneyChangedFunc()
        {
            if (OnMoneyChanged != null)
                OnMoneyChanged(this, GameTable.Balance);
        }
        /// <summary>
        /// Event caller, ha megváltozik az idő
        /// </summary>
        private void OnTimeChangedFunc()
        {
            if (OnTimeChanged != null)
                OnTimeChanged(this, GameTable.CurrentTime);
        }
        /// <summary>
        /// Event caller, ha megváltozik a látogatók száma
        /// </summary>
        private void OnGuestNumChangedFunc()
        {
            if (OnGuestNumChanged != null)
                OnGuestNumChanged(this, GameTable.Visitors.Count);
        }

        #endregion

        #region Properties

        public int DayTick { get;  set; }
        private int GenerateVisitorTick = 0;

        #endregion

        #region Methods
        /// <summary>
        /// Tickek változása, és minden metódus meghívása ami ez alapján váltztat valamit.
        /// </summary>
        public void Tick()
        {
            GameTable.Tick += 1;
            if (GameTable.Tick == 30)
            {
                GameTable.Tick = 0;
                GameTable.CurrentTime += TimeSpan.FromSeconds(1);
                OnTimeChangedFunc();
            }

            DayTick++;
            if (DayTick == 900)
            {
                DayTick = 0;
                PayMaintenance();
                if (GameTable.Balance <= 0)
                {
                    //GameOver
                    //return;
                }
            }

            // Building Checks
            CheckConstructions();

            foreach (var building in GameTable.Buildings)
            {
                if (building is Ride)
                {
                    UpdateRide(building); ;
                } else if (building is Shop || building is Restroom)
                {
                    UpdateShopsAndRestroom(building);
                }
            }

            GenerateVisitorTick++;
            if (GenerateVisitorTick == 20)
            {
                GenerateVisitorTick = 0;
                if (GenerateVisitors())
                {
                    OnGuestNumChangedFunc();
                }
            }

            foreach (var visitor in GameTable.Visitors)
            {
                if(visitor.NecessityChange())
                {
                    //Változott a mood
                    //ActivityChange();
                }
            }

            // Visitor movement
            foreach (var visitor in GameTable.Visitors)
            {
                var destCoords = new HashSet<Point>();
                if (visitor.Path == null || (!visitor.Path.Any() && visitor.CurrentActivity == Activity.Nothing))
                {
                    GetDestinationCords(visitor, out destCoords);
                    visitor.Path = PathFinding.PathFind(visitor.Coords, destCoords);
                    visitor.CurrentActivity = visitor.Path.Any() ? Activity.Moving : Activity.Resting;
                }
                else if (visitor.CurrentActivity == Activity.Arrived)
                {
                    EnterFacility(visitor, GameTable.GetTileValue(visitor.Coords).Building);
                }
                else if (visitor.CurrentActivity == Activity.Resting)
                {
                    if(visitor.Resting())
                        if (visitor.Path.Count == 0 && visitor.Coords == visitor.NextTile)
                        {
                            var nextP = GetNeighbouringRoad(visitor);
                            visitor.NextTile = nextP;
                        }
                }
            }

            foreach (var visitor in GameTable.Visitors)
            {
                visitor.Move();
            }

            GameTable.Visitors.RemoveAll(x => x.CurrentActivity == Activity.Left && GameTable.GetTileValue(x.Coords).BuildingType == TileType.Entrance);

            for (int i = 0; i < GameTable.Visitors.Count; i++)
            {
                if (GameTable.Visitors[i].CurrentActivity == Activity.Left)
                {
                    if (GameTable.GetTileValue(GameTable.Visitors[i].Coords).BuildingType == TileType.Entrance)
                    {
                        GameTable.Visitors.RemoveAt(i);
                    }
                    else
                    {
                        GameTable.Visitors[i].CurrentActivity = Activity.Resting;
                    }
                }
            }

        }

        #endregion
    }
}
