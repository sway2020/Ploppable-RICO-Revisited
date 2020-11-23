using System.IO;
using System.Collections.Generic;
using ICities;


namespace PloppableRICO
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        // Internal instances.
        internal static RICOPrefabManager xmlManager;
        internal static ConvertPrefabs convertPrefabs;

        // Broken prefabs list.
        internal static List<BuildingInfo> brokenPrefabs;

        // RICO definitions.
        internal static PloppableRICODefinition localRicoDef;
        internal static PloppableRICODefinition mod1RicoDef;
        internal static PloppableRICODefinition mod2RicoDef;

        // Internal flags.
        private static bool isModEnabled;
        internal static bool patchOperating;


        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            // Don't do anything if not in game (e.g. if we're going into an editor).
            if (loading.currentMode == AppMode.AssetEditor || loading.currentMode == AppMode.MapEditor || loading.currentMode == AppMode.ThemeEditor)
            {
                isModEnabled = false;
                Debugging.Message("not loading into game, skipping activation - AppMode is " + loading.currentMode);
            }
            else
            {
                // Check for conflicting (and other) mods.
                isModEnabled = ModUtils.CheckMods();
            }

            // If we're not enabling the mod due to one of the above checks failing, unapply Harmony patches before returning without doing anything.
            if (!isModEnabled)
            {
                Patcher.UnpatchAll();
                return;
            }

            // Make sure patches have been applied before proceeding.
            if (!Patcher.Patched)
            {
                Debugging.Message("Harmony patches not applied, exiting");
                isModEnabled = false;
                return;
            }

            // Otherwise, game on!
            Debugging.Message("v" + PloppableRICOMod.Version + " loading");

            // Ensure patch watchdog flag is properly initialised.
            patchOperating = false;

            // Create instances if they don't already exist.
            if (convertPrefabs == null)
            {
                convertPrefabs = new ConvertPrefabs();
            }

            if (xmlManager == null)
            {
                xmlManager = new RICOPrefabManager
                {
                    prefabHash = new Dictionary<BuildingInfo, BuildingData>(),
                };
            }

            // Reset broken prefabs list.
            brokenPrefabs = new List<BuildingInfo>();

            // Read any local RICO settings.
            string ricoDefPath = "LocalRICOSettings.xml";
            localRicoDef = null;

            if (!File.Exists(ricoDefPath))
            {
                Debugging.Message("no " + ricoDefPath + " file found");
            }
            else
            {
                localRicoDef = RICOReader.ParseRICODefinition("", ricoDefPath, isLocal: true);

                if (localRicoDef == null)
                {
                    Debugging.Message("no valid definitions in " + ricoDefPath);
                }
            }

            base.OnCreated(loading);
        }


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            // Alert the user to any mod conflicts.
            ModUtils.NotifyConflict();

            // Don't do anything further if mod hasn't activated (conflicting mod detected, or loading into editor instead of game).
            if (!isModEnabled)
            {
                return;
            }

            base.OnLevelLoaded(mode);

            // Don't do anything if in asset editor.
            if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset)
            {
                return;
            }

            // Wait for loading to fully complete.
            while (!LoadingManager.instance.m_loadingComplete) { }

            // Check watchdog flag.
            if (!patchOperating)
            {
                // Patch wasn't operating; display warning notification and exit.
                HarmonyNotification notification = new HarmonyNotification();
                notification.Create();
                notification.Show();

                return;
            }

            // Report any broken assets and remove from our prefab dictionary.
            foreach (BuildingInfo prefab in brokenPrefabs)
            {
                Debugging.Message("broken prefab: " + prefab.name);
                xmlManager.prefabHash.Remove(prefab);
            }
            brokenPrefabs.Clear();

            // Init Ploppable Tool panel.
            PloppableTool.Initialize();

            // Add buttons to access building details from zoned building info panels.
            SettingsPanel.AddInfoPanelButtons();

            // Report any loading errors.
            Debugging.ReportErrors();
            
            Debugging.Message("loading complete");


            // Display update notification.
            WhatsNew.ShowWhatsNew();

            // Set up options panel event handler.
            OptionsPanel.OptionsEventHook();

        }
    }
}