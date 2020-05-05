using System;
using System.Linq;
using System.Text;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.Math;
using ColossalFramework.Globalization;


namespace PloppableRICO
{
    /// <summary>
    /// This class draws the RICO panel, populates it with building buttons, and activates the building tool when buttons are clicked. 
    /// </summary>
    /// 
    public class PloppableTool : ToolBase
    {
        private static GameObject _gameObject;
        private static PloppableTool _instance;
        public static PloppableTool instance

        {
            get { return _instance; }
        }

        UIButton PloppableButton;
        UIPanel BuildingPanel;
        UITabstrip Tabs;
        UIButton BuildingButton;

        // Number of UI categories.
        const int NumTypes = 14;
        // Number of UI tabs: +1 to account for 'Settings' tab.
        const int NumTabs = NumTypes + 1;

        UISprite[] TabSprites = new UISprite[NumTabs];
        UIScrollablePanel[] BuildingPanels = new UIScrollablePanel[NumTabs];
        UIScrollablePanel currentSelection = new UIScrollablePanel();

        UIButton[] TabButtons = new UIButton[NumTabs];

        UIButton LeftButton = new UIButton();
        UIButton RightButton = new UIButton();

        // Names used to identify icons for tabs (specific game icon names - not just made up).
        string[] Names = new string[]
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

        private UITextureAtlas ingame;
        private UITextureAtlas thumbnails;


