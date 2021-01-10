using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ColossalFramework.Packaging;
using PloppableRICO.MessageBox;


namespace PloppableRICO
{
    /// <summary>
    /// Utilities dealing with other mods, including compatibility and functionality checks.
    /// </summary>
    public static class ModUtils
    {
        /// <summary>
        ///  Flag to determine whether or not a realistic population mod is installed and enabled.
        /// </summary>
        internal static bool realPopEnabled = false;

        // Flags for handling mod conflicts.
        private static bool conflictingMod = false;
        internal static string conflictMessage;



        /// <summary>
        /// Checks for known mod conflicts and function extenders.
        /// </summary>
        /// <returns>Whether or not Ploppable RICO should load</returns>
        internal static bool CheckMods()
        {
            // Check for conflicting mods.
            if (IsModEnabled(586012417ul))
            {
                // Original Ploppable RICO mod detected.
                conflictingMod = true;
                Debugging.Message("Original Ploppable RICO detected - RICO Revisited exiting");
                conflictMessage = Translations.Translate("PRR_CON_OPR") + " - " + Translations.Translate("PRR_CON_DWN") + "\r\n\r\n" + Translations.Translate("PRR_CON_ONE");
                return false;
            }
            else if (IsModInstalled("EnhancedBuildingCapacity"))
            {
                // Enhanced Building Capacity mod detected.
                conflictingMod = true;
                Debugging.Message("Enhanced Building Capacity mod detected - RICO Revisited exiting");
                conflictMessage = Translations.Translate("PRR_CON_EBC") + " - " + Translations.Translate("PRR_CON_DWN") + "\r\n\r\n" + Translations.Translate("PRR_CON_ONE");
                return false;
            }
            else if (IsModInstalled("VanillaGarbageBinBlocker"))
            {
                // Garbage bin controller mod detected.
                conflictingMod = true;
                Debugging.Message("Garbage Bin Controller mod detected - RICO Revisited exiting");
                conflictMessage = Translations.Translate("PRR_CON_GBC") + " - " + Translations.Translate("PRR_CON_DWN") + "\r\n\r\n" + Translations.Translate("PRR_CON_ONE");
                return false;
            }
            else if (IsModInstalled(1372431101ul))
            {
                // Painter mod detected.
                conflictingMod = true;
                Debugging.Message("Painter detected - RICO Revisited exiting");
                conflictMessage = Translations.Translate("PRR_CON_PTR") + " - " + Translations.Translate("PRR_CON_DWN") + "\r\n\r\n" + Translations.Translate("PRR_CON_PTR1");
                return false;
            }

            // No hard conflicts - check for 'soft' conflicts.
            if (IsModEnabled(924884948ul))
            {
                // Plop the Growables detected.
                conflictingMod = true;
                Debugging.Message("Plop the Growables detected");
                conflictMessage = Translations.Translate("PRR_CON_PTG") + "\r\n\r\n" + Translations.Translate("PRR_CON_PTG1");
            }

            // No conflicts - now check for realistic population mods.
            realPopEnabled = (IsModInstalled("RealPopRevisited", true) || IsModInstalled("WG_BalancedPopMod", true));

            // Check for Workshop RICO settings mod.
            if (IsModEnabled(629850626uL))
            {
                Debugging.Message("found Workshop RICO settings mod");
                Loading.mod1RicoDef = RICOReader.ParseRICODefinition("", Path.Combine(Util.SettingsModPath("629850626"), "WorkshopRICOSettings.xml"), false);
            }

            // Check for Ryuichi Kaminogi's "RICO Settings for Modern Japan CCP"
            Package modernJapanRICO = PackageManager.GetPackage("2035770233");
            if (modernJapanRICO != null)
            {
                Debugging.Message("found RICO Settings for Modern Japan CCP");
                Loading.mod2RicoDef = RICOReader.ParseRICODefinition("", Path.Combine(Path.GetDirectoryName(modernJapanRICO.packagePath), "PloppableRICODefinition.xml"), false);
            }

            return true;
        }


        /// <summary>
        /// Notifies the user if any mod conflict has been detected.
        /// Mod conflicts are determined at Loading.OnCreated(); this is called at Loading.OnLevelLoaded() to provide the notification to the user.
        /// (UI can only be accessed after loading is complete).
        /// </summary>
        internal static void NotifyConflict()
        {
            // If a conflicting mod has been detected, show the notification.
            if (conflictingMod)
            {
                OneButtonMessageBox conflictBox = MessageBoxBase.ShowModal<OneButtonMessageBox>();
                conflictBox.CaprionText = Translations.Translate("PRR_NAME");
                conflictBox.MessageText = conflictMessage;
                conflictBox.ButtonText = Translations.Translate("PRR_MES_CLS");
            }
        }


        /// <summary>
        /// Checks to see if another mod is installed, based on a provided Steam Workshop ID.
        /// </summary>
        /// <param name="id">Steam workshop ID</param>
        /// <returns>True if the mod is installed and enabled, false otherwise</returns>
        internal static bool IsModInstalled(UInt64 id)
        {
            return PluginManager.instance.GetPluginsInfo().Any(mod => (mod.publishedFileID.AsUInt64 == id));
        }


        /// <summary>
        /// Checks to see if another mod is installed and enabled, based on a provided Steam Workshop ID.
        /// </summary>
        /// <param name="id">Steam workshop ID</param>
        /// <returns>True if the mod is installed and enabled, false otherwise</returns>
        internal static bool IsModEnabled(UInt64 id)
        {
            return PluginManager.instance.GetPluginsInfo().Any(mod => (mod.publishedFileID.AsUInt64 == id && mod.isEnabled));
        }


        /// <summary>
        /// Checks to see if another mod is installed, based on a provided assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly</param>
        /// <param name="enabledOnly">True if the mod needs to be enabled for the purposes of this check; false if it doesn't matter</param>
        /// <returns>True if the mod is installed (and, if enabledOnly is true, is also enabled), false otherwise</returns>
        internal static bool IsModInstalled(string assemblyName, bool enabledOnly = false)
        {
            // Convert assembly name to lower case.
            string assemblyNameLower = assemblyName.ToLower();

            // Iterate through the full list of plugins.
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    if (assembly.GetName().Name.ToLower().Equals(assemblyNameLower))
                    {
                        Debugging.Message("found mod assembly " + assemblyName);
                        if (enabledOnly)
                        {
                            return plugin.isEnabled;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            // If we've made it here, then we haven't found a matching assembly.
            return false;
        }
    }
}