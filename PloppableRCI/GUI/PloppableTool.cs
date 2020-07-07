using System;
using System.Linq;
using System.Text;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.Math;
using System.Net.NetworkInformation;

namespace PloppableRICO
{
    /// <summary>
    /// This class draws the RICO panel, populates it with building buttons, and activates the building tool when buttons are clicked. 
    /// </summary>
    /// 
    public class PloppableTool : ToolBase
    {
        // Number of UI categories.
        private const int NumTypes = 14;
        // Number of UI tabs: +1 to account for 'Settings' tab.
        private const int NumTabs = NumTypes + 1;


        // Object instances.
        private static GameObject _gameObject;
        private static PloppableTool _instance;

        public static PloppableTool Instance => _instance;

        // UI components.
        private UIButton PloppableButton;

        private UITabstrip Tabs;
        private UISprite[] TabSprites = new UISprite[NumTabs];
        private UIButton[] TabButtons = new UIButton[NumTabs];
        private UIButton showSettings;

        private UIPanel BuildingPanel;
        private UIScrollablePanel[] BuildingPanels = new UIScrollablePanel[NumTabs];
        private UIScrollablePanel currentSelection = new UIScrollablePanel();

        private UIButton LeftButton = new UIButton();
        private UIButton RightButton = new UIButton();


        // Names used to identify icons for tabs (specific game icon names - not just made up).
        private string[] Names = new string[]
        {
            "ResidentialLow",
            "ResidentialHigh",
            "CommercialLow",
            "CommercialHigh",
            "Office",
            "Industrial",
            "Farming",
            "Forest",
            "Oil",
            "Ore",
            "Leisure",
            "Tourist",
            "Organic",
            "Hightech",
            "Selfsufficient"
        };


        /// <summary>
        /// Initializes the Ploppable Tool (including panel).
        /// </summary>
        internal static void Initialize()
        {
            // Don't do anything if we're already setup.
            if (_instance == null)
            {
                try
                {
                    // Creating our own gameObect helps finding the UI in ModTools.
                    _gameObject = new GameObject("PloppableTool");
                    _gameObject.transform.parent = UIView.GetAView().transform;
                    _instance = _gameObject.AddComponent<PloppableTool>();
                    _instance.DrawPloppablePanel();
                    _instance.PopulateButtons();

                    // Deactivate to start with.
                    if (ModSettings.speedBoost)
                    {
                        _gameObject.SetActive(false);
                    }
                }
                catch (Exception e)
                {
                    Debugging.LogException(e);
                }
            }
        }
        

        /// <summary>
        /// Awaken the Kraken! Or PloppableTool tool controller, whatever.
        /// </summary>
        protected override void Awake()
        {
            this.m_toolController = ToolsModifierControl.toolController;
        }
        

        /// <summary>
        /// Destroys the Ploppable Tool GameObject.
        /// </summary>
        public static void Destroy()
        {
            try
            {
                if (_gameObject != null)
                    GameObject.Destroy(_gameObject);
            }
            catch (Exception e)
            {
                Debugging.LogException(e);
            }
        }


