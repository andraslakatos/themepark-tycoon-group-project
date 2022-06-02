using System.Drawing;

namespace Model.BuildingConstructors
{
    public class GeneratorData : BuildingBaseData
    {
        #region Properties
        /// <summary>
        /// Generátor statikus adatainak tárolására szolgáló osztály
        /// </summary>
        public int PowerRadius { get; set; }

        #endregion

        #region Contructor
        /// <summary>
        /// Generátor példányosítása
        /// </summary>
        /// <param name="pr">Áram biztosításának rádiusza</param>
        /// <param name="size">Méret</param>
        /// <param name="price">Ár</param>
        /// <param name="bId">Id</param>
        /// <param name="buildTime">Építési idő</param>
        public GeneratorData(int pr, Point size, int price, int bId, string name, int buildTime) 
            : base(size, price, bId, name, buildTime)
        {
            PowerRadius = pr;
        }

        #endregion
    }
}
