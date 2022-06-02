using System;
using System.Drawing;
using MapModel.Building;

namespace Persistence
{
    /// <summary>
    /// Mező típus osztálya, mezőkbül áll a játéktábla
    /// </summary>
    [Serializable]
    public class Tile
    {
        #region Properties
        public int GrassType = 0;
        public Buildings Building { get; set; }
        public TileType Base { get; set; } // ez lehet: grass/water (talaj típus)
        public int Powered { get; set; } // 0:unpowered amúgy powered
        public TileType BuildingType { get; set; } // ez minden csak nem grass/water (épület típus)

        //Base:föld / víz --> buildingtype- rá
        public Point Coords { get; set; }

        #endregion

        #region Contructor

        /// <summary>
        /// Tile példányosítása
        /// </summary>
        /// <param name="tileBase">A mező alapja, fű vagy víz</param>
        /// <param name="powered">Áram szint azonosítója</param>
        /// <param name="buildingType">Mezőn épített épület típusa</param>
        /// <param name="isCorner">Épület sarka-e a mező</param>
        /// <param name="coords">Mező koordinátái</param>
        /// <param name="grassType">Fű típusa</param>
        public Tile(TileType tileBase, int powered, TileType buildingType, bool isCorner, Point coords, int grassType)
        {
            Base = tileBase;
            Powered = powered;
            BuildingType = buildingType;
            Coords = coords;
            GrassType = grassType;
        }

        public Tile() { }

        #endregion

        #region Functions
        /// <summary>
        /// Üres-e a mező
        /// </summary>
        /// <returns>Igaz, ha üres, hamis, ha nem</returns>
        public bool IsEmpty()
        {
            return BuildingType == TileType.Empty;
        }

        #endregion

    }
}
