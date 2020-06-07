using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ColossalFramework.UI;
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

    
    /// <summary>
    /// A simple UI panel to show any mod conflict notifications.
    /// </summary>
    class ConflictNotification : UIPanel
    {
        // Constants.
        private const float panelWidth = 450;
        private const float panelHeight = 200;
        private const float spacing = 10;

        // Instance references.
        private static GameObject uiGameObject;
        private static ConflictNotification _instance;


        /// <summary>
        /// Creates the panel object in-game.
        /// </summary>
        internal void Create()
        {
            try
            {
                // Destroy existing (if any) instances.
                uiGameObject = GameObject.Find("RICORevisitedConflictNotification");
                if (uiGameObject != null)
                {
                    UnityEngine.Debug.Log("RICO Revisited: found existing upgrade notification instance.");
                    GameObject.Destroy(uiGameObject);
                }

                // Create new instance.
                // Give it a unique name for easy finding with ModTools.
                uiGameObject = new GameObject("RICORevisitedConflictNotification");
                uiGameObject.transform.parent = UIView.GetAView().transform;
                _instance = uiGameObject.AddComponent<ConflictNotification>();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }


        /// <summary>
        /// Sets up the panel; called by Unity just before any of the Update methods is called for the first time..
        /// </summary>
        public override void Start()
        {
            base.Start();

            try
            {
                // Basic setup.
                isVisible = true;
                canFocus = true;
                isInteractive = true;
                width = panelWidth;
                height = panelHeight;
                relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
                backgroundSprite = "UnlockingPanel2";

                // Put behind other things.
                zOrder = 1;

                // Title.
                UILabel title = this.AddUIComponent<UILabel>();
                title.relativePosition = new Vector3(0, spacing);
                title.textAlignment = UIHorizontalAlignment.Center;
                title.text = "Ploppable RICO Revisited: mod conflict";
                title.textScale = 1.0f;
                title.autoSize = false;
                title.width = this.width;

                // Note 1.
                UILabel note1 = this.AddUIComponent<UILabel>();
                note1.relativePosition = new Vector3(spacing, 40);
                note1.textAlignment = UIHorizontalAlignment.Left;
                note1.text = ModUtils.conflictMessage;
                note1.textScale = 0.8f;
                note1.autoSize = false;
                note1.autoHeight = true;
                note1.width = this.width - (spacing * 2);
                note1.wordWrap = true;

                // Close button.
                UIButton closeButton = UIUtils.CreateButton(this);
                closeButton.width = 200;
                closeButton.relativePosition = new Vector3((this.width - closeButton.width) / 2, this.height - closeButton.height - spacing);
                closeButton.text = "Close";
                closeButton.Enable();

                // Event handler.
                closeButton.eventClick += (c, p) =>
                {
                    // Just hide this panel and destroy the game object - nothing more to do this load.
                    this.Hide();
                    GameObject.Destroy(uiGameObject);
                };
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
    }
}