using ColossalFramework.UI;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using ColossalFramework.Globalization;
using UnityEngine;
using ColossalFramework.Packaging;
using System.IO;
using System.Xml.Serialization;


namespace PloppableRICO
{
    /// <summary>
    /// Keeps track of prefabs and provides the link between prefabs and RICO settings. 
    /// </summary>
    /// 
    public class RICOPrefabManager
    {
        public Dictionary<BuildingInfo, BuildingData> prefabHash;
        public List<BuildingData> prefabList;

        
        /// <summary>
        /// Attempts to assign prefabs to low, medium, or high densities.
        /// Legacy code from an attempt to introduce medium density in conjunction with the (never-completed) 'Growable Overhaul' mod by AJ3D and Boformer.
        /// Currently unused, but retained for possible future redevelopment.
        /// </summary>
        /// <param name="prefab">The prefab to be assigned</param>
        /// <returns></returns>
        public int SetPrefabDensity(BuildingInfo prefab)
        {
            if (prefab.m_collisionHeight < 20) return 0; //under 4, assign low
            else if (prefab.m_collisionHeight >= 20 & prefab.m_collisionHeight < 45) return 1; //medium
            else if (prefab.m_collisionHeight > 45) return 2; //high
            else return 1;
        }


        /// <summary>
        /// Assigns settings panel categories for prefabs.
        /// </summary>
        /// <param name="prefab">The relevant prefab</param>
        /// <returns></returns>
        public Category AssignCategory(BuildingInfo prefab)
        {
            if (prefab.m_buildingAI is MonumentAI)
            {
                return Category.Monument;
            }
            else if (prefab.m_buildingAI is ParkAI)
            {
                return Category.Beautification;
            }
            else if (prefab.m_buildingAI is PowerPlantAI)
            {
                return Category.Power;
            }
            else if (prefab.m_buildingAI is WaterFacilityAI)
            {
                return Category.Water;
            }
            else if (prefab.m_buildingAI is SchoolAI)
            {
                return Category.Education;
            }
            else if (prefab.m_buildingAI is HospitalAI)
            {
                return Category.Health;
            }
            else if (prefab.m_buildingAI is ResidentialBuildingAI)
            {
                return Category.Residential;
            }
            else if (prefab.m_buildingAI is IndustrialExtractorAI)
            {
                return Category.Industrial;
            }
            else if (prefab.m_buildingAI is IndustrialBuildingAI)
            {
                return Category.Industrial;
            }
            else if (prefab.m_buildingAI is OfficeBuildingAI)
            {
                return Category.Office;
            }
            else if (prefab.m_buildingAI is CommercialBuildingAI)
            {
                return Category.Commercial;
            }
            else return Category.Beautification;

        }


        //This is called by the settings panel. It will serialize any new local settings the player sets in game. 
        public static void SaveLocal(RICOBuilding newBuildingData)
        {
            Debug.Log("SaveLocal");

            if (File.Exists("LocalRICOSettings.xml") && newBuildingData != null)
            {
                PloppableRICODefinition localSettings = null;
                var newlocalSettings = new PloppableRICODefinition();

                var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition));

                using (StreamReader streamReader = new System.IO.StreamReader("LocalRICOSettings.xml"))
                {
                    localSettings = xmlSerializer.Deserialize(streamReader) as PloppableRICODefinition;
                }

                foreach (var buildingDef in localSettings.Buildings)
                {
                    if (buildingDef.name != newBuildingData.name)
                    {
                        newlocalSettings.Buildings.Add(buildingDef);
                    }
                }

                //newBuildingData.name = newBuildingData.name;
                newlocalSettings.Buildings.Add(newBuildingData);

                using (TextWriter writer = new StreamWriter("LocalRICOSettings.xml"))
                {
                    xmlSerializer.Serialize(writer, newlocalSettings);
                }
            }
        }
    }


    //This is the data object definition for the dictionary. It contains one entry for every ploppable building. 
    //Each entry contains up to 3 PloppableRICODef entries. 

    /// <summary>
    /// Ploppable RICO data object definition for the dictionary.
    /// Every Ploppable RICO building has one entry, with (in turn) up to three PloppableRICODef sub-entries (local, author, and/or mod).
    /// </summary>
    public class BuildingData
    {
        public BuildingInfo prefab;
        public string name;
        public Category category;
        private string m_displayName;

        // Currently non-functional - see note to 'SetPrefabDensity' above.
        public int density;

        // PloppableRICODef sub-entries (RICO settings) and flags.
        public RICOBuilding local;
        public RICOBuilding author;
        public RICOBuilding mod;
        public bool hasAuthor;
        public bool hasLocal;
        public bool hasMod;


        /// Sanitises the raw prefab name for display.
        /// Called by the settings panel fastlist.
        /// </summary>
        public string displayName
        {
            get
            {
                m_displayName = Locale.GetUnchecked("BUILDING_TITLE", name);
                if (m_displayName.StartsWith("BUILDING_TITLE"))
                {
                    m_displayName = name.Substring(name.IndexOf('.') + 1).Replace("_Data", "");
                }
                m_displayName = CleanName(m_displayName, !name.Contains("."));

                return m_displayName;
            }
        }


        /// <summary>
        /// A regex prefab name cleaner.
        /// TODO - future currently under review.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cleanNumbers"></param>
        /// <returns></returns>
        private static string CleanName(string name, bool cleanNumbers = false)
        {
            name = Regex.Replace(name, @"^{{.*?}}\.", "");
            name = Regex.Replace(name, @"[_+\.]", " ");
            name = Regex.Replace(name, @"(\d[xX]\d)|([HL]\d)", "");
            if (cleanNumbers)
            {
                name = Regex.Replace(name, @"(\d+[\da-z])", "");
                name = Regex.Replace(name, @"\s\d+", " ");
            }
            name = Regex.Replace(name, @"\s+", " ").Trim();

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        }
    }
}