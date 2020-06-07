using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ColossalFramework.Plugins;
using ColossalFramework.Packaging;


namespace PloppableRICO
{
    /// <summary>
    /// Class that manages interactions with other mods, including compatibility and functionality checks.
    /// </summary>
    internal class ModUtils
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
                Debug.Log("Original Ploppable RICO detected - RICO Revisited exiting.");
                conflictMessage = Translations.GetTranslation("Original Ploppable RICO mod detected - RICO Revisited is shutting down to protect your game.\r\n\r\nOnly ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!");
                return false;
            }
            else if (IsModEnabled("EnhancedBuildingCapacity"))
            {
                // Enhanced Building Capacity mod detected.
                conflictingMod = true;
                Debug.Log("Enhanced Building Capacity mod detected - RICO Revisited exiting.");
                conflictMessage = Translations.GetTranslation("Enhanced Building Capacity mod detected - RICO Revisited is shutting down to protect your game.\r\n\r\nnOnly ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!");
                return false;
            }
            else if (IsModInstalled(1372431101ul))
            {
                // Painter mod detected.
                conflictingMod = true;
                Debug.Log("Painter detected - RICO Revisited exiting.");
                conflictMessage = Translations.GetTranslation("Old Painter mod detected - RICO Revisited is shutting down to protect your game.\r\n\r\nThe old Painter mod causes problems with the Harmony libraries used by this mod, resulting in random errors.  Please UNSUBSCRIBE from Painter (merely disabling is NOT sufficient); the Repaint mod can be used as a replacement.");
                return false;
            }

            // No conflicts - now check for realistic population mods.
            realPopEnabled = (IsModEnabled("RealPopRevisited") || IsModEnabled("WG_BalancedPopMod"));

            // Check for Workshop RICO settings mod.
            if (IsModEnabled(629850626uL))
            {
                Debug.Log("RICO Revisited: found Workshop RICO settings mod.");
                Loading.mod1RicoDef = RICOReader.ParseRICODefinition("", Path.Combine(Util.SettingsModPath("629850626"), "WorkshopRICOSettings.xml"), false);
            }

            // Check for Ryuichi Kaminogi's "RICO Settings for Modern Japan CCP"
            Package modernJapanRICO = PackageManager.GetPackage("2035770233");
            if (modernJapanRICO != null)
            {
                Debug.Log("RICO Revisited: found RICO Settings for Modern Japan CCP.");
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
                ConflictNotification notification = new ConflictNotification();
                notification.Create();
                notification.instance.messageText = conflictMessage;
                notification.Show();
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
        /// Checks to see if another mod is installed and enabled, based on a provided assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly</param>
        /// <returns>True if the mod is installed and enabled, false otherwise</returns>
        internal static bool IsModEnabled(string assemblyName)
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
                        Debug.Log("RICO Revisited: found mod assembly " + assemblyName + " with status " + (plugin.isEnabled ? "enabled." : "disabled."));
                        return plugin.isEnabled;
                    }
                }
            }

            // If we've made it here, then we haven't found a matching assembly.
            return false;
        }
    }
}