        /// <summary>
        /// Draws the Ploppable Tool panel.
        /// </summary>
        private void DrawPloppablePanel()
        {
            // Check to make sure that we haven't already done this.
            if (PloppableButton == null)
            {
                // Main button on ingame toolbar.
                PloppableButton = UIView.GetAView().FindUIComponent<UITabstrip>("MainToolstrip").AddUIComponent<UIButton>();
                PloppableButton.size = new Vector2(43, 49);
                PloppableButton.normalBgSprite = "ToolbarIconGroup6Normal";
                PloppableButton.normalFgSprite = "IconPolicyBigBusiness";
                PloppableButton.focusedBgSprite = "ToolbarIconGroup6Focused";
                PloppableButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
                PloppableButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
                PloppableButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
                PloppableButton.relativePosition = new Vector2(800, 0);
                PloppableButton.name = "PloppableButton";
                PloppableButton.tooltip = Translations.Translate("PRR_NAME");

                // Event handler - show the Ploppable Tool panel when the button is clicked.
                PloppableButton.eventClick += (component, clickEvent) =>
                {
                    component.Focus();
                    BuildingPanel.isVisible = true;
                };

                // Base panel.
                BuildingPanel = UIView.GetAView().FindUIComponent("TSContainer").AddUIComponent<UIPanel>();
                BuildingPanel.backgroundSprite = "SubcategoriesPanel";
                BuildingPanel.isVisible = false;
                BuildingPanel.name = "PloppableBuildingPanel";
                BuildingPanel.size = new Vector2(859, 109);
                BuildingPanel.relativePosition = new Vector2(0, 0);

                // Tabstrip.
                Tabs = UIView.GetAView().FindUIComponent("PloppableBuildingPanel").AddUIComponent<UITabstrip>();
                Tabs.size = new Vector2(832, 25);
                Tabs.relativePosition = new Vector2(13, -25);
                Tabs.pivot = UIPivotPoint.BottomCenter;
                Tabs.padding = new RectOffset(0, 3, 0, 0);

                // Get game sprite thumbnail atlas.
                UITextureAtlas gameIconAtlas = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Thumbnails");

                // Scrollable panels.
                for (int i = 0; i <= NumTypes; i++)
                {
                    // Basic setup.
                    BuildingPanels[i] = new UIScrollablePanel();
                    BuildingPanels[i] = BuildingPanel.AddUIComponent<UIScrollablePanel>();
                    BuildingPanels[i].size = new Vector2(763, 109);
                    BuildingPanels[i].relativePosition = new Vector2(50, 0);
                    BuildingPanels[i].name = Names[i] + "Panel";
                    BuildingPanels[i].isVisible = false;
                    BuildingPanels[i].autoLayout = true;
                    BuildingPanels[i].autoLayoutStart = LayoutStart.BottomLeft;
                    BuildingPanels[i].builtinKeyNavigation = true;
                    BuildingPanels[i].autoLayoutDirection = LayoutDirection.Horizontal;
                    BuildingPanels[i].clipChildren = true;
                    BuildingPanels[i].freeScroll = false;
                    BuildingPanels[i].horizontalScrollbar = new UIScrollbar();
                    BuildingPanels[i].scrollWheelAmount = 109;
                    BuildingPanels[i].horizontalScrollbar.stepSize = 1f;
                    BuildingPanels[i].horizontalScrollbar.incrementAmount = 109f;
                    BuildingPanels[i].scrollWithArrowKeys = true;

                    // Draw tabs in tabstrip.
                    TabButtons[i] = new UIButton(); 
                    TabButtons[i] = Tabs.AddUIComponent<UIButton>();
                    TabButtons[i].size = new Vector2(46, 25);
                    TabButtons[i].normalBgSprite = "SubBarButtonBase";
                    TabButtons[i].disabledBgSprite = "SubBarButtonBaseDisabled";
                    TabButtons[i].pressedBgSprite = "SubBarButtonBasePressed";
                    TabButtons[i].hoveredBgSprite = "SubBarButtonBaseHovered";
                    TabButtons[i].focusedBgSprite = "SubBarButtonBaseFocused";
                    TabButtons[i].state = UIButton.ButtonState.Normal;
                    TabButtons[i].name = Names[i] + "Button";
                    TabButtons[i].tabStrip = true;

                    TabSprites[i] = new UISprite();
                    TabSprites[i] = TabButtons[i].AddUIComponent<UISprite>();

                    // Standard "Vanilla" categories (low and high residential, low and high commercial, and offices) - use standard zoning icons from original vanilla release.
                    if (i <= 5)
                    {
                        TabSprites[i].atlas = gameIconAtlas;
                        SetTabSprite(TabSprites[i], "Zoning" + Names[i]);
                    }
                    else
                    {
                        // Other types don't have standard zoning icons; use policy icons instead.
                        SetTabSprite(TabSprites[i], "IconPolicy" + Names[i]);
                    }
                }
                
                // 'Left' and 'Right' buttons to croll panel.
                LeftButton = BuildingPanel.AddUIComponent<UIButton>();
                RightButton = BuildingPanel.AddUIComponent<UIButton>();

                LeftButton.size = new Vector2(32, 32);
                RightButton.size = new Vector2(32, 32);

                LeftButton.normalBgSprite = "ArrowLeft";
                LeftButton.pressedBgSprite = "ArrowLeftPressed";
                LeftButton.hoveredBgSprite = "ArrowLeftHovered";
                LeftButton.disabledBgSprite = "ArrowLeftDisabled";

                LeftButton.relativePosition = new Vector3(16, 33);
                RightButton.relativePosition = new Vector3(812, 33);

                RightButton.normalBgSprite = "ArrowRight";
                RightButton.pressedBgSprite = "ArrowRightPressed";
                RightButton.hoveredBgSprite = "ArrowRightHovered";
                RightButton.disabledBgSprite = "ArrowRightDisabled";

                // Initialise current selection to first panel.
                currentSelection = BuildingPanels[0];

                // Event handlers.
                RightButton.eventClick += (component, clickEvent) => ArrowClicked(component);
                LeftButton.eventClick += (component, clickEvent) => ArrowClicked(component);

                // Show left/right scroll buttons if we've got more than seven buttons, otherwise hide.
                if (BuildingPanels[0].childCount > 7)
                {
                    LeftButton.isVisible = true;
                    RightButton.isVisible = true;
                }
                else
                {
                    LeftButton.isVisible = false;
                    RightButton.isVisible = false;
                }

                // This can't happen in a loop, because the loop index is undefined after setup has occured (i.e. when the function is actually called).
                TabButtons[0].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[0], TabSprites[0]);
                TabButtons[1].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[1], TabSprites[1]);
                TabButtons[2].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[2], TabSprites[2]);
                TabButtons[3].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[3], TabSprites[3]);
                TabButtons[4].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[4], TabSprites[4]);
                TabButtons[5].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[5], TabSprites[5]);
                TabButtons[6].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[6], TabSprites[6]);
                TabButtons[7].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[7], TabSprites[7]);
                TabButtons[8].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[8], TabSprites[8]);
                TabButtons[9].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[9], TabSprites[9]);
                // Below are DLC categories - AD for first two, then GC for next 3.  Will be hidden if relevant DLC is not installed.
                TabButtons[10].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[10], TabSprites[10]);
                TabButtons[11].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[11], TabSprites[11]);
                TabButtons[12].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[12], TabSprites[12]);
                TabButtons[13].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[13], TabSprites[13]);
                TabButtons[14].eventClick += (component, clickEvent) => TabClicked(BuildingPanels[14], TabSprites[14]);

                // Activate low residential panel to start with (what the user sees on first opening the panel).
                BuildingPanels[0].isVisible = true;

                // Hide AD tabs if AD is not installed.
                if (!Util.isADinstalled())
                {
                    TabButtons[10].isVisible = false;
                    TabButtons[11].isVisible = false;
                }

                // Hide GC tabs if GC is not installed.
                if (!Util.isGCinstalled())
                {
                    TabButtons[12].isVisible = false;
                    TabButtons[13].isVisible = false;
                    TabButtons[14].isVisible = false;
                }

                // Settings tab.
                showSettings = UIUtils.CreateButton(Tabs);
                showSettings.size = new Vector2(80, 25);
                showSettings.normalBgSprite = "SubBarButtonBase";
                showSettings.eventClick += (component, clickEvent) => SettingsPanel.Open();

                // Add UI text.
                SetText();

                // Toggle active state on visibility changed if we're using the UI speed boost (deactivating when hidden to minimise UI workload and impact on performance).
                BuildingPanel.eventVisibilityChanged += (component, isVisible) =>
                {
                    // Additional check to allow for the case where speedboost has been deactivated mid-game while the panel was deactivated.
                    if ((ModSettings.speedBoost) || (isVisible && !BuildingPanel.gameObject.activeSelf))
                    {
                        BuildingPanel.gameObject.SetActive(isVisible);
                    }
                };
            }
        }


        /// <summary>
        /// Scrolls the selected panel left or right if an arrow scrolling button is pressed.
        /// </summary>
        /// <param name="component">Selected component</param>
        /// <param name="eventParam">Event parameter</param>
        /// <param name="selected"></param>
        public void ArrowClicked(UIComponent component)
        {
            // Which direction to scroll depends on which button was clicked.
            if (component == LeftButton)
            {
                // Scroll left.
                currentSelection.scrollPosition = currentSelection.scrollPosition - new Vector2(109, 0);
            }
            else
            {
                // Scroll right.
                currentSelection.scrollPosition = currentSelection.scrollPosition + new Vector2(109, 0);
            }

        }


        /// <summary>
        /// Adds/updates text components of the Ploppable Tool Panel (tooltips, settings button) according to the current language.
        /// </summary>
        internal void SetText()
        {
            // Set settings button text.
            showSettings.text = Translations.Translate("PRR_SET");

            // Populate tooltips.
            UICategories tooltips = new UICategories();
            for (int i = 0; i <= NumTypes; i++)
            {
                TabButtons[i].tooltip = tooltips.names[i];
            }
        }


        /// <summary>
        /// Fills the Ploppable Tool panel with building buttons.
        /// </summary>
        private void PopulateButtons()
        {
            Debugging.Message("populating building buttons");

            // Step through each loaded and active RICO prefab.
            foreach (BuildingData buildingData in Loading.xmlManager.prefabHash.Values)
            {
                if (buildingData != null)
                {
                    // Get the prefab.
                    BuildingInfo prefab = PrefabCollection<BuildingInfo>.FindLoaded(buildingData.name);

                    // Local settings first.
                    if (buildingData.hasLocal)
                    {
                        // Only if enabled.
                        if (buildingData.local.ricoEnabled)
                        {
                            // Add button to panel.
                            AddBuildingButton(buildingData, buildingData.local.uiCategory);
                            continue;
                        }
                    }
                    // Author settings second.
                    else if (buildingData.hasAuthor)
                    {
                        // Only if enabled.
                        if (buildingData.author.ricoEnabled)
                        {
                            // Add button to panel.
                            AddBuildingButton(buildingData, buildingData.author.uiCategory);
                            continue;
                        }
                    }
                    // Mod settings third.  Don't have to worry about enablement here.
                    else if (buildingData.hasMod)
                    {
                        AddBuildingButton(buildingData, buildingData.mod.uiCategory);
                    }
                }
            }

            // Set active tab as default.
            TabClicked(BuildingPanels[0], TabSprites[0]);
        }


        /// <summary>
        /// Sets the sprite for a given Ploppable Tool panel tab.
        /// </summary>
        /// <param name="sprite">Panel tab</param>
        /// <param name="spriteName">Name of sprite to set</param>
        private void SetTabSprite(UISprite sprite, string spriteName)
        {
            UISprite tabSprite = sprite;
            tabSprite.isInteractive = false;
            tabSprite.relativePosition = new Vector2(5, 0);
            tabSprite.spriteName = spriteName;
            tabSprite.size = new Vector2(35, 25);
        }


        /// <summary>
        /// Generates a button for each ploppable building in the relevant panel as determined by UI category.
        /// </summary>
        /// <param name="buildingData">RICO building to add</param>
        /// <param name="uiCategory">UI category</param>
        internal void AddBuildingButton(BuildingData buildingData, string uiCategory)
        {
            // Set UI category index for this buildingData instance.
            buildingData.uiCategory = UICategoryIndex(uiCategory);

            // Don't do anything if UI category is set to 'none'.
            if (uiCategory.Equals("none"))
            {
                return;
            }

            try
            {
                // Add building button to relevant panel.
                buildingData.buildingButton = new UIButton();
                buildingData.buildingButton = BuildingPanels[buildingData.uiCategory].AddUIComponent<UIButton>();

                // Size and position.
                buildingData.buildingButton.size = new Vector2(109, 100);
                buildingData.buildingButton.horizontalAlignment = UIHorizontalAlignment.Center;
                buildingData.buildingButton.verticalAlignment = UIVerticalAlignment.Middle;
                buildingData.buildingButton.pivot = UIPivotPoint.TopCenter;

                // Assign prefab.
                buildingData.buildingButton.objectUserData = buildingData.prefab;

                // Add thumbnail rendering to queue.
                ThumbnailManager.QueueThumbnail(buildingData);

                // Information label - building name.
                UILabel nameLabel = buildingData.buildingButton.AddUIComponent<UILabel>();
                nameLabel.textScale = 0.6f;
                nameLabel.useDropShadow = true;
                nameLabel.dropShadowColor = new Color32(80, 80, 80, 255);
                nameLabel.dropShadowOffset = new Vector2(2, -2);
                nameLabel.text = buildingData.displayName;
                nameLabel.autoSize = false;
                nameLabel.autoHeight = true;
                nameLabel.wordWrap = true;
                nameLabel.width = buildingData.buildingButton.width - 10;
                nameLabel.isVisible = true;
                nameLabel.relativePosition = new Vector3(5, 5);

                // Information label - building level.
                UILabel levelLabel = buildingData.buildingButton.AddUIComponent<UILabel>();
                levelLabel.textScale = 0.6f;
                levelLabel.useDropShadow = true;
                levelLabel.dropShadowColor = new Color32(80, 80, 80, 255);
                levelLabel.dropShadowOffset = new Vector2(2, -2);
                levelLabel.text = Translations.Translate("PRR_LVL") + " " + ((int)buildingData.prefab.m_class.m_level + 1);
                levelLabel.autoSize = true;
                levelLabel.isVisible = true;
                levelLabel.relativePosition = new Vector3(5, buildingData.buildingButton.height - levelLabel.height - 5);

                // Information label - building size.
                UILabel sizeLabel = buildingData.buildingButton.AddUIComponent<UILabel>();
                sizeLabel.textScale = 0.6f;
                sizeLabel.useDropShadow = true;
                sizeLabel.dropShadowColor = new Color32(80, 80, 80, 255);
                sizeLabel.dropShadowOffset = new Vector2(2, -2);
                sizeLabel.text = buildingData.prefab.GetWidth() + "x" + buildingData.prefab.GetLength();
                sizeLabel.autoSize = true;
                sizeLabel.isVisible = true;
                sizeLabel.relativePosition = new Vector3(buildingData.buildingButton.width - sizeLabel.width - 5, buildingData.buildingButton.height - sizeLabel.height - 5);

                // Tooltip.
                buildingData.buildingButton.tooltipAnchor = UITooltipAnchor.Anchored;
                buildingData.buildingButton.tooltip = BuildingTooltip(buildingData);
                buildingData.buildingButton.eventClick += (component, clickEvent) => BuildingBClicked(buildingData.prefab);
                buildingData.buildingButton.eventMouseHover += (component, mouseEvent) =>
                {
                    // Reset the tooltip before showing each time, as sometimes it gets clobbered either by the game or another mod.
                    component.tooltip = BuildingTooltip(buildingData);
                };

                // Ready to use!
                buildingData.buildingButton.isEnabled = true;
            }
            catch (Exception e)
            {
                Debugging.Message("BuildingButton creation exception with UI category '" + uiCategory);
                Debugging.LogException(e);
            }
        }


        /// <summary>
        /// Creates a tooltip string for a building, including key stats.
        /// </summary>
        /// <param name="building">Building to generate for</param>
        /// <returns>A tooltip string</returns>
        private string BuildingTooltip(BuildingData building)
        {
            StringBuilder tooltip = new StringBuilder();

            tooltip.AppendLine(building.displayName);

            // Construction cost.
            try
            {
                tooltip.AppendLine(LocaleFormatter.FormatCost(building.prefab.GetConstructionCost(), false));
            }
            catch
            {
                // Don't care - just don't show construction cost in the tooltip.
            }

            // Only add households or workplaces for Private AI types, not for e.g. Beautification (dummy service).
            if (building.prefab.GetAI() is PrivateBuildingAI thisAI)
            {
                // Household or workplace count.
                if (building.prefab.GetService() == ItemClass.Service.Residential)
                {
                    // Residential - households.
                    tooltip.Append(Translations.Translate("PRR_HOU"));
                    tooltip.Append(": ");
                    tooltip.AppendLine(thisAI.CalculateHomeCount(building.prefab.GetClassLevel(), new Randomizer(), building.prefab.GetWidth(), building.prefab.GetLength()).ToString());
                }
                else
                {
                    // Non-residential - workplaces.
                    int[] workplaces = new int[4];

                    tooltip.Append(Translations.Translate("PRR_WOR"));
                    tooltip.Append(": ");
                    thisAI.CalculateWorkplaceCount(building.prefab.GetClassLevel(), new Randomizer(), building.prefab.GetWidth(), building.prefab.GetLength(), out workplaces[0], out workplaces[1], out workplaces[2], out workplaces[3]);
                    tooltip.AppendLine(workplaces.Sum().ToString());
                }
            }

            // Physical size.
            tooltip.Append("Size: ");
            tooltip.Append(building.prefab.GetWidth());
            tooltip.Append("x");
            tooltip.AppendLine(building.prefab.GetLength().ToString());

            return tooltip.ToString();
        }


        /// <summary>
        /// Handles click events for building buttons.
        /// Basically, sets the current tool to plop the selected RICO building.
        /// </summary>
        /// <param name="prefab">Selected building prefab</param>
        public void BuildingBClicked(BuildingInfo prefab)
        {
            BuildingTool buildingTool = ToolsModifierControl.SetTool<BuildingTool>();
            {
                buildingTool.m_prefab = prefab;
                buildingTool.m_relocate = 0;
                BuildingPanel.isVisible = true;
            }
        }


        /// <summary>
        /// Handles click events for Ploppable Tool panel tabs.
        /// </summary>
        /// <param name="panel">The Ploppable Tool panel for the selected tab</param>
        /// <param name="sprite">The sprite icon for the selected tab</param>
        public void TabClicked(UIScrollablePanel thisPanel, UISprite sprite)
        {
            // Set current selection.
            currentSelection = thisPanel;

            // Show left and right scroll buttons if we have more than seven building buttons on this panel; otherwise, hide.
            if (thisPanel.childCount > 7)
            {
                LeftButton.isVisible = true;
                RightButton.isVisible = true;
            }
            else
            {
                LeftButton.isVisible = false;
                RightButton.isVisible = false;
            }

            // Hide all building panels.
            foreach (UIScrollablePanel panel in BuildingPanels)
            {
                panel.isVisible = false;
            }

            // Show this panel.
            thisPanel.isVisible = true;

            // Redraw all tab sprites in their base state (unfocused).
            for (int i = 0; i <= NumTypes; i++)
            {
                if (i <= 5)
                {
                    TabSprites[i].spriteName = "Zoning" + Names[i];

                }
                else
                {
                    TabSprites[i].spriteName = "IconPolicy" + Names[i];
                }
            }

            // Focus this sprite (no focused versions for AD or GC sprites so exclude those).
            if (sprite.spriteName != "IconPolicyLeisure" && sprite.spriteName != "IconPolicyTourist" && sprite.spriteName != "IconPolicyHightech" && sprite.spriteName != "IconPolicyOrganic" && sprite.spriteName != "IconPolicySelfsufficient")
            {
                sprite.spriteName = sprite.spriteName + "Focused";
            }
        }


        /// <summary>
        /// Returns the UI category index for the given UI category string.
        /// </summary>
        /// <param name="uiCategory">Ploppable RICO UI category string</param>
        /// <returns>UI category index</returns>
        private int UICategoryIndex(string uiCategory)
        {
            switch (uiCategory)
            {
                case "reslow":
                    return 0;
                case "reshigh":
                    return 1;
                case "comlow":
                    return 2;
                case "comhigh":
                    return 3;
                case "office":
                    return 4;
                case "industrial":
                    return 5;
                case "farming":
                    return 6;
                case "forest":
                    return 7;
                case "oil":
                    return 8;
                case "ore":
                    return 9;
                case "leisure":
                    if (Util.isADinstalled())
                    {
                        return 10;
                    }
                    else
                    {
                        // If AD is not installed, default to low commercial.
                        return 2;
                    }
                case "tourist":
                    if (Util.isADinstalled())
                    {
                        return 11;
                    }
                    else
                    {
                        // If AD is not installed, fall back to low commercial.
                        return 2;
                    }
                case "organic":
                    if (Util.isGCinstalled())
                    {
                        return 12;
                    }
                    else
                    {
                        // If GC is not installed, fall back to low commercial.
                        return 2;
                    }
                case "hightech":
                    // IT cluster.
                    if (Util.isGCinstalled())
                    {
                        return 13;
                    }
                    else
                    {
                        // If GC is not installed, fall back to office.
                        return 4;
                    }
                case "selfsufficient":
                    // Self-sufficient (eco) residential.
                    if (Util.isGCinstalled())
                    {
                        return 14;
                    }
                    else
                    {
                        // If GC is not installed, fall back to low residential.
                        return 0;
                    }
                default:
                    return 0;
            }
        }


        /// <summary>
        /// Regenerates all thumbnails.
        /// Useful for e.g. regenerating thumbnails.
        /// </summary>
        internal void RegenerateThumbnails()
        {
            // Only do this if the ploppable tool has been created.
            if (Instance != null)
            {
                Debugging.Message("regenerating all thumbnails");

                // Step through each loaded and active RICO prefab.
                foreach (BuildingData buildingData in Loading.xmlManager.prefabHash.Values)
                {
                    // See if it has a building button.
                    if (buildingData.buildingButton != null)
                    {
                        // If so, add its thumbnail to the rendering queue.
                        ThumbnailManager.QueueThumbnail(buildingData);
                    }
                }
            }
        }
    }
}




