namespace PloppableRICO
{
    /// <summary>
    /// Class for external public interface methods for other mods to use.
    /// </summary>
    public static class Interfaces
    {
        /// <summary>
        /// Called by other mods to determine whether or not Ploppable RICO Revisited is managing this prefab.
        /// </summary>
        /// <param name="prefab">Prefab reference</param>
        /// <returns>True if Ploppable RICO is managing this prefab, false otherwise.</returns>
        public static bool IsRICOManaged(BuildingInfo prefab)
        {
            // First, do we have a setting at all?
            if (Loading.xmlManager.prefabHash.ContainsKey(prefab))
            {
                // Get active RICO settings.
                RICOBuilding building = RICOUtils.CurrentRICOSetting(Loading.xmlManager.prefabHash[prefab]);

                // Check that it's enabled.
                if (building != null && building.ricoEnabled)
                {
                    return true;
                }
            }

            // If we got here, we don't have an active setting.
            return false;
        }


        /// <summary>
        /// Called by other mods to determine whether or not this is a Ploppable RICO Revisited 'non-growable'.
        /// </summary>
        /// <param name="prefab">Prefab reference</param>
        /// <returns>True if this is a Ploppable RICO non-growable, false otherwise.</returns>
        public static bool IsRICOPloppable(BuildingInfo prefab)
        {
            // First, do we have a setting at all?
            if (Loading.xmlManager.prefabHash.ContainsKey(prefab))
            {
                // Get active RICO settings.
                RICOBuilding building = RICOUtils.CurrentRICOSetting(Loading.xmlManager.prefabHash[prefab]);

                // Check that it's enabled and isn't growable.
                if (building != null && building.ricoEnabled && !building.growable)
                {
                    return true;
                }
            }

            // If we got here, we don't have an active setting.
            return false;
        }


        /// <summary>
        /// Called by other mods to determine whether or not Ploppable RICO Revisited is controlling the population of this prefab.
        /// </summary>
        /// <param name="prefab">Prefab reference</param>
        /// <returns>True if Ploppable RICO is controlling the population of this prefab, false otherwise.</returns>
        public static bool IsRICOPopManaged(BuildingInfo prefab)
        {
            // First, do we have a setting at all?
            if (Loading.xmlManager.prefabHash.ContainsKey(prefab))
            {
                // Get active RICO settings.
                RICOBuilding building = RICOUtils.CurrentRICOSetting(Loading.xmlManager.prefabHash[prefab]);

                // Check that it's enabled and isn't using reality.
                if (building != null && building.ricoEnabled && !building.useReality)
                {
                    return true;
                }
            }

            // If we got here, we don't have an active setting.
            return false;
        }

    }
}