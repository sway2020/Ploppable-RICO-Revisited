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
        internal static bool isModEnabled = false;
        internal static bool harmonyLoaded = false;
        internal static bool patchOperating = false;

        // Used to flag if conflicting mods are running.
        internal static bool conflictingMod = false;
        internal static bool softModConflct;


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
                    localRicoDef = RICOReader.ParseRICODefinition(ricoDefPath, isLocal: true);

                    if (localRicoDef == null)
                    {
                        Logging.Message("no valid definitions in ", ricoDefPath);
                    }
                }
            }
        }

        // OnLevelLoaded has been replaced with Harmony Postfix.
    }
}