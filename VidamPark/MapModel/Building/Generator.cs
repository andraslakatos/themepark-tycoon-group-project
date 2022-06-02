using System;
using MapModel.Facility;

namespace MapModel.Building
{
    [Serializable]
    public class Generator : Buildings 
    {
        #region Properties
        public int Radius { get; set; }

        public FacilityStatus Status { get; set; }

        #endregion

        #region Contructor

        public Generator(int bId, int bTypeId, System.Drawing.Point cord, int r) : base(bId,bTypeId,cord)
        {
            Radius = r;
            Status = FacilityStatus.Building;
        }

        public Generator() { }

        #endregion
    }
}
