using ICities;
using UnityEngine;
using ColossalFramework.UI;
using Harmony;
using System.IO;


namespace PloppableRICO
{
    public class Loading : LoadingExtensionBase
    {
        const string HarmonyID = "com.github.algernon-A.csl.ploppablericorevisited";
        private HarmonyInstance _harmony;

        public GameObject RICODataManager;
        public RICOPrefabManager xmlManager;
        public static PloppableRICODefinition ricoDef;

        private ConvertPrefabs convertPrefabs;


        public override void OnCreated(ILoading loading)
        {
            string ricoDefPath = "LocalRICOSettings.xml";

            Debug.Log("RICO Revisited v" + PloppableRICOMod.version + " loading.");

            // Deploy Harmony patches.
            _harmony = HarmonyInstance.Create(HarmonyID);
            _harmony.PatchAll(GetType().Assembly);
            Debug.Log("RICO Revisited: patching complete.");

            // Read LocalRICOSettings.xml if it exists.
            ricoDef = null;
            if (!File.Exists(ricoDefPath))
            {
                Debug.Log("RICO Revisited: no " + ricoDefPath + " file found.");
            }
            else
            {
                ricoDef = RICOReader.ParseRICODefinition("", ricoDefPath, insanityOK: true);

                if (ricoDef == null)
                {
                    Debug.Log("RICO Revisited: no valid definitions in " + ricoDefPath);
                }
            }

            base.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            // CalculateHomeCount patch only needed while loading existing buildings; unapply patch now that everything is loaded.
            _harmony.Unpatch(typeof(ResidentialBuildingAI).GetMethod("CalculateHomeCount"), typeof(RICOHomeCount).GetMethod("Prefix"));

            base.OnLevelLoaded(mode);

            // Don't do anything if in asset editor.
            if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset)
                return;

            // Check for original Ploppable RICO mod.
            if (Util.IsModEnabled(586012417ul))
            {
                // Original Ploppable RICO mod - log and show warning, then return without doing anything.
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

        public override void OnLevelUnloading()
        {
            // Unapply Harmony patches.
            _harmony.UnpatchAll(HarmonyID);
            Debug.Log("RICO Revisited: patches unapplied.");
        }
    }
}