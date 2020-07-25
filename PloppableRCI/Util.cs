using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ColossalFramework.Plugins;


namespace PloppableRICO
{
    // Get .crp files (for building information and pretty pictures).
    public static class Util
    {
        public static FileInfo crpFileIn(DirectoryInfo d)
        {
            try
            {
                var f = d.GetFiles("*.crp");
                if (f != null && f.Count() == 1)
                    return f[0];
            }
            catch
            {
            }
            return null;
        }


        public static int[] WorkplaceDistributionOf(string service, string subservice, string level)
        {
            // Workplace distributions by building category, subservice, and level.
            Dictionary<String, int[]> distributions = new Dictionary<String, int[]>()
            {
                { "IndustrialIndustrialFarming", new int[] { 100, 100, 0, 0, 0 } },
                { "IndustrialIndustrialForestry", new int[] { 100, 100, 0, 0, 0 } },
                { "IndustrialIndustrialOre", new int[] { 100, 20, 60, 20, 0 } },
                { "IndustrialIndustrialOil", new int[] { 100, 20, 60, 20, 0 } },
                { "IndustrialIndustrialGenericLevel1", new int[] { 100, 100, 0, 0, 0 } },
                { "IndustrialIndustrialGenericLevel2", new int[] { 100, 20, 50, 20, 0 } },
                { "IndustrialIndustrialGenericLevel3", new int[] { 100, 15, 55, 25, 5 } },
                { "OfficeNoneLevel1", new int[] { 100, 0, 40, 50, 10 } },
                { "OfficeNoneLevel2", new int[] { 100, 0, 20, 50, 30 } },
                { "OfficeNoneLevel3", new int[] { 100, 0, 0, 40, 60 } },
                { "ExtractorIndustrialFarming", new int[] { 100, 100, 0, 0, 0 } },
                { "ExtractorIndustrialForestry", new int[] { 100, 100, 0, 0, 0 } },
                { "ExtractorIndustrialOre", new int[] { 100, 20, 60, 20, 0 } },
                { "ExtractorIndustrialOil", new int[] { 100, 20, 60, 20, 0 } },
                { "CommercialCommercialTourist", new int[] { 100, 20, 20, 30, 30 } },
                { "CommercialCommercialLeisure", new int[] { 100, 30, 30, 20, 20 } },
                { "CommercialCommercialLowLevel1", new int[] { 100, 100, 0, 0, 0 } },
                { "CommercialCommercialLowLevel2", new int[] { 100, 20, 60, 20, 0 } },
                { "CommercialCommercialLowLevel3", new int[] { 100, 5, 15, 30, 50 } },
                { "CommercialCommercialHighLevel1", new int[] { 100, 0, 40, 50, 10 } },
                { "CommercialCommercialHighLevel2", new int[] { 100, 0, 20, 50, 30 } },
                { "CommercialCommercialHighLevel3", new int[] { 100, 0, 0, 40, 60 } },
                { "CommercialCommercialEco", new int[] { 100, 50, 50, 0, 0 } },
                { "OfficeOfficeHighTech", new int[] { 100, 0, 10, 40, 50 } },
            };


            distributions.Add("industrialfarming", distributions["IndustrialIndustrialFarming"]);
            distributions.Add("industrialforestry", distributions["IndustrialIndustrialForestry"]);
            distributions.Add("industrialore", distributions["IndustrialIndustrialOre"]);
            distributions.Add("industrialoil", distributions["IndustrialIndustrialOil"]);
            distributions.Add("industrialgenericLevel1", distributions["IndustrialIndustrialGenericLevel1"]);
            distributions.Add("industrialgenericLevel2", distributions["IndustrialIndustrialGenericLevel2"]);
            distributions.Add("industrialgenericLevel3", distributions["IndustrialIndustrialGenericLevel3"]);
            distributions.Add("officenoneLevel1", distributions["OfficeNoneLevel1"]);
            distributions.Add("officenoneLevel2", distributions["OfficeNoneLevel2"]);
            distributions.Add("officenoneLevel3", distributions["OfficeNoneLevel3"]);
            distributions.Add("extractorfarming", distributions["ExtractorIndustrialFarming"]);
            distributions.Add("extractorforestry", distributions["ExtractorIndustrialForestry"]);
            distributions.Add("extractorore", distributions["ExtractorIndustrialOre"]);
            distributions.Add("extractoroil", distributions["ExtractorIndustrialOil"]);
            distributions.Add("commercialtourist", distributions["CommercialCommercialTourist"]);
            distributions.Add("commercialleisure", distributions["CommercialCommercialLeisure"]);
            distributions.Add("commerciallowLevel1", distributions["CommercialCommercialLowLevel1"]);
            distributions.Add("commerciallowLevel2", distributions["CommercialCommercialLowLevel2"]);
            distributions.Add("commerciallowLevel3", distributions["CommercialCommercialLowLevel3"]);
            distributions.Add("commercialhighLevel1", distributions["CommercialCommercialHighLevel1"]);
            distributions.Add("commercialhighLevel2", distributions["CommercialCommercialHighLevel2"]);
            distributions.Add("commercialhighLevel3", distributions["CommercialCommercialHighLevel3"]);
            distributions.Add("commercialeco", distributions["CommercialCommercialEco"]);
            distributions.Add("officehigh tech", distributions["OfficeOfficeHighTech"]);

            int[] workplaceDistribution = null;

            if (distributions.ContainsKey(service + subservice))
            {
                // First try basic (level-less) sevice + subservice match.
                workplaceDistribution = distributions[service + subservice];
            }
            else if (distributions.ContainsKey(service + subservice + level))
            {
                // If not, try adding level.
                workplaceDistribution = distributions[service + subservice + level];
            }
            else if (distributions.ContainsKey(service + "none" + level))
            {
                // If not, try using "none" for subservice.
                workplaceDistribution = distributions[service + "none" + level];
            }
            else if (distributions.ContainsKey(service + "none" + level))
            {
                // If not, try using "generic" for subservice.
                workplaceDistribution = distributions[service + "generic" + level];
            }

            if (workplaceDistribution != null)
            {
                // We've got a distribution; return it.
                return workplaceDistribution;
            }
            else
            {
                // Fallback - no distribtion found - evenly assign jobs across all education levels.
                return new int[] { 100, 25, 25, 25, 25 };
            }
        }


