using System.Drawing;

namespace Model.BuildingConstructors
{
    /// <summary>
    /// WC osztálya
    /// </summary>
    public class RestRoomData : FacilityData
    {
        #region Contructor
        /// <summary>
        /// Wc példányosítása
        /// </summary>
        /// <param name="size">Méret</param>
        /// <param name="price">Ár</param>
        /// <param name="buildId">Id</param>
        /// <param name="capacity">Férőhely</param>
        /// <param name="provideValue">Biztosítiott szükséglet mennyisége</param>
        /// <param name="interactionLength">Benne töltendő idő tartam</param>
        /// <param name="upkeepCost">Fenntartási költség</param>
        /// <param name="fee">Belépő ár kezdő értéke</param>
        /// <param name="minUsage">Minimum kihasználtság százalékának kezdőértéke</param>
        /// <param name="isReachable">Elérhető-e</param>
        /// <param name="name">Néb</param>
        /// <param name="buildTime">Építési idő</param>
        public RestRoomData
            (int capacity, int provideValue , int interactionLength, int upkeepCost, int fee, int minUsage, bool isReachable, Point size, int price, int buildId, string name, int buildTime) 
            : base(size, price, buildId, capacity, provideValue, interactionLength, upkeepCost, fee, minUsage, isReachable, false, name, buildTime) { }

        #endregion
    }
}