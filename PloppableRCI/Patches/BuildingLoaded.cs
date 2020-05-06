using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;


/// <summary>
/// Patch for PrivateBuildingAI.BuildingLoaded to catch Ploppable RICO buildings and override their initialisation with Ploppable RICO assets.
/// This is used to catch Ploppable RICO buildings created from PrivateBuilding (growable) originals; normally, their original stats would initialise before Ploppable RICO can override.
/// </summary>
namespace PloppableRICO
{
    [HarmonyPatch(typeof(PrivateBuildingAI))]
    [HarmonyPatch("BuildingLoaded")]
    [HarmonyPriority(Priority.VeryHigh)]
    class RICOBuildingLoaded
    {
		public static bool Prefix(ref PrivateBuildingAI __instance, ushort buildingID, ref Building data, uint version)
		{
			// Check to see if we've preloaded a local settings file.
			if (Loading.localRicoDef != null)
			{
				// Step through each definition from the local settings file, looking for a match.
				foreach (RICOBuilding building in Loading.localRicoDef.Buildings)
				{
					if (building.ricoEnabled && __instance.m_info.name == building.name)
					{
						// m_level is one less than building.level.
						byte newLevel = (byte)(building.level - 1);

						if (data.m_level != newLevel)
						{
							Debug.Log("RICO Revisited: Found building '" + building.name + "' with level " + (data.m_level + 1) + ", overriding to level " + building.level + ".");
							data.m_level = newLevel;
						}

						// Basic game code processing to continue initialisation.
						__instance.CalculateWorkplaceCount((ItemClass.Level)data.m_level, new Randomizer(buildingID), data.Width, data.Length, out int level, out int level2, out int level3, out int level4);
						__instance.AdjustWorkplaceCount(buildingID, ref data, ref level, ref level2, ref level3, ref level4);

						int workCount = level + level2 + level3 + level4;
						int homeCount;

						// Check and override homecounts if the building is residential and we're not using Realistic Population.
						if (building.RealityIgnored && building.service == "residential")
						{
							homeCount = building.homeCount;
							Debug.Log("RICO Revisited: found Residential prefab " + building.name + " with homecount " + building.homeCount + "; forcing homecount reset.");
						}
						else
						{
							// Realistic population in use - use it's (patched) CalculateHomeCount via original method.
							homeCount = __instance.CalculateHomeCount((ItemClass.Level)data.m_level, new Randomizer(buildingID), data.Width, data.Length);
						}

						// Update citizen units to match new totals.
						int visitCount = __instance.CalculateVisitplaceCount((ItemClass.Level)data.m_level, new Randomizer(buildingID), data.Width, data.Length);
						RealisticCitizenUnits.EnsureCitizenUnits(ref __instance, buildingID, ref data, homeCount, workCount, visitCount, 0);

						// Clear any problems (so we don't have any residual issues from changing service types, for example (new) residential buildings showing 'not enough goods'.
						// Any 'genuine' problems will be quickly reapplied by the game.
						data.m_problems = 0;

						// We've set things up here for Ploppable RICO - don't fall through to game code.
						return false;
					}
				}
			}

			// If we've hit this point, then no Ploppable RICO setup has occured - fall through to game code.
			return true;
		}
	}
}