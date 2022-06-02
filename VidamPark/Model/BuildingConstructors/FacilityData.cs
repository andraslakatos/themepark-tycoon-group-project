using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Szolgáltatás osztályok statikus adatainak tárolására szolgáló osztály
    /// </summary>
	public class FacilityData : BuildingBaseData
    {
        #region Properties

        public int Capacity { get; set; }
        public int ProvideValue { get; set; } // mennyi boldogságot nyújt
        public int InteractionLength { get; set; }
        public int UpkeepCost { get; set; } // mennyibe kerül a fentarthatósága periodikusan
        public int Fee { get; set; }
        public int MinUsage { get; set; }
        public bool IsReachable { get; set; } // pathfinding-hoz
        public bool Water { get; set; } // true -> vizi játék

        #endregion

        #region Constructor
        /// <summary>
        /// FacilityData példányosítása
        /// </summary>
        /// <param name="s">Méret</param>
        /// <param name="p">Ár</param>
        /// <param name="id">Id</param>
        /// <param name="c">Férőhely</param>
        /// <param name="pv">Biztosítiott szükséglet mennyisége</param>
        /// <param name="il">Benne töltendő idő tartam</param>
        /// <param name="uc">Fenntartási költség</param>
        /// <param name="f">Belépő ár kezdő értéke</param>
        /// <param name="mu">Minimum kihasználtság százalékának kezdőértéke</param>
        /// <param name="ir">Elérhető-e</param>
        /// <param name="w">Vízi épület-e</param>
        /// <param name="n">Néb</param>
        /// <param name="buildTime">Építési idő</param>
        public FacilityData(Point s, int p, int id, int c, int pv, int il, int uc,int f, int mu, bool ir, bool w, string n, int buildTime)
            :base(s,p,id, n, buildTime)
        {
            Capacity = c;
            ProvideValue = pv;
            InteractionLength = il;
            UpkeepCost = uc;
            Fee = f;
            MinUsage = mu;
            IsReachable = ir;
            Water = w;
        }

        #endregion
    }
}