        public static void Initialize()
        {
            if (_instance == null)
            {
                try
                {
                    // Creating our own gameObect helps finding the UI in ModTools.
                    _gameObject = new GameObject("PloppableTool");
                    _gameObject.transform.parent = UIView.GetAView().transform;
                    _instance = _gameObject.AddComponent<PloppableTool>();
                    _instance.DrawPloppablePanel();
                    _instance.PopulateAssets();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        

        protected override void Awake()
        {
            this.m_toolController = ToolsModifierControl.toolController;
        }
        

        public static void Destroy()
        {
            try
            {
                if (_gameObject != null)
                    GameObject.Destroy(_gameObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        public void DrawPloppablePanel()
        {
            if (PloppableButton == null)
            {
                ingame = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Ingame");
                thumbnails = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Thumbnails");

                // Main button on ingame toolbar.
                PloppableButton = UIView.GetAView().FindUIComponent<UITabstrip>("MainToolstrip").AddUIComponent<UIButton>();
                PloppableButton.size = new Vector2(43, 49);
                PloppableButton.eventClick += PloppablebuttonClicked;
                PloppableButton.normalBgSprite = "ToolbarIconGroup6Normal";
                PloppableButton.normalFgSprite = "IconPolicyBigBusiness";
                PloppableButton.focusedBgSprite = "ToolbarIconGroup6Focused";
                PloppableButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
                PloppableButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
                PloppableButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
                PloppableButton.relativePosition = new Vector2(800, 0);
                PloppableButton.name = "PloppableButton";
                PloppableButton.tooltip = Translations.GetTranslation("Ploppable RICO");

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

                for (int i = 0; i <= NumTypes; i++)
                {
                    // Draw scrollable panels.
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
                    TabButtons[i].tooltip = Translations.UICategory[i];
                    TabButtons[i].tabStrip = true;

                    TabSprites[i] = new UISprite();
                    TabSprites[i] = TabButtons[i].AddUIComponent<UISprite>();

                    // Standard "Vanilla" categories (low and high residential, low and high commercial, and offices) - use standard zoning icons from original vanilla release.
                    if (i <= 5)
                    {
                        TabSprites[i].atlas = thumbnails;
                        SetSprites(TabSprites[i], "Zoning" + Names[i]);
                    }
                    else
                    {
                        // Other types don't have standard zoning icons; use policy icons instead.
                        SetSprites(TabSprites[i], "IconPolicy" + Names[i]);
                    }
                }
                
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

                currentSelection = BuildingPanels[0];

                RightButton.eventClick += (sender, e) => ArrowClicked(sender, e, currentSelection);
                LeftButton.eventClick += (sender, e) => ArrowClicked(sender, e, currentSelection);

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

                // Couldnt get this to work in the loop.  TODO: reinvestigae.
                TabButtons[0].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[0], TabButtons[0], TabSprites[0]);
                TabButtons[1].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[1], TabButtons[1], TabSprites[1]);
                TabButtons[2].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[2], TabButtons[2], TabSprites[2]);
                TabButtons[3].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[3], TabButtons[3], TabSprites[3]);
                TabButtons[4].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[4], TabButtons[4], TabSprites[4]);
                TabButtons[5].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[5], TabButtons[5], TabSprites[5]);
                TabButtons[6].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[6], TabButtons[6], TabSprites[6]);
                TabButtons[7].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[7], TabButtons[7], TabSprites[7]);
                TabButtons[8].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[8], TabButtons[8], TabSprites[8]);
                TabButtons[9].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[9], TabButtons[9], TabSprites[9]);
                // Below are DLC categories - AD for first two, then GC for next 3.  Will be hidden if relevant DLC is not installed.
                TabButtons[10].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[10], TabButtons[10], TabSprites[10]);
                TabButtons[11].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[11], TabButtons[11], TabSprites[11]);
                TabButtons[12].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[12], TabButtons[12], TabSprites[12]);
                TabButtons[13].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[13], TabButtons[13], TabSprites[13]);
                TabButtons[14].eventClick += (sender, e) => TabClicked(sender, e, BuildingPanels[14], TabButtons[14], TabSprites[14]);

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

                UIButton showThemeManager = UIUtils.CreateButton(Tabs);
                showThemeManager.size = new Vector2(80, 25);
                showThemeManager.normalBgSprite = "SubBarButtonBase";
                showThemeManager.text = Translations.GetTranslation("Settings");
                showThemeManager.eventClick += (c, p) => RICOSettingsPanel.instance.Toggle();
            }
        }


        public void ArrowClicked(UIComponent component, UIMouseEventParameter eventParam, UIScrollablePanel selected)
        {
            // Scrolling.
            if (component == LeftButton)
            {
                currentSelection.scrollPosition = currentSelection.scrollPosition - new Vector2(109, 0);
            }
            else
            {
                currentSelection.scrollPosition = currentSelection.scrollPosition + new Vector2(109, 0);
            }

        }


        public void PopulateAssets()
        {
            foreach (var buildingData in Loading.xmlManager.prefabHash.Values)
            {
                if (buildingData != null)
                {
                    var prefab = PrefabCollection<BuildingInfo>.FindLoaded(buildingData.name);

                    if (buildingData.hasLocal && buildingData.local.ricoEnabled)
                    {
                        DrawBuildingButton(buildingData, buildingData.local.uiCategory);
                        RemoveUIButton(prefab);
                        continue;
                    }

                    else if (buildingData.hasAuthor && buildingData.author.ricoEnabled)
                    {
                        if (!buildingData.hasLocal)
                        {
                            DrawBuildingButton(buildingData, buildingData.author.uiCategory);
                            RemoveUIButton(prefab);
                            continue;
                        }
                    }

                    else if (buildingData.hasMod)
                    {
                        if (!buildingData.hasLocal && !buildingData.hasAuthor)
                        {
                            DrawBuildingButton(buildingData, buildingData.mod.uiCategory);
                            RemoveUIButton(prefab);
                        }
                    }
                }
            }
        }


        public void RemoveUIButton(BuildingInfo prefab)
        {
            var uiView = UIView.GetAView();
            var refButton = new UIButton();

            if (prefab != null) refButton = uiView.FindUIComponent<UIButton>(prefab.name);

            if (refButton != null)
            {
                refButton.isVisible = false;
                GameObject.Destroy(refButton.gameObject);
            }
        }


        public void DrawPanels(UIScrollablePanel panel, string name)
        {
            panel = UIView.GetAView().FindUIComponent("PloppableBuildingPanel").AddUIComponent<UIScrollablePanel>();
            panel.size = new Vector2(763, 109);
            panel.relativePosition = new Vector2(50, 0);
            panel.Reset();
        }


        public void SetSprites(UISprite labe, string sprite)
        {
            UISprite label = labe;
            label.isInteractive = false;
            label.relativePosition = new Vector2(5, 0);
            label.spriteName = sprite;
            label.size = new Vector2(35, 25);
        }


        void DrawBuildingButton(BuildingData buildingData, string type)
        {
            BuildingInfo buildingPrefab = buildingData.prefab;

            // Don't do anything if UI category is set to 'none'.
            if (type.Equals("none"))
            {
                return;
            }

            try
            {
                // Add building buttons to relevant panels.
                BuildingButton = new UIButton();

                if (type == "reslow")
                {
                    BuildingButton = BuildingPanels[0].AddUIComponent<UIButton>();
                }
                else if (type == "reshigh")
                {
                    BuildingButton = BuildingPanels[1].AddUIComponent<UIButton>();
                }
                else if (type == "comlow")
                {
                    BuildingButton = BuildingPanels[2].AddUIComponent<UIButton>();
                }
                else if (type == "comhigh")
                {
                    BuildingButton = BuildingPanels[3].AddUIComponent<UIButton>();
                }
                else if (type == "office")
                {
                    BuildingButton = BuildingPanels[4].AddUIComponent<UIButton>();
                }
                else if (type == "industrial")
                {
                    BuildingButton = BuildingPanels[5].AddUIComponent<UIButton>();
                }
                else if (type == "farming")
                {
                    BuildingButton = BuildingPanels[6].AddUIComponent<UIButton>();
                }
                else if (type == "oil")
                {
                    BuildingButton = BuildingPanels[8].AddUIComponent<UIButton>();
                }
                else if (type == "forest")
                {
                    BuildingButton = BuildingPanels[7].AddUIComponent<UIButton>();
                }
                else if (type == "ore")
                {
                    BuildingButton = BuildingPanels[9].AddUIComponent<UIButton>();
                }
                else if (type == "leisure")
                {
                    if (Util.isADinstalled())
                    {
                        BuildingButton = BuildingPanels[10].AddUIComponent<UIButton>();
                    }
                    else
                    {
                        // If AD is not installed, default to low commercial.
                        BuildingButton = BuildingPanels[2].AddUIComponent<UIButton>();
                    }
                }
                else if (type == "tourist")
                {
                    if (Util.isADinstalled())
                    {
                        BuildingButton = BuildingPanels[11].AddUIComponent<UIButton>();
                    }
                    else
                    {
                        // If AD is not installed, fall back to low commercial.
                        BuildingButton = BuildingPanels[2].AddUIComponent<UIButton>();
                    }
                }
                else if (type == "organic")
                {
                    // Eco commercial.
                    if (Util.isGCinstalled())
                    {
                        BuildingButton = BuildingPanels[12].AddUIComponent<UIButton>();
                    }
                    else
                    {
                        // If GC is not installed, fall back to low commercial.
                        BuildingButton = BuildingPanels[2].AddUIComponent<UIButton>();
                    }
                }
                else if (type == "hightech")
                {
                    // IT cluster.
                    if (Util.isGCinstalled())
                    {
                        BuildingButton = BuildingPanels[13].AddUIComponent<UIButton>();
                    }
                    else
                    {
                        // If GC is not installed, fall back to office.
                        BuildingButton = BuildingPanels[4].AddUIComponent<UIButton>();
                    }
                }
                else if (type == "selfsufficient")
                {
                    // Self-sufficient (eco) residential.
                    if (Util.isGCinstalled())
                    {
                        BuildingButton = BuildingPanels[14].AddUIComponent<UIButton>();
                    }
                    else
                    {
                        // If GC is not installed, fall back to low residential.
                        BuildingButton = BuildingPanels[0].AddUIComponent<UIButton>();
                    }
                }

                // Apply settings to building buttons.
                BuildingButton.size = new Vector2(109, 100);

                // Create thumbnails.
                Thumbnails.CreateThumbnail(buildingData);
                BuildingButton.atlas = buildingData.prefab.m_Atlas;
                BuildingButton.normalFgSprite = buildingData.prefab.m_Thumbnail;
                BuildingButton.focusedFgSprite = buildingData.prefab.m_Thumbnail + "Focused";
                BuildingButton.hoveredFgSprite = buildingData.prefab.m_Thumbnail + "Hovered";
                BuildingButton.pressedFgSprite = buildingData.prefab.m_Thumbnail + "Pressed";
                BuildingButton.disabledFgSprite = buildingData.prefab.m_Thumbnail + "Disabled";

                BuildingButton.objectUserData = buildingData.prefab;
                BuildingButton.horizontalAlignment = UIHorizontalAlignment.Center;
                BuildingButton.verticalAlignment = UIVerticalAlignment.Middle;
                BuildingButton.pivot = UIPivotPoint.TopCenter;

                // Information label - building name.
                UILabel nameLabel = new UILabel();
                nameLabel = BuildingButton.AddUIComponent<UILabel>();
                nameLabel.textScale = 0.6f;
                nameLabel.useDropShadow = true;
                nameLabel.dropShadowColor = new Color32(80, 80, 80, 255);
                nameLabel.dropShadowOffset = new Vector2(2, -2);
                nameLabel.text = buildingData.displayName;
                nameLabel.autoSize = false;
                nameLabel.autoHeight = true;
                nameLabel.wordWrap = true;
                nameLabel.width = BuildingButton.width - 10;
                nameLabel.isVisible = true;
                nameLabel.relativePosition = new Vector3(5, 5);

                // Information label - building level.
                UILabel levelLabel = new UILabel();
                levelLabel = BuildingButton.AddUIComponent<UILabel>();
                levelLabel.textScale = 0.6f;
                levelLabel.useDropShadow = true;
                levelLabel.dropShadowColor = new Color32(80, 80, 80, 255);
                levelLabel.dropShadowOffset = new Vector2(2, -2);
                levelLabel.text = Translations.GetTranslation("Lvl ") + ((int)buildingData.prefab.m_class.m_level + 1);
                levelLabel.autoSize = true;
                levelLabel.isVisible = true;
                levelLabel.relativePosition = new Vector3(5, BuildingButton.height - levelLabel.height - 5);

                // Information label - building size.
                UILabel sizeLabel = new UILabel();
                sizeLabel = BuildingButton.AddUIComponent<UILabel>();
                sizeLabel.textScale = 0.6f;
                sizeLabel.useDropShadow = true;
                sizeLabel.dropShadowColor = new Color32(80, 80, 80, 255);
                sizeLabel.dropShadowOffset = new Vector2(2, -2);
                sizeLabel.text = buildingData.prefab.GetWidth() + "x" + buildingData.prefab.GetLength();
                sizeLabel.autoSize = true;
                sizeLabel.isVisible = true;
                sizeLabel.relativePosition = new Vector3(BuildingButton.width - sizeLabel.width - 5, BuildingButton.height - sizeLabel.height - 5);

                // Tooltip.
                BuildingButton.tooltipAnchor = UITooltipAnchor.Anchored;
                BuildingButton.isEnabled = enabled;
                BuildingButton.tooltip = BuildingTooltip(buildingData);
                BuildingButton.eventClick += (sender, e) => BuildingBClicked(sender, e, buildingData.prefab);
                BuildingButton.eventMouseHover += (sender, e) => BuildingBHovered(sender, e, buildingData.prefab);
            }
            catch (Exception e)
            {
                Debug.Log("RICO Revisited: BuildingButton creation exception with type '" + type + "'.");
                Debug.LogException(e);
            }
        }


        /// <summary>
        /// Creates a tooltip string for a building, including key stats.
        /// </summary>
        /// <param name="building">Building to generate for</param>
        /// <returns>A tooltip string</returns>
        string BuildingTooltip(BuildingData building)
        {
            StringBuilder tooltip = new StringBuilder();

            tooltip.AppendLine(building.displayName);

            // Construction cost.
            tooltip.AppendLine(LocaleFormatter.FormatCost(building.prefab.GetConstructionCost(), false));

            // Household or workplace count.
            if (building.prefab.GetService() == ItemClass.Service.Residential)
            {
                // Residential - households.
                tooltip.Append(Translations.GetTranslation("Households"));
                tooltip.Append(": ");
                tooltip.AppendLine(((PrivateBuildingAI)building.prefab.GetAI()).CalculateHomeCount(building.prefab.GetClassLevel(), new Randomizer(), building.prefab.GetWidth(), building.prefab.GetLength()).ToString());
            }
            else
            {
                // Non-residential - workplaces.
                int[] workplaces = new int[4];

                tooltip.Append(Translations.GetTranslation("Workplaces"));
                tooltip.Append(": ");
                ((PrivateBuildingAI)building.prefab.GetAI()).CalculateWorkplaceCount(building.prefab.GetClassLevel(), new Randomizer(), building.prefab.GetWidth(), building.prefab.GetLength(), out workplaces[0], out workplaces[1], out workplaces[2], out workplaces[3]);
                tooltip.AppendLine(workplaces.Sum().ToString());
            }
            // Physical size.
            tooltip.Append("Size: ");
            tooltip.Append(building.prefab.GetWidth());
            tooltip.Append("x");
            tooltip.AppendLine(building.prefab.GetLength().ToString());

            return tooltip.ToString();
        }


        void BuildingBClicked(UIComponent component, UIMouseEventParameter eventParam, BuildingInfo Binf)
        {
            var buildingTool = ToolsModifierControl.SetTool<BuildingTool>();
            {
                buildingTool.m_prefab = Binf;
                buildingTool.m_relocate = 0;
                BuildingPanel.isVisible = true;
            }
        }


        void BuildingBHovered(UIComponent component, UIMouseEventParameter eventParam, BuildingInfo Binf)
        {
            var tooltipBoxa = UIView.GetAView().FindUIComponent<UIPanel>("InfoAdvancedTooltip");
            var tooltipBox = UIView.GetAView().FindUIComponent<UIPanel>("InfoAdvancedTooltipDetail");
            var spritea = tooltipBoxa.Find<UISprite>("Sprite");
            var sprite = tooltipBox.Find<UISprite>("Sprite");

            sprite.atlas = Binf.m_Atlas;
            spritea.atlas = Binf.m_Atlas;
        }


        void PloppablebuttonClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            component.Focus();
            BuildingPanel.isVisible = true;
        }


        void TabClicked(UIComponent component, UIMouseEventParameter eventParam, UIScrollablePanel panel, UIButton button, UISprite sprite)
        {
            currentSelection = panel;

            if (panel.childCount > 7)
            {
                LeftButton.isVisible = true;
                RightButton.isVisible = true;
            }
            else
            {
                LeftButton.isVisible = false;
                RightButton.isVisible = false;
            }

            foreach (UIScrollablePanel pan in BuildingPanels)
            {
                pan.isVisible = false;
            }

            panel.isVisible = true;

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

            // Add focused versions of sprites (no focused versions for AD or GC sprites so exclude those).
            if (sprite.spriteName != "IconPolicyLeisure" && sprite.spriteName != "IconPolicyTourist" && sprite.spriteName != "IconPolicyHightech" && sprite.spriteName != "IconPolicyOrganic" && sprite.spriteName != "IconPolicySelfsufficient")
            {
                sprite.spriteName = sprite.spriteName + "Focused";
            }
        }
    }
}




