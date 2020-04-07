using System;
using UnityEngine;

namespace PloppableRICO
{
    /// <summary>
    ///This class assigns the RICO settings to the prefabs. 
    /// </summary>
    ///

    public class ConvertPrefabs
    {
        public void run()
        {
            // Loop through the dictionary, and apply any RICO settings. 
            foreach (var buildingData in RICOPrefabManager.prefabHash.Values)
            {
                if (buildingData != null)
                {
                    // If asset has local settings, apply those. 
                    if (buildingData.hasLocal)
                    {
                        // If local settings disable RICO, dont convert.
                        if (buildingData.local.ricoEnabled)
                        {
                            ConvertPrefab(buildingData.local, buildingData.name);
                            continue;
                        }
                    }
                    // If not, apply author settings.
                    else if (buildingData.hasAuthor)
                    {
                        if (buildingData.author.ricoEnabled)
                        {
                            //Profiler.Info( " RUN " + buildingData.name );
                            ConvertPrefab(buildingData.author, buildingData.name);
                            continue;
                        }
                    }
                    // Finally, check for mod settings.
                    else if (buildingData.hasMod)
                    {
                        Debug.Log(buildingData.name + "Has Local");
                        ConvertPrefab(buildingData.mod, buildingData.name);
                        continue;
                    }
                }
            }
        }

