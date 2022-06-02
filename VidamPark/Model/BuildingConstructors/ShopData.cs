using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Játékok statikus adatainak tárolására szolgáló osztály
    /// </summary>
    public class ShopData : FacilityData
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
        /// <param name="n">Néb</param>
        /// <param name="buildTime">Építési idő</param>
        public ShopData(Point s, int p, int id, int c, int h, int il, int uc,int f, int mu, bool ir, bool w, string n, int buildTime) : 
            base(s,p,id,c,h,il,uc,f,mu,ir,w,n, buildTime) { }
        #endregion
    }
}

