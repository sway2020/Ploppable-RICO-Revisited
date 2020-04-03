using ICities;
using UnityEngine;
using ColossalFramework.UI;
using Harmony;


namespace PloppableRICO
{
    public class Loading : LoadingExtensionBase
    {
        const string HarmonyID = "com.github.algernon-A.csl.ploppablericorevisited";
        private HarmonyInstance _harmony = HarmonyInstance.Create(HarmonyID);

        public GameObject RICODataManager;
        public RICOPrefabManager xmlManager;

        private ConvertPrefabs convertPrefabs;


        public override void OnLevelLoaded(LoadMode mode)
        {
            Debug.Log("RICO Revisited v" + PloppableRICOMod.version + " loading.");

            base.OnLevelLoaded(mode);

            // Don't do anything if in asset editor.
            if (mode == LoadMode.NewAsset || mode == LoadMode.LoadAsset)
                return;

            // Check for original Ploppable RICO mod.
            if (Util.IsModEnabled(586012417ul))
            {
                // Original Ploppable RICO mod - log and show warning, then return without doing anything.
                Debug.Log("Original Ploppable RICO detected - RICO Revisited exiting.");
                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
                panel.SetMessage("RICO Revisited", Translations.GetTranslation("Original Ploppable RICO mod detected - RICO Revisited is shutting down to protect your game.  Only ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!"), false);
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

            // Deploy Harmony patches.
            _harmony.PatchAll(GetType().Assembly);
            Debug.Log("RICO Revisited: patching complete.");
        }

        public override void OnLevelUnloading()
        {
            // Unapply Harmony patches.
            _harmony.UnpatchAll(HarmonyID);
            Debug.Log("RICO Revisited: patches unapplied.");
        }
    }
}