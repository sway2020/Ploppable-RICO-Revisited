using System.Linq;
using ICities;
using ColossalFramework.Plugins;
using UnityEngine;

namespace PloppableRICO
{
    public class Loading : LoadingExtensionBase
	{
        private XMLManager xmlManager;
        private ConvertPrefabs convertPrefabs;
        private bool loaded;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
            return;

            loaded = true;

            //Load xml
            if (xmlManager == null)
            {
                xmlManager = new XMLManager();
                xmlManager.Run();
            }

            //Assign xml settings to prefabs.
            convertPrefabs = new ConvertPrefabs();
            convertPrefabs.run();

            //Init GUI
            PloppableTool.Initialize();
            RICOSettingsPanel.Initialize();

            //Deploy Detour
            Detour.BuildingToolDetour.Deploy();
        }

        public override void OnLevelUnloading ()
		{
			//base.OnLevelUnloading ();

            //RICO ploppables need a non private item class assigned to pass though the game reload. 
            if (loaded)
            {
                Util.AssignServiceClass();
            }
		}

		public override void OnReleased ()
		{
            //base.OnReleased ();
            if (loaded)
            {
                Detour.BuildingToolDetour.Revert();
                xmlManager = null;
            }
		}
	}
}
