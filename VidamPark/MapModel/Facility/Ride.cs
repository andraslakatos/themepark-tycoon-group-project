using System.Collections.Generic;
using MapModel.Visitors;

namespace MapModel.Facility
{
    public class Ride : Facilities 
    {
        #region Contructor

        public Ride(int fee, int minUsage, FacilityStatus st, Queue<Visitor> userList, Queue<Visitor> visitorQueue, List<Visitor> comingHere, int bId, int bTypeId, System.Drawing.Point cords) :
            base(fee,minUsage, st,userList,visitorQueue, comingHere,  bId, bTypeId, cords) { }

        public Ride(int fee, int minUsage, int bId, int bTypeId, System.Drawing.Point cords) : base(fee, minUsage, bId, bTypeId, cords) { }

        public Ride() { }

        #endregion
    }
}
