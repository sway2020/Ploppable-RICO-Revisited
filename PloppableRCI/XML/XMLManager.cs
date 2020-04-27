using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using ColossalFramework.Globalization;


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
            if (prefab.GetService() == ItemClass.Service.Monument)
            {
                return Category.Monument;
            }
            else if (prefab.GetService() == ItemClass.Service.Beautification)
            {
                return Category.Beautification;
            }
            else if (prefab.GetService() == ItemClass.Service.Electricity)
            {
                return Category.Power;
            }
            else if (prefab.GetService() == ItemClass.Service.Water)
            {
                return Category.Water;
            }
            else if (prefab.GetService() == ItemClass.Service.Education)
            {
                return Category.Education;
            }
            else if ( prefab.GetService() == ItemClass.Service.PlayerEducation)
            {
                return Category.Education;
            }
            else if (prefab.GetService() == ItemClass.Service.HealthCare)
            {
                return Category.Health;
            }
            else if (prefab.GetService() == ItemClass.Service.PoliceDepartment)
            {
                return Category.Health;
            }
            else if (prefab.GetService() == ItemClass.Service.FireDepartment)
            {
                return Category.Health;
            }
            else if (prefab.GetService() == ItemClass.Service.Residential)
            {
                return Category.Residential;
            }
            else if (prefab.GetService() == ItemClass.Service.Industrial)
            {
                return Category.Industrial;
            }
            else if (prefab.GetService() == ItemClass.Service.PlayerIndustry)
            {
                return Category.Industrial;
            }
            else if (prefab.GetService() == ItemClass.Service.Fishing)
            {
                return Category.Industrial;
            }
            else if (prefab.GetService() == ItemClass.Service.Garbage)
            {
                return Category.Industrial;
            }
            else if (prefab.GetService() == ItemClass.Service.Office)
            {
                return Category.Office;
            }
            else if (prefab.GetService() == ItemClass.Service.Commercial)
            {
                return Category.Commercial;
            }
            else return Category.Beautification;

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