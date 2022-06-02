using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Épület statikus adatait tároló osztályok szülő osztája
    /// </summary>
    public class BuildingBaseData
    {
        #region Properties
        public Point Size { get; set; }
        public int Price { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int BuildTime { get; set; }
        #endregion

        #region Contructor
        /// <summary>
        /// BuildingBaseData példányosítása
        /// </summary>
        /// <param name="s">Méret azonosítója</param>
        /// <param name="p">Ár azonosítója</param>
        /// <param name="id">Id azonosítója</param>
        /// <param name="n">Épület neve</param>
        /// <param name="buildTime">Építkezési idő azonosítója</param>
        public BuildingBaseData(Point s, int p, int id, string n, int buildTime)
        {
            Size = s;
            Price = p;
            Id = id;
            Name = n;
            BuildTime = buildTime;
        }
        #endregion
    }
}

