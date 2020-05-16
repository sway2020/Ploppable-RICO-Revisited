
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using ColossalFramework;
using UnityEngine;
using ColossalFramework.Math;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    ///This panel is in the middle column on the bottom. It contains the save and reset buttons, and will possibly contain more settings in the future. 
    /// </summary>

    public class UISavePanel : UIScrollablePanel
    {

        public BuildingData currentSelection;
        public UIButton save;
        public UIButton addLocal;
        public UIButton removeLocal;
        public UIButton reset;
        public UIButton apply;


        private static UISavePanel _instance;
        public static UISavePanel instance

        {
            get { return _instance; }
        }

        public override void Start()
        {
            base.Start();

            _instance = this;
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "UnlockingPanel";
            //padding = new RectOffset(5, 5, 5, 0);
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding.top = 5;
            autoLayoutPadding.left = 5;
            autoLayoutPadding.right = 5;
            builtinKeyNavigation = true;
            clipChildren = true;
            freeScroll = false;
            scrollWheelDirection = UIOrientation.Vertical;
            verticalScrollbar = new UIScrollbar();
            scrollWheelAmount = 10;
            verticalScrollbar.stepSize = 1f;
            verticalScrollbar.incrementAmount = 10f;
            SetupControls();
        }
        public void SelectionChanged(BuildingData buildingData)
        {
            currentSelection = buildingData;
        }

        private void SetupControls()
        {
            save = UIUtils.CreateButton(this);
            save.text = Translations.GetTranslation("Save");
            save.width = 140;

            addLocal = UIUtils.CreateButton(this);
            addLocal.text = Translations.GetTranslation("Add local");
            addLocal.width = 140;

            addLocal.eventClick += (c, p) =>
            {
                if (!currentSelection.hasLocal)
                {

                    currentSelection.local = new RICOBuilding();
                    currentSelection.hasLocal = true;

                    //If selected asset has author settings, copy those to local
                    if (currentSelection.hasAuthor)
                    {
                        currentSelection.local = (RICOBuilding)currentSelection.author.Clone();
                    }
                    else if (currentSelection.hasMod)
                    {
                        currentSelection.local = (RICOBuilding)currentSelection.mod.Clone();
                    }
                    else
                    {
                        // Set some basic settings for assets with no settings.
                        currentSelection.local.name = currentSelection.name;
                        currentSelection.local.ricoEnabled = true;
                        currentSelection.local.service = GetRICOService();
                        currentSelection.local.subService = GetRICOSubService();
                        currentSelection.local.level = (int)currentSelection.prefab.GetClassLevel() + 1;
                        currentSelection.local.constructionCost = 10;
                        currentSelection.local.homeCount = 10;

                        // UI Category will be updated later.
                        currentSelection.local.uiCategory = "none";
                    }

                    currentSelection.local.name = currentSelection.name;
                    //currentSelection.local = (PloppableRICODefinition.Building)newlocal.Clone();

                    // Update settings panel with new settings if RICO is enabled for this building.
                    if (enabled)
                    {
                        RICOSettingsPanel.instance.UpdateSelectedBuilding(currentSelection);
                        RICOSettingsPanel.instance.UpdateSelection();

                        // Update UI category.
                        RICOSettingsPanel.instance.m_buildingOptions.UpdateUICategory();
                    }
                    Save();
                }
            };

            removeLocal = UIUtils.CreateButton(this);
            removeLocal.eventClick += (c, p) =>
            {
                // If there are no other settings, destroy existing building button.
                if (!(currentSelection.hasAuthor || currentSelection.hasMod))
                {
                    PloppableTool.instance.DestroyBuildingButton(currentSelection.prefab.name, currentSelection.uiCategory);
                }

                currentSelection.local = null;
                currentSelection.hasLocal = false;

                RICOSettingsPanel.instance.UpdateSelectedBuilding(currentSelection);

                if (enabled) RICOSettingsPanel.instance.UpdateSelection();
                Save();

            };
            removeLocal.text = Translations.GetTranslation("Remove local");
            removeLocal.width = 140;

            save.eventClick += (c, p) =>
            {
                Save();
            };

            // Apply changes button and warning label.
            UILabel warningLabel = this.AddUIComponent<UILabel>();
            warningLabel.textAlignment = UIHorizontalAlignment.Center;
            warningLabel.autoSize = false;
            warningLabel.autoHeight = true;
            warningLabel.width = this.width - autoLayoutPadding.left - autoLayoutPadding.right;
            warningLabel.text = Translations.GetTranslation("\r\nCAUTION: EXPERIMENTAL");

            apply = UIUtils.CreateButton(this);
            apply.text = Translations.GetTranslation("Save and apply changes");
            apply.width = this.width - autoLayoutPadding.left - autoLayoutPadding.right;
            apply.eventClick += (c, p) =>
            {
                // Find current prefab instance.
                BuildingData currentBuildingData = Loading.xmlManager.prefabHash[currentSelection.prefab];

                // Delete existing building button.
                PloppableTool.instance.DestroyBuildingButton(currentBuildingData.prefab.name, currentSelection.uiCategory);

                // Save first.
                Save();

                // If we're converting a residential building to something else, then we first should clear out all households.
                if (currentBuildingData.prefab.GetService() == ItemClass.Service.Residential && !IsCurrentResidential())
                {
                    // removeAll argument to true to remove all households.
                    UpdateHouseholds(currentBuildingData.prefab.name, removeAll: true);
                }

                // Get the currently applied RICO settings (local, author, mod).
                RICOBuilding currentData = CurrentRICOSetting();

                if (currentData != null)
                {
                    // Convert the 'live' prefab (instance in PrefabCollection) and update household count and builidng level for all current instances.
                    Loading.convertPrefabs.ConvertPrefab(currentData, PrefabCollection<BuildingInfo>.FindLoaded(currentBuildingData.prefab.name));
                    UpdateHouseholds(currentBuildingData.prefab.name, currentData.level);

                    // Create new building button.
                    PloppableTool.instance.AddBuildingButton(currentBuildingData, CurrentUICategory());
                }
                else
                {
                    Debug.Log("RICO Revisited: no current RICO settings to apply to prefab '" + currentBuildingData + "'.");
                }
            };
        }

        public void Save()
        {

            RICOSettingsPanel.instance.Save();

            if (!File.Exists("LocalRICOSettings.xml"))
            {

                var newlocalSettings = new PloppableRICODefinition();
                var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition));

                using (XmlWriter writer = XmlWriter.Create("LocalRICOSettings.xml"))
                {
                    xmlSerializer.Serialize(writer, newlocalSettings);
                }
            }

            if (File.Exists("LocalRICOSettings.xml"))
            {
                PloppableRICODefinition localSettings;
                var newlocalSettings = new PloppableRICODefinition();

                var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition));

                using (StreamReader streamReader = new System.IO.StreamReader("LocalRICOSettings.xml"))
                {
                    localSettings = xmlSerializer.Deserialize(streamReader) as PloppableRICODefinition;
                }

                //Loop though all buildings in the file. If they arent the current selection, write them back to file. 
                foreach (var buildingDef in localSettings.Buildings)
                {
                    if (buildingDef.name != currentSelection.name)
                    {
                        newlocalSettings.Buildings.Add(buildingDef);
                    }
                }

                //If current selection has local settings, add them to file. 
                if (currentSelection.hasLocal)
                {
                    newlocalSettings.Buildings.Add(currentSelection.local);
                }

                using (TextWriter writer = new StreamWriter("LocalRICOSettings.xml"))
                {
                    xmlSerializer.Serialize(writer, newlocalSettings);
                }

            }

            // Force an update of the settings panel with current values.
            RICOSettingsPanel.instance.m_buildingOptions.SelectionChanged(currentSelection);
        }

        private string GetRICOService()
        {
            switch (currentSelection.prefab.m_class.m_service)
            {
                case ItemClass.Service.Commercial:
                    return "commercial";
                case ItemClass.Service.Industrial:
                    return "industrial";
                case ItemClass.Service.Office:
                    return "office";
                default:
                    return "residential";
            }
        }


        private string GetRICOSubService()
        {
            switch (currentSelection.prefab.m_class.m_subService)
            {
                case ItemClass.SubService.CommercialLow:
                    return "low";
                case ItemClass.SubService.CommercialHigh:
                    return "high";
                case ItemClass.SubService.CommercialTourist:
                    return "tourist";
                case ItemClass.SubService.CommercialLeisure:
                    return "leisure";
                case ItemClass.SubService.CommercialEco:
                    return "eco";
                case ItemClass.SubService.IndustrialGeneric:
                    return "generic";
                case ItemClass.SubService.IndustrialFarming:
                    return "farming";
                case ItemClass.SubService.IndustrialForestry:
                    return "forest";
                case ItemClass.SubService.IndustrialOil:
                    return "oil";
                case ItemClass.SubService.IndustrialOre:
                    return "ore";
                case ItemClass.SubService.OfficeGeneric:
                    return "none";
                case ItemClass.SubService.OfficeHightech:
                    return "high tech";
                case ItemClass.SubService.ResidentialLowEco:
                    return "low eco";
                case ItemClass.SubService.ResidentialHighEco:
                    return "high eco";
                case ItemClass.SubService.ResidentialLow:
                    return "low";
                default:
                    return "high";
            }
        }


        /// <summary>
        /// Updates household counts for all buildings in scene with the given prefab name.
        /// Can also remove all housholds, setting the total to zero.
        /// </summary>
        /// <param name="prefabName">Prefab name</param>
        /// <param name="removeAll">If true, all households will be removed (count set to 0)</param>
        private void UpdateHouseholds(string prefabName, int level = 0, bool removeAll = false)
        {
            int homeCount = 0;
            int visitCount = 0;
            int homeCountChanged = 0;

            // Get building manager instance.
            var instance = Singleton<BuildingManager>.instance;


            // Iterate through each building in the scene.
            for (ushort i = 0; i < instance.m_buildings.m_buffer.Length; i++)
            {
                // Check for matching name.
                if (instance.m_buildings.m_buffer[i].Info != null && instance.m_buildings.m_buffer[i].Info.name != null && instance.m_buildings.m_buffer[i].Info.name.Equals(prefabName))
                {
                    // Got a match!  Check level if applicable.
                    if (level > 0)
                    {
                        // m_level is one less than building.level.
                        byte newLevel = (byte)(level - 1);

                        if (instance.m_buildings.m_buffer[i].m_level != newLevel)
                        {
                            Debug.Log("RICO Revisited: Found building '" + prefabName + "' with level " + (instance.m_buildings.m_buffer[i].m_level + 1) + ", overriding to level " + level + ".");
                            instance.m_buildings.m_buffer[i].m_level = newLevel;
                        }
                    }

                    // Update homecounts for any residential buildings.
                    PrivateBuildingAI thisAI = instance.m_buildings.m_buffer[i].Info.GetAI() as ResidentialBuildingAI;
                    if (thisAI != null)
                    {
                        // This is residential! If we're not removing all households, recalculate home and visit counts using AI method.
                        if (!removeAll)
                        {
                            homeCount = thisAI.CalculateHomeCount((ItemClass.Level)instance.m_buildings.m_buffer[i].m_level, new Randomizer(i), instance.m_buildings.m_buffer[i].Width, instance.m_buildings.m_buffer[i].Length);
                            visitCount = thisAI.CalculateVisitplaceCount((ItemClass.Level)instance.m_buildings.m_buffer[i].m_level, new Randomizer(i), instance.m_buildings.m_buffer[i].Width, instance.m_buildings.m_buffer[i].Length);
                        }

                        // Apply changes via direct call to EnsureCitizenUnits prefix patch from this mod and increment counter.
                        RealisticCitizenUnits.EnsureCitizenUnits(ref thisAI, i, ref instance.m_buildings.m_buffer[i], homeCount, 0, visitCount, 0);
                        homeCountChanged++;
                    }

                    // Clear any problems.
                    instance.m_buildings.m_buffer[i].m_problems = 0;
                }
            }

            Debug.Log("RICO Revisited: set household counts to " + homeCount + " for " + homeCountChanged + " '" + prefabName + "' buildings.");
        }



        /// <summary>
        /// Checks to see if the currently applied RICO service for the selected building is residential.
        /// </summary>
        /// <returns>True if residential, otherwise false.</returns>
        private bool IsCurrentResidential()
        {
            if (currentSelection.hasLocal)
            {
                return currentSelection.local.service == "residential";
            }
            else if (currentSelection.hasAuthor)
            {
                return currentSelection.author.service == "residential";
            }
            else if (currentSelection.hasMod)
            {
                return currentSelection.mod.service == "residential";
            }

            return false;
        }



        /// <summary>
        /// Returns the currently applied RICO UI category for the selected building.
        /// </summary>
        /// <returns>True if residential, otherwise false.</returns>
        private string CurrentUICategory()
        {
            if (currentSelection.hasLocal)
            {
                return currentSelection.local.uiCategory;
            }
            else if (currentSelection.hasAuthor)
            {
                return currentSelection.author.uiCategory;
            }
            else if (currentSelection.hasMod)
            {
                return currentSelection.mod.uiCategory;
            }

            return "none";
        }


        /// <summary>
        /// Returns the currently applied RICO settings for the selected building.
        /// </summary>
        /// <returns></returns>
        private RICOBuilding CurrentRICOSetting()
        {
            if (currentSelection.hasLocal)
            {
                return currentSelection.local;
            }
            else if (currentSelection.hasAuthor)
            {
                return currentSelection.author;
            }
            else if (currentSelection.hasMod)
            {
                return currentSelection.mod;
            }

            return null;
        }
    }
}