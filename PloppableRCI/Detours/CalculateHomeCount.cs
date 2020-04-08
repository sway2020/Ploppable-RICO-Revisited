using System;
using UnityEngine;
using ColossalFramework.Math;
using Harmony;


namespace PloppableRICO
{
    // Patching CalculateHomeCount in ResidentialBuildingAI to override initial HomeCount when plopped building is reloaded after a save.
    // This is to ensure that any RICO assets based off Residential templates get assigned their correct HomeCounts on reload
    // (default RICO operation is pre-empted on game load by default game AI which overrides HomeCounts as buildings are loaded from the save).
    // Must run before other patches, and specifically before Realistic Population Revisited.
    // Only needs to be run on level loading to provide intial correction prior to full RICO load; after RICO has initialised fully then this is no longer needed (so unload).
    [HarmonyPatch(typeof(ResidentialBuildingAI))]
    [HarmonyPatch("CalculateHomeCount")]
    [HarmonyPatch(new Type[] { typeof(ItemClass.Level), typeof(Randomizer), typeof(int), typeof(int) })]
    [HarmonyBefore(new string[] { "com.github.algernon-A.csl.realisticpopulationrevisited"})]
    [HarmonyPriority(Priority.VeryHigh)]
    class RICOHomeCount
    {
        public static bool Prefix(ref int __result, ref ResidentialBuildingAI __instance, ItemClass.Level level, Randomizer r, int width, int length)
        {
            // If we've preloaded a local settings file and this is a residential asset, then this building is in play.
            if (Loading.ricoDef != null && __instance.m_info.GetService() == ItemClass.Service.Residential)
            {
                // Step through each definition from the local settings file, looking for a name match that has RealityIgnored set.
                foreach (RICOBuilding building in Loading.ricoDef.Buildings)
                {
                    if (building.RealityIgnored && __instance.m_info.name == building.name)
                    {
                        // Found one!
                        Debug.Log("RICO Revisited: found Residential prefab " + building.name + " with homecount " + building.homeCount + "; forcing homecount reset.");

                        // Forcibly reset homecount via original method return result.
                        __result = building.homeCount;

                        // We're done - don't execute original method (and/or any other Harmony patches) after this.
                        return false;
                    }
                }
            }

            // No match found - continue on to original method (and/or any other Harmony patches).
            return true;
        }
    }
}
