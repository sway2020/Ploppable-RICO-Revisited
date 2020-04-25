using ICities;
using UnityEngine;
using ColossalFramework.UI;
using Harmony;
using System.IO;


namespace PloppableRICO
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        // Harmony.
        const string HarmonyID = "com.github.algernon-A.csl.ploppablericorevisited";
        private HarmonyInstance _harmony;

        // Internal instances.
        public GameObject RICODataManager;
        private RICOPrefabManager xmlManager;
        private ConvertPrefabs convertPrefabs;

        // RICO definitions.
        public static PloppableRICODefinition localRicoDef;

        
        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading"></param>
        public override void OnCreated(ILoading loading)
        {
            Debug.Log("RICO Revisited v" + PloppableRICOMod.version + " loading.");

            // Deploy Harmony patches.
            _harmony = HarmonyInstance.Create(HarmonyID);
            _harmony.PatchAll(GetType().Assembly);
            Debug.Log("RICO Revisited: patching complete.");

            // Read local RICO settings (if they exist).
            string ricoDefPath = "LocalRICOSettings.xml";
            localRicoDef = null;

            if (!File.Exists(ricoDefPath))
            {
                Debug.Log("RICO Revisited: no " + ricoDefPath + " file found.");
            }
            else
            {
                localRicoDef = RICOReader.ParseRICODefinition("", ricoDefPath, insanityOK: true);

                if (localRicoDef == null)
                {
                    Debug.Log("RICO Revisited: no valid definitions in " + ricoDefPath);
                }
            }

            base.OnCreated(loading);
        }


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode"></param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Don't do anything if in asset editor.
            if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset)
                return;

            // Check for original Ploppable RICO mod.
            if (Util.IsModEnabled(586012417ul))
            {
                // Original Ploppable RICO mod detected - log and show warning, then return without doing anything.
                Debug.Log("Original Ploppable RICO detected - RICO Revisited exiting.");
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("RICO Revisited", Translations.GetTranslation("Original Ploppable RICO mod detected - RICO Revisited is shutting down to protect your game.  Only ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!"), true);
                return;
            }
            
            // Load xml only from main menu. 
            if (xmlManager == null)
            {
                xmlManager = new RICOPrefabManager();
                xmlManager.Run();
            }

            // Assign xml settings to prefabs.
            convertPrefabs = new ConvertPrefabs();
            convertPrefabs.run();

            // Init GUI.
            PloppableTool.Initialize();
            RICOSettingsPanel.Initialize();

            // Report any loading errors.
            Debugging.ReportErrors();

            Debug.Log("RICO Revisited: loading complete.");
        }

        /// <summary>
        /// Called by the game when exiting.
        /// </summary>
        public override void OnLevelUnloading()
        {
            // Unapply Harmony patches.
            _harmony.UnpatchAll(HarmonyID);
            Debug.Log("RICO Revisited: patches unapplied.");
        }
    }
}