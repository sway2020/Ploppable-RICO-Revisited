using System.IO;
using System.Collections.Generic;
using ICities;
using PloppableRICO.MessageBox;


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
        private static bool isModEnabled = false;
        private bool harmonyLoaded = false;
        internal static bool patchOperating = false;

        // Used to flag if conflicting mods are running.
        private static bool conflictingMod = false;
        private static bool softModConflct;


        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            // Don't do anything if not in game (e.g. if we're going into an editor).
            if (loading.currentMode != AppMode.Game)
            {
                isModEnabled = false;
                Logging.KeyMessage("not loading into game, skipping activation");

                // Set harmonyLoaded and PatchOperating flags to suppress Harmony warning when e.g. loading into editor.
                patchOperating = true;
                harmonyLoaded = true;

                // Unload Harmony patches and exit before doing anything further.
                Patcher.UnpatchAll();
                return;
            }

            // Ensure that Harmony patches have been applied.
            harmonyLoaded = Patcher.Patched;
            if (!harmonyLoaded)
            {
                isModEnabled = false;
                Logging.Error("Harmony patches not applied; aborting");
                return;
            }

            // Check for mod conflicts.
            if (ModUtils.IsModConflict())
            {
                // Conflict detected.
                conflictingMod = true;
                isModEnabled = false;

                // Unload Harmony patches and exit before doing anything further.
                Patcher.UnpatchAll();
                return;
            }

            // Passed all checks - okay to load (if we haven't already fo some reason).
            if (!isModEnabled)
            {
                isModEnabled = true;
                Logging.KeyMessage("v " + PloppableRICOMod.Version + " loading");

                // Ensure patch watchdog flag is properly initialised.
                patchOperating = false;

                // Check for other mods, including any soft conflicts.
                softModConflct = ModUtils.CheckMods();

                // Check for Advanced Building Level Control.
                ModUtils.ABLCReflection();

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
                    Logging.Message("no ", ricoDefPath, " file found");
                }
                else
                {
                    localRicoDef = RICOReader.ParseRICODefinition("", ricoDefPath, isLocal: true);

                    if (localRicoDef == null)
                    {
                        Logging.Message("no valid definitions in ", ricoDefPath);
                    }
                }
            }
        }


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Wait for loading to fully complete.
            while (!LoadingManager.instance.m_loadingComplete) { }

            // Check watchdog flag.
            if (!patchOperating)
            {
                // Patch wasn't operating; display harmony error and abort.
                harmonyLoaded = false;
                isModEnabled = false;
            }

            // Check to see that Harmony 2 was properly loaded.
            if (!harmonyLoaded)
            {
                // Harmony 2 wasn't loaded; display warning notification and exit.
                ListMessageBox harmonyBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                harmonyBox.AddParas(Translations.Translate("ERR_HAR0"), Translations.Translate("PRR_ERR_HAR"), Translations.Translate("PRR_ERR_FAT"), Translations.Translate("ERR_HAR1"));

                // List of dot points.
                harmonyBox.AddList(Translations.Translate("ERR_HAR2"), Translations.Translate("ERR_HAR3"));

                // Closing para.
                harmonyBox.AddParas(Translations.Translate("MES_PAGE"));
            }

            // Check to see if a conflicting mod has been detected.
            if (conflictingMod)
            {
                // Mod conflict detected - display warning notification and exit.
                ListMessageBox modConflictBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                modConflictBox.AddParas(Translations.Translate("ERR_CON0"), Translations.Translate("PRR_ERR_FAT"), Translations.Translate("PRR_ERR_CON0"), Translations.Translate("ERR_CON1"));

                // Add conflicting mod name(s).
                modConflictBox.AddList(ModUtils.conflictingModNames.ToArray());

                // Closing para.
                modConflictBox.AddParas(Translations.Translate("PRR_ERR_CON1"));
            }

            // Don't do anything further if mod hasn't activated for whatever reason (mod conflict, harmony error, something else).
            if (!isModEnabled)
            {
                return;
            }

            // Report any 'soft' mod conflicts.
            if (softModConflct)
            {
                // Soft conflict detected - display warning notification for each one.
                foreach (string mod in ModUtils.conflictingModNames)
                {
                    if (mod.Equals("PTG") && ModSettings.dsaPTG == 0)
                    {
                        // Plop the Growables.
                        DontShowAgainMessageBox softConflictBox = MessageBoxBase.ShowModal<DontShowAgainMessageBox>();
                        softConflictBox.AddParas(Translations.Translate("PRR_CON_PTG0"), Translations.Translate("PRR_CON_PTG1"), Translations.Translate("PRR_CON_PTG2"));
                        softConflictBox.DSAButton.eventClicked += (component, clickEvent) => { ModSettings.dsaPTG = 1; SettingsUtils.SaveSettings(); };
                    }
                }
            }

            // Report any broken assets and remove from our prefab dictionary.
            foreach (BuildingInfo prefab in brokenPrefabs)
            {
                Logging.Error("broken prefab: ", prefab.name);
                xmlManager.prefabHash.Remove(prefab);
            }
            brokenPrefabs.Clear();

            // Init Ploppable Tool panel.
            PloppableTool.Initialize();

            // Add buttons to access building details from zoned building info panels.
            SettingsPanel.AddInfoPanelButtons();

            Logging.KeyMessage("loading complete");

            // Display update notification.
            WhatsNew.ShowWhatsNew();

            // Set up options panel event handler.
            OptionsPanel.OptionsEventHook();

        }
    }
}