        // Return maximum level permitted for each service.
        public static int MaxLevelOf(string service, string subservice)
        {
            return service == "residential" ? 5 :
                   service == "office" && subservice != "high tech" ? 3 :
                   service == "commercial" && subservice != "tourist" && subservice != "leisure" ? 3 :
                   service == "industrial" && subservice == "generic" ? 3 :
                   1;
        }

        // Return maximum level permitted for each subservice.
        public static int MaxLevelOf(ItemClass.SubService subService)
        {
            switch (subService)
            {
                case ItemClass.SubService.ResidentialLow:
                case ItemClass.SubService.ResidentialHigh:
                case ItemClass.SubService.ResidentialLowEco:
                case ItemClass.SubService.ResidentialHighEco:
                    return 5;
                case ItemClass.SubService.CommercialLow:
                case ItemClass.SubService.CommercialHigh:
                case ItemClass.SubService.OfficeGeneric:
                case ItemClass.SubService.IndustrialGeneric:
                    return 3;
                default:
                    return 1;

            }
        }


        // Proper name of category based on category and subservice combination.
        public static string UICategoryOf(string service, string subservice)
        {
            var category = "";
            if (service == "" || subservice == "")
                return "";

            switch (service)
            {
                case "residential":
                    category = subservice == "high" ? "reshigh" : "reslow";
                    break;
                case "commercial":
                    category = subservice == "high" ? "comhigh" : "comlow";
                    break;
                case "office":
                    category = "office";
                    break;
                case "industrial":
                    category = subservice == "generic" ? "industrial" : subservice;
                    break;
                case "extractor":
                    category = subservice;
                    break;
                case "none":
                    category = "none";
                    break;
            }
            return category;
        }


        public static List<String> industryServices = new List<String>()
        {
            "farming",
            "forest",
            "oil",
            "ore"
        };
        public static List<String> vanillaCommercialServices = new List<String>()
        {
            "low",
            "high" 
        };
        public static List<String> afterDarkCommercialServices = new List<String>()
        {
            "low",
            "high",
            "tourist",
            "leisure"
        };


        // Service name from category - forest = "Forestry", everything else is just capitalised first letter. 
        public static string ucFirst(String s)
        {
            if (s == "forest")
            {
                return "Forestry";
            }
            else
                return s.Substring(0, 1).ToUpper() + s.Substring(1);
        }


        // This is run in the SimulationStep of all the ploppable AIs. 
        public static void buildingFlags(ref Building buildingData)
        {
            // A set of flags to apply to RICO buildings before/after each sim step. Sloppy, but it avoids having to mess with simstep code. 
            buildingData.m_garbageBuffer = 100;
            buildingData.m_majorProblemTimer = 0;
            buildingData.m_levelUpProgress = 0;
            buildingData.m_flags &= ~Building.Flags.ZonesUpdated;
            buildingData.m_flags &= ~Building.Flags.Abandoned;
            buildingData.m_flags &= ~Building.Flags.Demolishing;
            // This will solve the "Turned Off" error. 
            buildingData.m_problems &= ~Notification.Problem.TurnedOff;
        }


        // Path for mod settings.
        public static string SettingsModPath(string name)
        {
            var modList = PluginManager.instance.GetPluginsInfo();
            var modPath = "null";

            foreach (var modInfo in modList)
            {
                if (modInfo.name == name)
                {
                    modPath = modInfo.modPath;
                }
            }
            return modPath;
        }


        // Check if After Dark DLC is installed.
        public static bool isADinstalled()
        {

            return SteamHelper.IsDLCOwned(SteamHelper.DLC.AfterDarkDLC);
        }


        // Check if Green Cities is installed.
        public static bool isGCinstalled()
        {
            return SteamHelper.IsDLCOwned(SteamHelper.DLC.GreenCitiesDLC);
        }
    }
}