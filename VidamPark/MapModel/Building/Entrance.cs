using System;

namespace MapModel.Building
{
    [Serializable]
    public class Entrance : Generator
    {
        #region Contructor
        public Entrance(int bId, int bTypeId, System.Drawing.Point cord, int radius) : base(bId, bTypeId, cord, radius) 
        { }
        
        public Entrance()
        { }

        #endregion
    }
}
