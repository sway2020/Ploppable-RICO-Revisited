using System.Collections.Generic;


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
            if (prefab != null && Loading.xmlManager.prefabHash.ContainsKey(prefab))
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
            if (prefab != null && Loading.xmlManager.prefabHash.ContainsKey(prefab))
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
            if (prefab != null && Loading.xmlManager.prefabHash.ContainsKey(prefab))
            {
                // Get active RICO settings.
                RICOBuilding building = RICOUtils.CurrentRICOSetting(Loading.xmlManager.prefabHash[prefab]);

                // Check that it's enabled and isn't using reality.
                if (building != null && building.ricoEnabled && !building.UseReality)
                {
                    return true;
                }
            }

            // If we got here, we don't have an active setting.
            return false;
        }


        /// <summary>
        /// Called by other mods to clear any cached workplace settings for a given prefab (e.g. for when a Realistic Population mod's calculations have changed).
        /// Only takes affect for buidings using Realistic Population settings.
        /// </summary>
        /// <param name="prefab">Prefab to clear</param>
        public static void ClearWorkplaceCache(BuildingInfo prefab)
        {
            // Safety first!
            if (prefab == null)
            {
                Logging.Error("null prefab passed to ClearWorkplaceCache");
                return;
            }

            // Clear cache.
            ClearWorkplaceCache(prefab, Loading.xmlManager.prefabHash[prefab]);
        }


        /// <summary>
        /// Called by other mods to clear any cached workplace settings for all prefabs (e.g. for when a Realistic Population mod's calculations have changed).
        /// Only takes affect for buidings using Realistic Population settings.
        /// </summary>
        /// <param name="prefab">Prefab to clear</param>
        public static void ClearAllWorkplaceCache()
        {
            // Iterate through all settings in dictionary.
            foreach (KeyValuePair<BuildingInfo, BuildingData> entry in Loading.xmlManager.prefabHash)
            {
                // Clear cache for this entry.
                ClearWorkplaceCache(entry.Key, entry.Value);
            }
        }


        /// <summary>
        /// Clears the workplace cache for the given RICO building.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="buildingData"></param>
        private static void ClearWorkplaceCache(BuildingInfo prefab, BuildingData buildingData)
        {
            // Get active RICO settings.
            RICOBuilding building = RICOUtils.CurrentRICOSetting(buildingData);

            // Check that it's enabled and is using reality.
            if (building != null && building.ricoEnabled && building.UseReality)
            {
                // Get AI.
                PrefabAI buildingAI = prefab.GetAI();

                // Do nothing if null.
                if (buildingAI == null)
                {
                    Logging.Error("null prefabAI at ClearWorkplaceCache");
                    return;
                }

                // See if it's one of our AI types; if so, clear the cache for that AI.
                if (buildingAI is GrowableCommercialAI comAI)
                {
                    comAI.ClearWorkplaceCache();
                }
                else if (buildingAI is GrowableIndustrialAI indAI)
                {
                    indAI.ClearWorkplaceCache();
                }
                else if (buildingAI is GrowableOfficeAI offAI)
                {
                    offAI.ClearWorkplaceCache();
                }
                else if (buildingAI is GrowableExtractorAI growAI)
                {
                    growAI.ClearWorkplaceCache();
                }
            }
        }
    }
}