using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using MapModel.Visitors;
using MapModel.Building;


namespace MapModel.Facility
{
    /// <summary>
    /// Játékok ősosztálya.
    /// </summary>
    [Serializable]
    public class Facilities : Buildings
    {
        #region Properties

        public int Fee { get; set; }
        public int MinUsage { get; set; }
        public FacilityStatus Status { get; set; }
        [XmlIgnore] public Queue<Visitor> UserList { get; set; }
        public List<Visitor> UserListSerializable { get; set; }
        [XmlIgnore] public Queue<Visitor> VisitorQueue { get; set; }
        public List<Visitor> VisitorQueueSerializable { get; set; }

        public int InteractionTick { get; set; }

        #endregion

        #region Contructor
        /// <summary>
        /// Játékok példányosítása.
        /// </summary>
        /// <param name="fee">Fenntartási költség</param>
        /// <param name="minUsage">Minimum látogatószám, hogy elinduljon</param>
        /// <param name="st">A játék státusza</param>
        /// <param name="userList">Jelenleg a játékot használók</param>
        /// <param name="visitorQueue">A játékra várnak</param>
        /// <param name="comingHere">A játékhoz tartók</param>
        /// <param name="bId">Játék id-je</param>
        /// <param name="bTypeId">Játék típusának id-je</param>
        /// <param name="cords">Játék koordinátái</param>
        public Facilities(int fee, int minUsage, FacilityStatus st, Queue<Visitor> userList,
            Queue<Visitor> visitorQueue, List<Visitor> comingHere, int bId, int bTypeId, System.Drawing.Point cords) :
            base(bId, bTypeId, cords)
        {
            Fee = fee;
            MinUsage = minUsage;
            Status = st;
            UserList = userList;
            VisitorQueue = visitorQueue;
            InteractionTick = 0;
        }

        public Facilities(int fee, int minUsage, int bId, int bTypeId, System.Drawing.Point cords) : base(bId, bTypeId,
            cords)
        {
            Fee = fee;
            MinUsage = minUsage;
            BuildingId = bId;
            BuildingTypeId = bTypeId;
            Status = FacilityStatus.Building;
            UserList = new Queue<Visitor>();
            VisitorQueue = new Queue<Visitor>();
            InteractionTick = 0;
        }

        public Facilities()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Lementhetővé konertálja a listákat.
        /// </summary>
        public void SaveQueues()
        {
            UserListSerializable = new List<Visitor>();
            UserListSerializable = UserList.ToList();
            VisitorQueueSerializable = new List<Visitor>();
            VisitorQueueSerializable = VisitorQueue.ToList();
        }

        /// <summary>
        /// Visszatölti a lementett listákat.
        /// </summary>
        public void LoadQueues()
        {
            UserList = new Queue<Visitor>();
            foreach (var visitor in UserListSerializable)
                UserList.Enqueue(visitor);
            VisitorQueue = new Queue<Visitor>();
            foreach (var visitor in VisitorQueueSerializable)
                VisitorQueue.Enqueue(visitor);
            //Debug.WriteLine(UserList.Count + " " + VisitorQueue.Count);
        }

    #endregion
    }
}
