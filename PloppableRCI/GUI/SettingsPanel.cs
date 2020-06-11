using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Settings panel manager static class.
    /// </summary>
    public static class SettingsPanel
    {
        // Instance references.
        private static GameObject uiGameObject;
        private static RICOSettingsPanel _panel;
        public static RICOSettingsPanel Panel => _panel;


        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        internal static void Open(BuildingInfo selected = null)
        {
            try
            {
                // If no instance already set, create one.
                if (uiGameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    uiGameObject = new GameObject("RICOSettingsPanel");
                    uiGameObject.transform.parent = UIView.GetAView().transform;

                    _panel = uiGameObject.AddComponent<RICOSettingsPanel>();

                    // Set up panel.
                    Panel.Setup();
                }

                // Select appropriate building if there's a preselection.
                if (selected != null)
                {
                    Panel.SelectBuilding(selected);
                }

                Panel.Show();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
        }


        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        internal static void Close()
        {
            GameObject.Destroy(_panel);
            GameObject.Destroy(uiGameObject);
        }


        /// <summary>
        /// Adds Ploppable RICO settings buttons to building info panels to directly access that building's RICO settings.
        /// </summary>
        internal static void AddInfoPanelButtons()
        {
            // Zoned building (PrivateBuilding) info panel.
            AddInfoPanelButton(UIView.library.Get<ZonedBuildingWorldInfoPanel>(typeof(ZonedBuildingWorldInfoPanel).Name));

            // Service building (PlayerBuilding) info panel.
            AddInfoPanelButton(UIView.library.Get<CityServiceWorldInfoPanel>(typeof(CityServiceWorldInfoPanel).Name));
        }


        /// <summary>
        /// Adds a Ploppable RICO button to a building info panel to directly access that building's RICO settings.
        /// The button will be added to the right of the panel with a small margin from the panel edge, at the relative Y position specified.
        /// </summary>
        /// <param name="infoPanel">Infopanel to apply the button to</param>
        /// <param name="relativeY">The relative Y position of the button within the panel</param>
        private static void AddInfoPanelButton(BuildingWorldInfoPanel infoPanel)
        {
            UIButton panelButton = infoPanel.component.AddUIComponent<UIButton>();

            // Basic button setup.
            panelButton.size = new Vector2(34, 34);
            panelButton.normalBgSprite = "ToolbarIconGroup6Normal";
            panelButton.normalFgSprite = "IconPolicyBigBusiness";
            panelButton.focusedBgSprite = "ToolbarIconGroup6Focused";
            panelButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
            panelButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
            panelButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
            panelButton.name = "PloppableButton";
            panelButton.tooltip = Translations.Translate("PRR_SET_RICO");

            // Find ProblemsPanel relative position to position button.
            // We'll use 40f as a default relative Y in case something doesn't work.
            UIComponent problemsPanel;
            float relativeY = 40f;

            // Player info panels have wrappers, zoned ones don't.
            UIComponent wrapper = infoPanel.Find("Wrapper");
            if (wrapper == null)
            {
                problemsPanel = infoPanel.Find("ProblemsPanel");
            }
            else
            {
                problemsPanel = wrapper.Find("ProblemsPanel");
            }

            try
            {
                // Position button vertically in the middle of the problems panel.  If wrapper panel exists, we need to add its offset as well.
                relativeY = (wrapper == null ? 0 : wrapper.relativePosition.y) + problemsPanel.relativePosition.y + ((problemsPanel.height - 34) / 2);
            }
            catch
            {
                // Don't really care; just use default relative Y.
                Debugging.Message("couldn't find ProblemsPanel relative position");
            }

            // Set position.
            panelButton.AlignTo(infoPanel.component, UIAlignAnchor.TopRight);
            panelButton.relativePosition += new Vector3(-5f, relativeY, 0f);

            // Event handler.
            panelButton.eventClick += (control, clickEvent) =>
            {
                // Select current building in the building details panel and show.
                Open(InstanceManager.GetPrefabInfo(WorldInfoPanel.GetCurrentInstanceID()) as BuildingInfo);

                // Manually unfocus control, otherwise it can stay focused until next UI event (looks untidy).
                control.Unfocus();
            };
        }
    }


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
        internal const float titleHeight = 40;

        // Panel components.
        private UITitleBar titleBar;
        private UIBuildingFilter filterBar;
        private UIFastList buildingSelection;
        private UIPreviewPanel previewPanel;
        private UISavePanel savePanel;
        private UIBuildingOptions buildingOptionsPanel;

        // Selected items.
        internal BuildingData currentSelection;


        /// <summary>
        /// Performs initial setup for the panel; we no longer use Start() as that's not sufficiently reliable (race conditions), and is no longer needed, with the new create/destroy process.
        /// </summary>
        public void Setup()
        {
            try
            {
                // Hide while we're setting up.
                isVisible = false;

                // Basic setup.
                canFocus = true;
                isInteractive = true;
                width = leftWidth + middleWidth + rightWidth + (spacing * 4);
                height = panelHeight + titleHeight + filterHeight + (spacing * 2) + bottomMargin;
                relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
                backgroundSprite = "UnlockingPanel2";

                // Titlebar.
                titleBar = AddUIComponent<UITitleBar>();
                titleBar.Setup();

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
                previewPanel.Setup();

                savePanel = middlePanel.AddUIComponent<UISavePanel>();
                savePanel.width = middlePanel.width;
                savePanel.height = (panelHeight - spacing) / 2;
                savePanel.relativePosition = new Vector3(0, previewPanel.height + spacing);
                savePanel.Setup();

                // Right panel - mod calculations.
                UIPanel rightPanel = AddUIComponent<UIPanel>();
                rightPanel.width = rightWidth;
                rightPanel.height = panelHeight;
                rightPanel.relativePosition = new Vector3(leftWidth + middleWidth + (spacing * 3), titleHeight + filterHeight + spacing);

                buildingOptionsPanel = rightPanel.AddUIComponent<UIBuildingOptions>();
                buildingOptionsPanel.width = rightWidth;
                buildingOptionsPanel.height = panelHeight;
                buildingOptionsPanel.relativePosition = Vector3.zero;
                buildingOptionsPanel.Setup();

                // Building selection list.
                buildingSelection = UIFastList.Create<UIBuildingRow>(leftPanel);
                buildingSelection.backgroundSprite = "UnlockingPanel";
                buildingSelection.width = leftPanel.width;
                buildingSelection.height = leftPanel.height;
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
        /// Called when the building selection changes to update other panels.
        /// </summary>
        /// <param name="building"></param>
        internal void UpdateSelectedBuilding(BuildingData building)
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
        internal void Save()
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
        internal void UpdateUICategory()
        {
            buildingOptionsPanel.UpdateUICategory();
        }


        /// <summary>
        /// Called to select a building from 'outside' the building details editor (e.g. by button on building info panel).
        /// Sets the filter to only display the relevant category for the relevant building, and makes that building selected in the list.
        /// </summary>
        /// <param name="building">The BuildingInfo record for this building.</param>
        public void SelectBuilding(BuildingInfo buildingInfo)
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
    }
}
