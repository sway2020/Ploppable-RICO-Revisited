using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Packaging;
using Harmony;


namespace PloppableRICO
{
	/// <summary>
	/// Patch for BuildingInfo.InitializePrefab, to read and apply RICO settings prior to prefab initialization.
	/// Doing it this way (as opposed to the previous approach of changing prefab settings after initialization) solves a whole lot of issues,
	/// and opens a lot of doors.
	/// </summary>
	[HarmonyPatch(typeof(BuildingInfo), "InitializePrefab")]
	public static class InitPatch
	{
		/// <summary>
		/// Harmony prefix patch for BuildingInfo.InitializePrefab.
		/// Reads and applies RICO settings prior to prefab initialization.
		/// </summary>
		/// <param name="__instance">Original method instance reference</param>
		/// <returns></returns>
		public static bool Prefix(ref BuildingInfo __instance)
		{
			// Create a new building record for this prefab and add it to our lists.
			var buildingData = new BuildingData
			{
				prefab = __instance,
				name = __instance.name,
				density = Loading.xmlManager.SetPrefabDensity(__instance),
				category = Loading.xmlManager.AssignCategory(__instance),
			};
			Loading.xmlManager.prefabList.Add(buildingData);
			Loading.xmlManager.prefabHash[__instance] = buildingData;

			// Search for PloppableRICODefinition.xml files with this asset.
			// Need to use FindAssetByName(string, AssetType) because FindAssetByName(string) doesn't catch all assets at this stage of initialisation
			// (those two methods are more different than you might think - discovered that the hard way).
			var asset = PackageManager.FindAssetByName(__instance.name, Package.AssetType.Object);

			if (asset != null && asset.package != null)
			{
				// Get custom asset filesystem location (if CRP pacakge).
				var crpPath = asset.package.packagePath;

				if (crpPath != null)
				{
					// Look for RICO settings file.
					var ricoDefPath = Path.Combine(Path.GetDirectoryName(crpPath), "PloppableRICODefinition.xml");

					if (File.Exists(ricoDefPath))
					{
						// Parse the file.
						// TODO - ParseRICODefinition - check need for args
						var tempRicoDef = RICOReader.ParseRICODefinition(asset.package.packageName, ricoDefPath);

						if (tempRicoDef != null)
						{
							foreach (var buildingDef in tempRicoDef.Buildings)
							{
								// Go through each building parsed and check to see if we've got a match for this prefab.
								if (MatchRICOName(buildingDef.name, __instance.name, asset.package.packageName))
								{
									// Match!  Add these author settings to our prefab dictionary.
									Debug.Log("RICO Revisited: found author settings for '" + buildingDef.name + "'.");
									Loading.xmlManager.prefabHash[__instance].author = buildingDef;
									Loading.xmlManager.prefabHash[__instance].hasAuthor = true;
								}
							}
						}
					}
				}
			}


			// Check for and add any local settings for this prefab to our list.
			if (Loading.localRicoDef != null)
			{
				// Step through our previously loaded local settings and see if we've got a match.
				foreach (var buildingDef in Loading.localRicoDef.Buildings)
				{
					if (buildingDef.name.Equals(__instance.name))
					{
						// Match!  Add these author settings to our prefab dictionary.
						Loading.xmlManager.prefabHash[__instance].local = buildingDef;
						Loading.xmlManager.prefabHash[__instance].hasLocal = true;
					}
				}
			}


			// Check for any Workshop RICO mod settings for this prefab.
			if (Loading.mod1RicoDef != null)
			{
				// Step through our previously loaded local settings and see if we've got a match.
				foreach (var buildingDef in Loading.mod1RicoDef.Buildings)
				{
					if (buildingDef.name.Equals(__instance.name))
					{
						// Match!  Add these author settings to our prefab dictionary.
						Loading.xmlManager.prefabHash[__instance].mod = buildingDef;
						Loading.xmlManager.prefabHash[__instance].hasMod = true;
					}
				}
			}

			// Check for Modern Japan CCP mod settings for this prefab.
			if (Loading.mod2RicoDef != null)
			{
				// Step through our previously loaded local settings and see if we've got a match.
				foreach (var buildingDef in Loading.mod2RicoDef.Buildings)
				{
					if (buildingDef.name.Equals(__instance.name))
					{
						// Match!  Add these author settings to our prefab dictionary.
						Loading.xmlManager.prefabHash[__instance].mod = buildingDef;
						Loading.xmlManager.prefabHash[__instance].hasMod = true;
					}
				}
			}


			// Apply appropriate RICO settings to prefab.
			if (Loading.convertPrefabs != null)
			{
				// Start with local settings.
				if (Loading.xmlManager.prefabHash[__instance].hasLocal)
				{
					// If local settings disable RICO, dont convert.
					if (Loading.xmlManager.prefabHash[__instance].local.ricoEnabled)
					{
						Loading.convertPrefabs.ConvertPrefab(Loading.xmlManager.prefabHash[__instance].local, ref __instance);
					}
				}
				// If no local settings, apply author settings.
				else if (Loading.xmlManager.prefabHash[__instance].hasAuthor)
				{
					// If author settings disable RICO, dont convert.
					if (Loading.xmlManager.prefabHash[__instance].author.ricoEnabled)
					{
						Loading.convertPrefabs.ConvertPrefab(Loading.xmlManager.prefabHash[__instance].author, ref __instance);
					}
				}
				// If none of the above, apply mod settings.
				else if (Loading.xmlManager.prefabHash[__instance].hasMod)
				{
					// If mod settings disable RICO, dont convert.
					if (Loading.xmlManager.prefabHash[__instance].mod.ricoEnabled)
					{
						Loading.convertPrefabs.ConvertPrefab(Loading.xmlManager.prefabHash[__instance].mod, ref __instance);
					}
				}
			}
			else
			{
				// This means that there's been a significant failure.  Ploppable RICO settings can't be applied.
				Debug.Log("RICO Revisited: convertPrefabs not initialised!");
			}

			return true;
		}

		public static bool MatchRICOName(string ricoName, string prefabName, string packageName)
		{
			if (prefabName.Equals(packageName + "." + ricoName + "_Data"))
			{
				return true;
			}
			if (prefabName.Equals(ricoName + "_Data"))
			{
				return true;
			}
			else if (prefabName.Equals(ricoName))
			{
				return true;
			}
			Debug.Log("RICO Revisited: couldn't match name " + ricoName + " " + prefabName + " " + packageName);
			return false;
		}
	}
}
