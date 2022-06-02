using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// Út osztálya
    /// </summary>
    public class RoadData : BuildingBaseData
    {
        #region Constructor
        /// <summary>
        /// Út példányosítása
        /// </summary>
        /// <param name="p">Ár</param>
        /// <param name="id">Id</param>
        /// <param name="name">Név</param>
        public RoadData(int p, int id, string name) : base(new Point(1,1), p, id, name, 0) { }
        #endregion
    }
}

