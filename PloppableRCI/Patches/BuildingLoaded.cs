using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Math;
using Harmony;


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
			if (Loading.ricoDef != null)
			{
				// Step through each definition from the local settings file, looking for a match.
				foreach (RICOBuilding building in Loading.ricoDef.Buildings)
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

						// Basic game code processing to complete the initialisation.
						int visitCount = __instance.CalculateVisitplaceCount((ItemClass.Level)data.m_level, new Randomizer(buildingID), data.Width, data.Length);
						EnsureCitizenUnits(ref __instance, buildingID, ref data, homeCount, workCount, visitCount, 0);

						// We've set things up here for Ploppable RICO - don't fall through to game code.
						return false;
					}
				}
			}

			// If we've hit this point, then no Ploppable RICO setup has occured - fall through to game code.
			return true;
		}


		// Copied from game code as placeholder pending implementation of Harmony 2 (reverse redirect required to access private game method).
		// TODO - won't be needed after Harmony 2 reverse redirect applied above.
		public static void EnsureCitizenUnits(ref PrivateBuildingAI __instance, ushort buildingID, ref Building data, int homeCount, int workCount, int visitCount, int studentCount)
		{
			if ((data.m_flags & (Building.Flags.Abandoned | Building.Flags.Collapsed)) != 0)
			{
				return;
			}
			Citizen.Wealth wealthLevel = Citizen.GetWealthLevel((ItemClass.Level)data.m_level);
			CitizenManager instance = Singleton<CitizenManager>.instance;
			uint num = 0u;
			uint num2 = data.m_citizenUnits;
			int num3 = 0;
			while (num2 != 0)
			{
				CitizenUnit.Flags flags = instance.m_units.m_buffer[num2].m_flags;
				if ((flags & CitizenUnit.Flags.Home) != 0)
				{
					instance.m_units.m_buffer[num2].SetWealthLevel(wealthLevel);
					homeCount--;
				}
				if ((flags & CitizenUnit.Flags.Work) != 0)
				{
					workCount -= 5;
				}
				if ((flags & CitizenUnit.Flags.Visit) != 0)
				{
					visitCount -= 5;
				}
				if ((flags & CitizenUnit.Flags.Student) != 0)
				{
					studentCount -= 5;
				}
				num = num2;
				num2 = instance.m_units.m_buffer[num2].m_nextUnit;
				if (++num3 > 524288)
				{
					CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
					break;
				}
			}
			homeCount = Mathf.Max(0, homeCount);
			workCount = Mathf.Max(0, workCount);
			visitCount = Mathf.Max(0, visitCount);
			studentCount = Mathf.Max(0, studentCount);
			if (homeCount == 0 && workCount == 0 && visitCount == 0 && studentCount == 0)
			{
				return;
			}
			uint firstUnit = 0u;
			if (instance.CreateUnits(out firstUnit, ref Singleton<SimulationManager>.instance.m_randomizer, buildingID, 0, homeCount, workCount, visitCount, 0, studentCount))
			{
				if (num != 0)
				{
					instance.m_units.m_buffer[num].m_nextUnit = firstUnit;
				}
				else
				{
					data.m_citizenUnits = firstUnit;
				}
			}
		}
	}
}