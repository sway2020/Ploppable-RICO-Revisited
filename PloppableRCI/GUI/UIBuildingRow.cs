using UnityEngine;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// An individual row in the list of buildings.
    /// </summary>
    public class UIBuildingRow : UIPanel, UIFastListRow
    {
        // Height of each row.
        private const int rowHeight = 40;

        // Panel components.
        private UIPanel panelBackground;
        private UILabel buildingName;
        private BuildingData buildingData;
        private UISprite hasModSettings;
        private UISprite hasAuthorSettings;
        private UISprite hasLocalSettings;


        // Background for each list item.
        public UIPanel Background
        {
            get
            {
                if (panelBackground == null)
                {
                    panelBackground = AddUIComponent<UIPanel>();
                    panelBackground.width = width;
                    panelBackground.height = rowHeight;
                    panelBackground.relativePosition = Vector2.zero;

                    panelBackground.zOrder = 0;
                }

                return panelBackground;
            }
        }


        /// <summary>
        /// Called when dimensions are changed, including as part of initial setup (required to set correct relative position of label).
        /// </summary>
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (buildingName != null)
            {
                Background.width = width;
                buildingName.relativePosition = new Vector3(10f, 5f);

                // Settings checkboxes.
                hasModSettings.relativePosition = new Vector3(280f, 10f);
                hasAuthorSettings.relativePosition = new Vector3(310f, 10f);
                hasLocalSettings.relativePosition = new Vector3(340f, 10f);
            }
        }


        /// <summary>
        /// Mouse click event handler - updates the selected building to what was clicked.
        /// </summary>
        /// <param name="p">Mouse event parameter</param>
        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);
            SettingsPanel.Panel.UpdateSelectedBuilding(buildingData);
        }


        /// <summary>
        /// Generates and displays a building row.
        /// </summary>
        /// <param name="data">Object to list</param>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Display(object data, bool isRowOdd)
        {
            // Perform initial setup for new rows.
            if (buildingName == null)
            {
                isVisible = true;
                canFocus = true;
                isInteractive = true;
                width = parent.width;
                height = 40;

                buildingName = AddUIComponent<UILabel>();
                buildingName.width = 200;

                // Checkboxes to indicate which items have custom settings.
                hasModSettings = AddUIComponent<UISprite>();
                hasModSettings.size = new Vector2(20, 20);
                hasModSettings.relativePosition = new Vector3(340, 10);
                hasModSettings.tooltip = Translations.Translate("PRR_SET_HASMOD");

                hasAuthorSettings = AddUIComponent<UISprite>();
                hasAuthorSettings.size = new Vector2(20, 20);
                hasAuthorSettings.relativePosition = new Vector3(340, 10);
                hasAuthorSettings.tooltip = Translations.Translate("PRR_SET_HASAUT");

                hasLocalSettings = AddUIComponent<UISprite>();
                hasLocalSettings.size = new Vector2(20, 20);
                hasLocalSettings.relativePosition = new Vector3(340, 10);
                hasLocalSettings.tooltip = Translations.Translate("PRR_SET_HASLOC");
            }

            // Set selected building.
            buildingData = data as BuildingData;
            buildingName.text = buildingData.displayName;

            // Update custom settings checkboxes to correct state.
            if (buildingData.hasMod)
            {
                // Mod settings found.
                hasModSettings.spriteName = "AchievementCheckedTrue";
            }
            else
            {
                // No mod settings.
                hasModSettings.spriteName = "AchievementCheckedFalse";

            }

            if (buildingData.hasAuthor)
            {
                // Author settings found.
                hasAuthorSettings.spriteName = "AchievementCheckedTrue";
            }
            else
            {
                // No mod settings.
                hasAuthorSettings.spriteName = "AchievementCheckedFalse";

            }

            if (buildingData.hasLocal)
            {
                // Local settings found.
                hasLocalSettings.spriteName = "AchievementCheckedTrue";
            }
            else
            {
                // No mod settings.
                hasLocalSettings.spriteName = "AchievementCheckedFalse";

            }

            // Set initial background as deselected state.
            Deselect(isRowOdd);
        }


        /// <summary>
        /// Highlights the selected row.
        /// </summary>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Select(bool isRowOdd)
        {
            Background.backgroundSprite = "ListItemHighlight";
            Background.color = new Color32(255, 255, 255, 255);
        }


        /// <summary>
        /// Unhighlights the (un)selected row.
        /// </summary>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Deselect(bool isRowOdd)
        {
            if (isRowOdd)
            {
                // Lighter background for odd rows.
                Background.backgroundSprite = "UnlockingItemBackground";
                Background.color = new Color32(0, 0, 0, 128);
            }
            else
            {
                // Darker background for even rows.
                Background.backgroundSprite = null;
            }
        }
    }
}

