using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PloppableRICO
{
    /// <summary>
    /// Base class of the RICO settings panel.  Based (via AJ3D's Ploppable RICO) ultimately on SamsamTS's Building Themes panel; many thanks to him for his work.
    /// </summary>
    public class RICOSettingsPanel : UIPanel
    {

        // Constants.
        private const float leftWidth = 400;
        private const float middleWidth = 250;
        private const float rightWidth = 280;
        private const float filterHeight = 40;
        private const float panelHeight = 550;
        private const float bottomMargin = 10;
        private const float spacing = 5;
        private const float checkFilterHeight = 30;
        public const float titleHeight = 40;

        // Panel components.
        private UITitleBar titleBar;
        private UIBuildingFilter filterBar;
        private UIFastList buildingSelection;
        private UIPreviewPanel previewPanel;
        private UISavePanel savePanel;
        private UIBuildingOptions buildingOptionsPanel;

        // Selected items.
        public BuildingData currentSelection;

        // Instance references.
        private static GameObject uiGameObject;
        private static RICOSettingsPanel _instance;
        public static RICOSettingsPanel instance => _instance;


        /// <summary>
        /// Creates the panel object in-game.
        /// </summary>
        public static void Create()
        {
            try
            {
                // Destroy existing (if any) instances.
                uiGameObject = GameObject.Find("RICOSettingsPanel");
                if (uiGameObject != null)
                {
                    UnityEngine.Debug.Log("Ploppable RICO Revisited: destroying existing building details panel instance.");
                    GameObject.Destroy(uiGameObject);
                }

                // Create new instance.
                // Give it a unique name for easy finding with ModTools.
                uiGameObject = new GameObject("RICOSettingsPanel");
                uiGameObject.transform.parent = UIView.GetAView().transform;
                _instance = uiGameObject.AddComponent<RICOSettingsPanel>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        /// <summary>
        /// Create the RICO settings panel; called by Unity just before any of the Update methods is called for the first time.
        /// </summary>
        public override void Start()
        {
            base.Start();

            try
            {
                // Basic setup.
                isVisible = false;
                canFocus = true;
                isInteractive = true;
                width = leftWidth + middleWidth + rightWidth + (spacing * 4);
                height = panelHeight + titleHeight + filterHeight + (spacing * 2) + bottomMargin;
                relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
                backgroundSprite = "UnlockingPanel2";

                // Titlebar.
                titleBar = AddUIComponent<UITitleBar>();

                // Filter.
                filterBar = AddUIComponent<UIBuildingFilter>();
                filterBar.width = width - (spacing * 2);
                filterBar.height = filterHeight;
                filterBar.relativePosition = new Vector3(spacing, titleHeight);

                // Event handler to dealth with changes to filtering.
                filterBar.eventFilteringChanged += (component, value) =>
                {
                    if (value == -1) return;

                    int listCount = buildingSelection.rowsData.m_size;
                    float position = buildingSelection.listPosition;

                    buildingSelection.selectedIndex = -1;

                    buildingSelection.rowsData = GenerateFastList();
                };

                // Set up panels.
                // Left panel - list of buildings.
                UIPanel leftPanel = AddUIComponent<UIPanel>();
                leftPanel.width = leftWidth;
                leftPanel.height = panelHeight - checkFilterHeight;
                leftPanel.relativePosition = new Vector3(spacing, titleHeight + filterHeight + checkFilterHeight + spacing);

                // Middle panel - building preview and edit panels.
                UIPanel middlePanel = AddUIComponent<UIPanel>();
                middlePanel.width = middleWidth;
                middlePanel.height = panelHeight;
                middlePanel.relativePosition = new Vector3(leftWidth + (spacing * 2), titleHeight + filterHeight + spacing);

                previewPanel = middlePanel.AddUIComponent<UIPreviewPanel>();
                previewPanel.width = middlePanel.width;
                previewPanel.height = (panelHeight - spacing) / 2;
                previewPanel.relativePosition = Vector3.zero;

                savePanel = middlePanel.AddUIComponent<UISavePanel>();
                savePanel.width = middlePanel.width;
                savePanel.height = (panelHeight - spacing) / 2;
                savePanel.relativePosition = new Vector3(0, previewPanel.height + spacing);

                // Right panel - mod calculations.
                UIPanel rightPanel = AddUIComponent<UIPanel>();
                rightPanel.width = rightWidth;
                rightPanel.height = panelHeight;
                rightPanel.relativePosition = new Vector3(leftWidth + middleWidth + (spacing * 3), titleHeight + filterHeight + spacing);

                buildingOptionsPanel = rightPanel.AddUIComponent<UIBuildingOptions>();
                buildingOptionsPanel.width = rightWidth;
                buildingOptionsPanel.height = panelHeight;
                buildingOptionsPanel.relativePosition = Vector3.zero;

                // Building selection list.
                buildingSelection = UIFastList.Create<UIBuildingRow>(leftPanel);
                buildingSelection.backgroundSprite = "UnlockingPanel";
                buildingSelection.width = leftPanel.width;
                buildingSelection.height = leftPanel.height - checkFilterHeight;
                buildingSelection.canSelect = true;
                buildingSelection.rowHeight = 40;
                buildingSelection.autoHideScrollbar = true;
                buildingSelection.relativePosition = Vector3.zero;
                buildingSelection.rowsData = new FastList<object>();
                buildingSelection.selectedIndex = -1;

                // Set up filterBar to make sure selection filters are properly initialised before calling GenerateFastList.
                filterBar.Setup();

                // Populate the list.
                buildingSelection.rowsData = GenerateFastList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        /// <summary>
        /// Shows/hides the building details screen.
        /// </summary>
        public void Toggle()
        {
            if (isVisible)
                Hide();
            else
                Show(true);
        }


        /// <summary>
        /// Called when the building selection changes to update other panels.
        /// </summary>
        /// <param name="building"></param>
        public void UpdateSelectedBuilding(BuildingData building)
        {
            if (building != null)
            {
                // Update sub-panels.
                currentSelection = Loading.xmlManager.prefabHash[building.prefab];

                buildingOptionsPanel.SelectionChanged(currentSelection);
                savePanel.SelectionChanged(currentSelection);
                previewPanel.Show(currentSelection);
            }
        }


        /// <summary>
        /// Called to save building data.
        /// </summary>
        public void Save()
        {
            buildingOptionsPanel.SaveRICO();
        }


        /// <summary>
        /// Refreshes the building selection list.
        /// </summary>
        public void UpdateSelection()
        {
            // Refresh the building list.
            buildingSelection.Refresh();
        }


        /// <summary>
        /// Updates the UI Category of the building in the options panel.
        /// </summary>
        public void UpdateUICategory()
        {
            buildingOptionsPanel.UpdateUICategory();
        }

        /// <summary>
        /// Called to select a building from 'outside' the building details editor (e.g. by button on building info panel).
        /// Sets the filter to only display the relevant category for the relevant building, and makes that building selected in the list.
        /// </summary>
        /// <param name="building">The BuildingInfo record for this building.</param>
        private void SelectBuilding(BuildingInfo buildingInfo)
        {
            // Get the RICO BuildingData associated with this prefab.
            BuildingData building = Loading.xmlManager.prefabHash[buildingInfo];

            // Ensure the fastlist is filtered to include this building category only.
            filterBar.SelectBuildingCategory(building.category);
            buildingSelection.rowsData = GenerateFastList();

            // Clear the name filter.
            filterBar.nameFilter.text = String.Empty;

            // Find and select the building in the fastlist.
            buildingSelection.FindBuilding(building.name);

            // Update the selected building to the current.
            UpdateSelectedBuilding(building);
        }


        /// <summary>
        /// Generates the list of buildings depending on current filter settings.
        /// </summary>
        /// <returns></returns>
        private FastList<object> GenerateFastList()
        {
            // List to store all building prefabs that pass the filter.
            List<BuildingData> filteredList = new List<BuildingData>();

            // Iterate through all loaded building prefabs and add them to the list if they meet the filter conditions.
            foreach (BuildingData item in Loading.xmlManager.prefabHash.Values)
            {
                // Skip any null or invalid prefabs.
                if ((item == null) || (item.prefab == null))
                {
                    continue;
                }

                // Filter by zoning category.
                if (!filterBar.IsAllZoneSelected())
                {
                    Category category = item.category;
                    if (category == Category.None || !filterBar.IsZoneSelected(category))
                    {
                        continue;
                    }
                }

                // Filter by settings.
                if (filterBar.settingsFilter[0].isChecked && !item.hasMod) continue;
                if (filterBar.settingsFilter[1].isChecked && !item.hasAuthor) continue;
                if (filterBar.settingsFilter[2].isChecked && !item.hasLocal) continue;
                if (filterBar.settingsFilter[3].isChecked && !(item.hasMod || item.hasAuthor || item.hasLocal)) continue;

                // Filter by name.
                if (!filterBar.buildingName.IsNullOrWhiteSpace() && !item.name.ToLower().Contains(filterBar.buildingName.ToLower()))
                {
                    continue;
                }

                // Finally!  We've got an item that's passed all filters; add it to the list.
                filteredList.Add(item);
            }

            // Create return list with our filtered list, sorted alphabetically.
            FastList<object> fastList = new FastList<object>();
            fastList.m_buffer = filteredList.OrderBy(x => x.displayName).ToArray();
            fastList.m_size = filteredList.Count;

            return fastList;
        }


        /// <summary>
        /// Adds a Ploppable RICO button to a building info panel to directly access that building's RICO settings.
        /// The button will be added to the right of the panel with a small margin from the panel edge, at the relative Y position specified.
        /// </summary>
        /// <param name="infoPanel">Infopanel to apply the button to</param>
        /// <param name="relativeY">The relative Y position of the button within the panel</param>
        private void AddInfoPanelButton(BuildingWorldInfoPanel infoPanel, float relativeY)
        {
            // Button instance.
            UIButton panelButton = infoPanel.component.AddUIComponent<UIButton>();

            // Basic button setup.
            panelButton.size = new Vector2(34, 34);
            panelButton.normalBgSprite = "ToolbarIconGroup6Normal";
            panelButton.normalFgSprite = "IconPolicyBigBusiness";
            panelButton.focusedBgSprite = "ToolbarIconGroup6Focused";
            panelButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
            panelButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
            panelButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
            panelButton.relativePosition = new Vector2(infoPanel.component.width - panelButton.width - 5f, relativeY);
            panelButton.name = "PloppableButton";
            panelButton.tooltip = Translations.GetTranslation("RICO Settings");

            // Event handler.
            panelButton.eventClick += (c, p) =>
            {
                // Select current building in the building details panel and show.
                SelectBuilding(InstanceManager.GetPrefabInfo(WorldInfoPanel.GetCurrentInstanceID()) as BuildingInfo);
                Show();
            };
        }


        /// <summary>
        /// Adds Ploppable RICO settings buttons to building info panels to directly access that building's RICO settings.
        /// </summary>
        public void AddInfoPanelButtons()
        {
            // Zoned building (PrivateBuilding) info panel.
            AddInfoPanelButton(UIView.library.Get<ZonedBuildingWorldInfoPanel>(typeof(ZonedBuildingWorldInfoPanel).Name), 40f);

            // Service building (PlayerBuilding) info panel.
            AddInfoPanelButton(UIView.library.Get<CityServiceWorldInfoPanel>(typeof(CityServiceWorldInfoPanel).Name), 77f);
        }
    }
}
