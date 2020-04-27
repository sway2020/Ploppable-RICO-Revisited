using System;
using Harmony;


namespace PloppableRICO
{

	/// <summary>
	///This detours the CheckCollidingBuildngs method in BuildingTool. Its based on boformers Larger Footprints mod. Many thanks to him for his work. 
	/// </summary>

	[HarmonyPatch(typeof(BuildingTool))]
	[HarmonyPatch("IsImportantBuilding")]
	[HarmonyPatch(new Type[] { typeof(BuildingInfo), typeof(Building) },
		new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
	public class BuildingToolDetour
	{
		private static bool Prefix (ref bool __result, BuildingInfo info, ref Building building)
		{
			// All we want to do here is ensure that ploppable RICO buildings are classified as "Important Buildings" (to "spare them from the wrath of the BuildingTool"...)
			if (info.m_buildingAI is PloppableOfficeAI || info.m_buildingAI is PloppableExtractor || info.m_buildingAI is PloppableResidentialAI || info.m_buildingAI is PloppableCommercialAI || info.m_buildingAI is PloppableIndustrialAI)
			{
				// Found a ploppable RICO building - set original method return value.
				__result = true;

				// Don't execute base method after this.
				return false;
			}

			// Didn't find a ploppable RICO building - go onto running the original game method.
            return true;
		}
	}
}
