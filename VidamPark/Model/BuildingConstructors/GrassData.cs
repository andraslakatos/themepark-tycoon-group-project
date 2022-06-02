namespace Model.BuildingConstructors
{
    /// <summary>
    /// Fő osztálya
    /// </summary>
    public class GrassData : BuildingBaseData
    {

        #region Contructor
        /// <summary>
        /// Fő példányosítása
        /// </summary>
        /// <param name="size">Fű mérete</param>
        /// <param name="price">Fű ára</param>
        /// <param name="id">Fű id-je</param>
        /// <param name="name">Fű neve</param>
        public GrassData(System.Drawing.Point size, int price, int id, string name) : base(size, price, id, name, 0) { }

        #endregion
    }
}
