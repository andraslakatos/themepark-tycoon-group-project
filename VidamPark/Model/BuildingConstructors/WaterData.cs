namespace Model.BuildingConstructors
{
    /// <summary>
    /// Víz osztálya
    /// </summary>
    public class WaterData : BuildingBaseData
    {
        #region Contructor
        /// <summary>
        /// Víz példányosítása
        /// </summary>
        /// <param name="size"></param>
        /// <param name="price"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public WaterData(System.Drawing.Point size, int price, int id, string name) : base(size, price, id, name, 0) { }
        #endregion
    }
}
