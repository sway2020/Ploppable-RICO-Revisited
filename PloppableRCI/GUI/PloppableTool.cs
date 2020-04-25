using ColossalFramework;
using UnityEngine;
using System.Linq;
using ColossalFramework.UI;
using ColossalFramework.DataBinding;
using System;


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
                PloppableButton.tooltip = "Ploppable RICO";

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
                        DrawBuildingButton(prefab, buildingData.local.uiCategory);
                        RemoveUIButton(prefab);
                        continue;
                    }

                    else if (buildingData.hasAuthor && buildingData.author.ricoEnabled)
                    {
                        if (!buildingData.hasLocal)
                        {
                            DrawBuildingButton(prefab, buildingData.author.uiCategory);
                            RemoveUIButton(prefab);
                            continue;
                        }
                    }

                    else if (buildingData.hasMod)
                    {
                        if (!buildingData.hasLocal && !buildingData.hasAuthor)
                        {
                            DrawBuildingButton(prefab, buildingData.mod.uiCategory);
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


        void DrawBuildingButton(BuildingInfo BuildingPrefab, string type)
        {
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
                BuildingButton.atlas = BuildingPrefab.m_Atlas;

                if (BuildingPrefab.m_Thumbnail == null || BuildingPrefab.m_Thumbnail == "")
                {
                    BuildingButton.normalFgSprite = "ToolbarIconProps";
                }
                else
                {

                    BuildingButton.normalFgSprite = BuildingPrefab.m_Thumbnail;
                    BuildingButton.focusedFgSprite = BuildingPrefab.m_Thumbnail + "Focused";
                    BuildingButton.hoveredFgSprite = BuildingPrefab.m_Thumbnail + "Hovered";
                    BuildingButton.pressedFgSprite = BuildingPrefab.m_Thumbnail + "Pressed";
                    BuildingButton.disabledFgSprite = BuildingPrefab.m_Thumbnail + "Disabled";
                }

                BuildingButton.objectUserData = BuildingPrefab;
                BuildingButton.horizontalAlignment = UIHorizontalAlignment.Center;
                BuildingButton.verticalAlignment = UIVerticalAlignment.Middle;
                BuildingButton.pivot = UIPivotPoint.TopCenter;

                string localizedTooltip = BuildingPrefab.GetLocalizedTooltip();
                int hashCode = TooltipHelper.GetHashCode(localizedTooltip);
                UIComponent tooltipBox = GeneratedPanel.GetTooltipBox(hashCode);

                BuildingButton.tooltipAnchor = UITooltipAnchor.Anchored;
                BuildingButton.isEnabled = enabled;
                BuildingButton.tooltip = localizedTooltip;
                BuildingButton.tooltipBox = tooltipBox;
                BuildingButton.eventClick += (sender, e) => BuildingBClicked(sender, e, BuildingPrefab);
                BuildingButton.eventMouseHover += (sender, e) => BuildingBHovered(sender, e, BuildingPrefab);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
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




