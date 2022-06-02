using System.Collections.Generic;
using MapModel.Visitors;

namespace MapModel.Facility
{
    public class Restroom : Facilities
    {
        #region Contructor

        public Restroom(int fee, int minUsage, FacilityStatus st, Queue<Visitor> userList, Queue<Visitor> visitorQueue, List<Visitor> comingHere, int bId, int bTypeId, System.Drawing.Point cords) : 
            base(fee, minUsage, st, userList, visitorQueue, comingHere, bId, bTypeId, cords) { }

        public Restroom() { }

        #endregion
    }
}
