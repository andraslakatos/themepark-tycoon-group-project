using System;
using System.Drawing;
using System.Xml.Serialization;
using MapModel.Facility;
using MapModel.Visitors;

namespace MapModel.Building
{
    /// <summary>
    /// Épületek típusainak ősosztálya.
    /// </summary>
    [XmlInclude(typeof(Visitor))]
    [XmlInclude(typeof(Shop))]
    [XmlInclude(typeof(Ride))]
    [XmlInclude(typeof(Restroom))]
    [XmlInclude(typeof(Facilities))]
    [XmlInclude(typeof(Road))]
    [XmlInclude(typeof(Pier))]
    [XmlInclude(typeof(Entrance))]
    [XmlInclude(typeof(Generator))]
    [XmlInclude(typeof(Buildings))]
    [Serializable]
    public class Buildings
    {
        #region Properties
        public int BuildingId { get; set; } 
        public int BuildingTypeId { get; set; }
        public Point Coords { get; set; }
        public int BuildingTick { get; set; }

        #endregion

        #region Contructor

        /// <summary>
        /// Épületek példányosítása.
        /// </summary>
        /// <param name="bId">Épületek id-je</param>
        /// <param name="bTypeId">Épület típusának id-je</param>
        /// <param name="cord">Épület koordinátája</param>
        public Buildings(int bId, int bTypeId, Point cord)
        {
            BuildingId = bId;
            BuildingTypeId = bTypeId;
            Coords = cord;
            BuildingTick = 0;
        }

        /// <summary>
        /// Épületek példányosítása.
        /// </summary>
        /// <param name="bId">Épületek id-je</param>
        /// <param name="bTypeId">Épület típusának id-je</param>
        /// <param name="x">Épület x koordinátája</param>
        /// <param name="y">Épület y koordinátája</param>
        public Buildings(int bId, int bTypeId, int x, int y)
        {
            BuildingId = bId;
            BuildingTypeId = bTypeId;
            Coords = new Point(x, y);
            BuildingTick = 0;
        }

        /// <summary>
        /// Épületek üres példányosítása.
        /// </summary>
        public Buildings() { }

        #endregion
    }
}
