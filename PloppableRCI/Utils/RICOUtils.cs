namespace PloppableRICO
{
    internal static class RICOUtils
    {
        /// <summary>
        /// Returns the currently applied RICO settings (RICO building) for the provided BuilingData instance.
        /// </summary>
        /// <param name="buildingData">BuildingData record</param>
        /// <returns>Currently active RICO building setting (null if none)</returns>
        internal static RICOBuilding CurrentRICOSetting(BuildingData buildingData)
        {
            if (buildingData.hasLocal)
            {
                return buildingData.local;
            }
            else if (buildingData.hasAuthor)
            {
                return buildingData.author;
            }
            else if (buildingData.hasMod)
            {
                return buildingData.mod;
            }

            return null;
        }
    }
}