        public void ConvertPrefab(RICOBuilding buildingData, string name)
        {
            var prefab = PrefabCollection<BuildingInfo>.FindLoaded(name);


            if (prefab != null)
            {

                if (buildingData.service == "dummy")
                {
                    var ai = prefab.gameObject.AddComponent<DummyBuildingAI>();

                    prefab.m_buildingAI = ai;
                    prefab.m_buildingAI.m_info = prefab;
                    try
                    {
                        prefab.InitializePrefab();
                    }
                    catch
                    {
                        Debug.Log("RICO Revisited: InitPrefab failed for dummy: " + prefab.name);
                    }
                    prefab.m_placementStyle = ItemClass.Placement.Manual;


                }
                else if (buildingData.service == "residential")
                {
                    var ai = prefab.gameObject.AddComponent<PloppableResidential>();
                    if (ai == null) throw (new Exception("Residential-AI not found."));

                    ai.m_ricoData = buildingData;
                    ai.m_constructionCost = buildingData.constructionCost;
                    ai.m_homeCount = buildingData.homeCount;

                    if (buildingData.subService == "low eco")
                    {
                        // Apply eco service if GC installed, otherwise use normal low residential.
                        if (Util.isGCinstalled())
                        {
                            InitializePrefab(prefab, ai, "Low Residential Eco - Level" + buildingData.level);
                        }
                        else
                        {
                            InitializePrefab(prefab, ai, "Low Residential - Level" + buildingData.level);
                        }
                    }
                    else if (buildingData.subService == "high eco")
                    {
                        // Apply eco service if GC installed, otherwise use normal high residential.
                        if (Util.isGCinstalled())
                        {
                            InitializePrefab(prefab, ai, "High Residential Eco - Level" + buildingData.level);
                        }
                        else
                        {
                            InitializePrefab(prefab, ai, "High Residential - Level" + buildingData.level);
                        }
                    }
                    else if (buildingData.subService == "high")
                    {
                        // Stock standard high commercial.
                        InitializePrefab(prefab, ai, "High Residential - Level" + buildingData.level);
                    }
                    else
                    {
                        // Fall back to low residential as default.
                        InitializePrefab(prefab, ai, "Low Residential - Level" + buildingData.level);

                        // If invalid subservice, report.
                        if (buildingData.subService != "low")
                        {
                            Debugging.ErrorBuffer.AppendLine("Residential building " + buildingData.name + " has invalid subservice " + buildingData.subService + "; reverting to low residential.");
                        }
                    }
                }
                else if (buildingData.service == "office")
                {
                    var ai = prefab.gameObject.AddComponent<PloppableOffice>();
                    if (ai == null) throw (new Exception("Office-AI not found."));

                    ai.m_ricoData = buildingData;
                    ai.m_workplaceCount = buildingData.workplaceCount;
                    ai.m_constructionCost = buildingData.constructionCost;

                    if (buildingData.subService == "high tech")
                    {
                        // Apply IT cluster if GC installed, otherwise use Level 3 office.
                        if (Util.isGCinstalled())
                        {
                            InitializePrefab(prefab, ai, "Office - Hightech");
                        }
                        else
                        {
                            InitializePrefab(prefab, ai, "Office - Level3");
                        }
                    }
                    else
                    {
                        // Not IT cluster - boring old ordinary office.
                        InitializePrefab(prefab, ai, "Office - Level" + buildingData.level);
                    }
                }
                else if (buildingData.service == "industrial")
                {
                    var ai = prefab.gameObject.AddComponent<PloppableIndustrial>();
                    if (ai == null) throw (new Exception("Industrial-AI not found."));

                    ai.m_ricoData = buildingData;
                    ai.m_workplaceCount = buildingData.workplaceCount;
                    ai.m_constructionCost = buildingData.constructionCost;
                    ai.m_pollutionEnabled = buildingData.pollutionEnabled;

                    if (Util.industryServices.Contains(buildingData.subService))
                        InitializePrefab(prefab, ai, Util.ucFirst(buildingData.subService) + " - Processing");
                    else
                        InitializePrefab(prefab, ai, "Industrial - Level" + buildingData.level);
                }
                else if (buildingData.service == "extractor")
                {
                    var ai = prefab.gameObject.AddComponent<PloppableExtractor>();
                    if (ai == null) throw (new Exception("Extractor-AI not found."));

                    ai.m_ricoData = buildingData;
                    ai.m_workplaceCount = buildingData.workplaceCount;
                    ai.m_constructionCost = buildingData.constructionCost;
                    ai.m_pollutionEnabled = buildingData.pollutionEnabled;

                    if (Util.industryServices.Contains(buildingData.subService))
                        InitializePrefab(prefab, ai, Util.ucFirst(buildingData.subService) + " - Extractor");
                }

                else if (buildingData.service == "commercial")
                {
                    var ai = prefab.gameObject.AddComponent<PloppableCommercial>();
                    if (ai == null) throw (new Exception("Commercial-AI not found."));

                    ai.m_ricoData = buildingData;
                    ai.m_workplaceCount = buildingData.workplaceCount;
                    ai.m_constructionCost = buildingData.constructionCost;

                    if (buildingData.subService == "eco")
                    {
                        // Apply eco specialisation if GC installed, otherwise use Level 1 low commercial.
                        if (Util.isGCinstalled())
                        {
                            // Eco commercial buildings only import food goods.
                            ai.m_incomingResource = TransferManager.TransferReason.Food;
                            InitializePrefab(prefab, ai, "Eco Commercial");
                        }
                        else
                        {
                            InitializePrefab(prefab, ai, "Low Commercial - Level1");
                        }
                    }
                    else if (buildingData.subService == "tourist")
                    {
                        // Apply tourist specialisation if AD installed, otherwise use Level 1 low commercial.
                        if (Util.isADinstalled())
                        {
                            InitializePrefab(prefab, ai, "Tourist Commercial - Land");
                        }
                        else
                        {
                            InitializePrefab(prefab, ai, "Low Commercial - Level1");
                        }
                    }
                    else if (buildingData.subService == "leisure")
                    {
                        // Apply leisure specialisation if AD installed, otherwise use Level 1 low commercial.
                        if (Util.isADinstalled())
                        {
                            InitializePrefab(prefab, ai, "Leisure Commercial");
                        }
                        else
                        {
                            InitializePrefab(prefab, ai, "Low Commercial - Level1");
                        }
                    }
                    else if (buildingData.subService == "high")
                    {
                        // Bog standard high commercial.
                        InitializePrefab(prefab, ai, "High Commercial - Level" + buildingData.level);
                    }
                    else
                    {
                        // Fall back to low commercial as default.
                        InitializePrefab(prefab, ai, "Low Commercial - Level" + buildingData.level);

                        // If invalid subservice, report.
                        if (buildingData.subService != "low")
                        {
                            Debugging.ErrorBuffer.AppendLine("Commercial building " + buildingData.name + " has invalid subService " + buildingData.subService + "; reverting to low commercial.");
                        }
                    }
                }
            }
        }

        public static void InitializePrefab(BuildingInfo prefab, PrivateBuildingAI ai, String aiClass)
        {
            prefab.m_buildingAI = ai;
            // Non-zero construction time important for other mods (Real Time, Real Construction)
            ai.m_constructionTime = 30;
            prefab.m_buildingAI.m_info = prefab;

            try
            {
                prefab.InitializePrefab();
            }
            catch
            {
                // Debug.Log("RICO Revisited: InitPrefab failed - " + prefab.name +". [This message is probably harmless]");
            }

            prefab.m_class = ItemClassCollection.FindClass(aiClass);
            prefab.m_placementStyle = ItemClass.Placement.Manual;
            prefab.m_autoRemove = true;
        }
    }
}