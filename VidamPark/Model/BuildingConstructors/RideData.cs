using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Játékok statikus adatainak példányosítása
    /// </summary>
    public class RideData : FacilityData
    {
        #region Contructor
        /// <summary>
        /// FacilityData példányosítása
        /// </summary>
        /// <param name="s">Méret</param>
        /// <param name="p">Ár</param>
        /// <param name="id">Id</param>
        /// <param name="c">Férőhely</param>
        /// <param name="h">Biztosítiott boldogság mennyisége</param>
        /// <param name="il">Benne töltendő idő tartam</param>
        /// <param name="uc">Fenntartási költség</param>
        /// <param name="f">Belépő ár kezdő értéke</param>
        /// <param name="mu">Minimum kihasználtság százalékának kezdőértéke</param>
        /// <param name="ir">Elérhető-e</param>
        /// <param name="w">Vízi épület-e</param>
        /// <param name="name">Néb</param>
        /// <param name="buildTime">Építési idő</param>
        public RideData(Point s, int p, int id, int c, int h, int il, int uc, int f, int mu, bool ir, bool w, string name, int buildTime) : base(s, p, id, c, h, il, uc, f, mu, ir, w, name, buildTime) { }

        #endregion
    }
}

