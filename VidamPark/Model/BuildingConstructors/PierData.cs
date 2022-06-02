using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Stég osztálya
    /// </summary>
    public class PierData : BuildingBaseData
    {
        #region Constructor
        /// <summary>
        /// Stég példányosítása
        /// </summary>
        /// <param name="p">Ár</param>
        /// <param name="id">Id</param>
        /// <param name="name">Név</param>
        public PierData(int p, int id, string name) : base(new Point(1,1), p, id, name, 0) { }

        #endregion
    }
}