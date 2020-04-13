using System;
using ColossalFramework.UI;
using UnityEngine;


namespace PloppableRICO
{
    /// <summary>
    ///The far right column of the settigns panel. Contains the drop downs and entry fields that allows players to assign RICO settings. 
    /// </summary>

    public class UIBuildingOptions : UIScrollablePanel
    {
        string[] Service = new string[]
        {
            Translations.GetTranslation("None"),
            Translations.GetTranslation("Residential"),
            Translations.GetTranslation("Industrial"),
            Translations.GetTranslation("Office"),
            Translations.GetTranslation("Commercial"),
            Translations.GetTranslation("Extractor"),
            Translations.GetTranslation("Dummy")
        };

        string[] OfficeSub = new string[]
        {
            Translations.GetTranslation("Generic"),
            Translations.GetTranslation("IT cluster")
        };

        string[] ResSub = new string[]
        {
            Translations.GetTranslation("High"),
            Translations.GetTranslation("Low"),
            Translations.GetTranslation("High eco"),
            Translations.GetTranslation("Low eco")
        };

        string[] ComSub = new string[]
        {
            Translations.GetTranslation("High"),
            Translations.GetTranslation("Low"),
            Translations.GetTranslation("Tourism"),
            Translations.GetTranslation("Leisure"),
            Translations.GetTranslation("Eco (organic)")
        };

        string[] IndustrialSub = new string[]
        {
            Translations.GetTranslation("Generic"),
            Translations.GetTranslation("Farming"),
            Translations.GetTranslation("Forestry"),
            Translations.GetTranslation("Oil"),
            Translations.GetTranslation("Ore")
        };

        string[] ExtractorSub = new string[]
        {
            Translations.GetTranslation("Farming"),
            Translations.GetTranslation("Forestry"),
            Translations.GetTranslation("Oil"),
            Translations.GetTranslation("Ore")
        };

        string[] Level = new string[]
        {
            "1",
            "2",
            "3",
        };

        string[] resLevel = new string[]
        {
            "1",
            "2",
            "3",
            "4",
            "5"
        };

        string[] extLevel = new string[]
        {
            "1"
        };

        public bool disableEvents;
        public RICOBuilding currentSelection;
        // Enable RICO.
        public UICheckBox ricoEnabled;
        public UIPanel enableRICOPanel;

        public UICheckBox growable;

        public UIDropDown service;
        public UIDropDown subService;
        public UIDropDown level;
        public UIDropDown uiCategory;

        // Households / aggregate workplaces.
        public UITextField manual;

        // Workplace breakdown by education level.
        public UITextField uneducated;
        public UITextField educated;
        public UITextField welleducated;
        public UITextField highlyeducated;

        // Pollution.
        public UICheckBox pollutionEnabled;
        public UIPanel pollutionPanel;

        public UICheckBox realityIgnored;
        public UICheckBox manualWorkersEnabled;
        public UIPanel manualPanel;

        // Construction.
        public UICheckBox constructionCostEnabled;
        public UIPanel constructionPanel;
        public UITextField construction;

        public UILabel label;
        public UIPanel labelpanel;

        private static UIBuildingOptions _instance;
        public static UIBuildingOptions instance
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
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding.top = 5;
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


        private void SetupControls()
        {
            // Subpanels.
            labelpanel = this.AddUIComponent<UIPanel>();
            labelpanel.height = 20;

            label = labelpanel.AddUIComponent<UILabel>();
            label.relativePosition = new Vector3(80, 0);
            label.width = 270;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.text = Translations.GetTranslation("No settings");

            ricoEnabled = UIUtils.CreateCheckBar(this, Translations.GetTranslation("Enable RICO"));

            enableRICOPanel = this.AddUIComponent<UIPanel>();
            enableRICOPanel.height = 0;
            enableRICOPanel.isVisible = false;
            enableRICOPanel.name = "OptionsPanel";

            ricoEnabled.eventCheckChanged += (c, state) =>
            {
                if (!state)
                {
                    enableRICOPanel.height = 0;
                    enableRICOPanel.isVisible = false;
                }

                else {
                    enableRICOPanel.height = 240;
                    enableRICOPanel.isVisible = true;
                }
            };

            // Dropdown menus.
            service = UIUtils.CreateDropDown(enableRICOPanel, 30, Translations.GetTranslation("Service"));
            service.items = Service;
            service.selectedIndex = 0;
            service.eventSelectedIndexChanged += UpdateService;

            subService = UIUtils.CreateDropDown(enableRICOPanel, 60, Translations.GetTranslation("Sub-service"));
            subService.selectedIndex = 0;
            // Update UI category when subservice changes.
            subService.eventSelectedIndexChanged += UpdateSubService;

            uiCategory = UIUtils.CreateDropDown(enableRICOPanel, 90, Translations.GetTranslation("UI category"));
            uiCategory.selectedIndex = 0;
            uiCategory.items = Translations.UICategory;

            level = UIUtils.CreateDropDown(enableRICOPanel, 120, Translations.GetTranslation("Level"));
            level.selectedIndex = 0;
            level.items = Level;

            // Base text fields.
            construction = UIUtils.CreateTextField(enableRICOPanel, 150, Translations.GetTranslation("Construction cost"));
            manual = UIUtils.CreateTextField(enableRICOPanel, 180, Translations.GetTranslation("Worker/Home count"));

            // Base checkboxes.
            realityIgnored = UIUtils.CreateCheckBox(enableRICOPanel, 210, Translations.GetTranslation("Use Realistic Pop mod"));
            pollutionEnabled = UIUtils.CreateCheckBox(enableRICOPanel, 240, Translations.GetTranslation("Enable pollution"));
            growable = UIUtils.CreateCheckBox(enableRICOPanel, 0, Translations.GetTranslation("Growable [EXPERIMENTAL]"));

            // Workplace breakdown by education level.
            uneducated = UIUtils.CreateTextField(enableRICOPanel, 300, Translations.GetTranslation("Uneducated jobs"));
            educated = UIUtils.CreateTextField(enableRICOPanel, 330, Translations.GetTranslation("Educated jobs"));
            welleducated = UIUtils.CreateTextField(enableRICOPanel, 360, Translations.GetTranslation("Well-educated jobs"));
            highlyeducated = UIUtils.CreateTextField(enableRICOPanel, 390, Translations.GetTranslation("Highly-educated jobs"));
        }


        public void UpdateService(UIComponent c, int value)
        {
            // Update options panel if the service is changed.

            if (!disableEvents)
            {
                if (value == 0) UpdateElements("none");
                else if (value == 1) UpdateElements("residential");
                else if (value == 2) UpdateElements("industrial");
                else if (value == 3) UpdateElements("office");
                else if (value == 4) UpdateElements("commercial");
                else if (value == 5) UpdateElements("extractor");
                else if (value == 6) UpdateElements("dummy");
            }
        }


        public void UpdateSubService(UIComponent c, int value)
        {
            // Update UI category if the subservice is changed.

            if (!disableEvents)
            {
                UpdateUICategory();
            }
        }


        public void SaveRICO()
        {
            // Reads current settings from UI elements, and saves them to the XMLData.

            if (service.selectedIndex == 0)
            {
                currentSelection.service = "none";
            }

            else if (service.selectedIndex == 1)
            {
                currentSelection.service = "residential";
                if (subService.selectedIndex == 0) currentSelection.subService = "high";
                else if (subService.selectedIndex == 1) currentSelection.subService = "low";
                else if (subService.selectedIndex == 2) currentSelection.subService = "high eco";
                else if (subService.selectedIndex == 3) currentSelection.subService = "low eco";
            }
            else if (service.selectedIndex == 2)
            {
                currentSelection.service = "industrial";

                if (subService.selectedIndex == 0) currentSelection.subService = "generic";
                else if (subService.selectedIndex == 1) currentSelection.subService = "farming";
                else if (subService.selectedIndex == 2) currentSelection.subService = "forest";
                else if (subService.selectedIndex == 3) currentSelection.subService = "oil";
                else if (subService.selectedIndex == 4) currentSelection.subService = "ore";
            }
            else if (service.selectedIndex == 3)
            {
                currentSelection.service = "office";

                if (subService.selectedIndex == 0) currentSelection.subService = "none";
                else if (subService.selectedIndex == 1) currentSelection.subService = "high tech";
            }
            else if (service.selectedIndex == 4)
            {
                currentSelection.service = "commercial";
                if (subService.selectedIndex == 0) currentSelection.subService = "high";
                else if (subService.selectedIndex == 1) currentSelection.subService = "low";
                else if (subService.selectedIndex == 2) currentSelection.subService = "tourist";
                else if (subService.selectedIndex == 3) currentSelection.subService = "leisure";
                else if (subService.selectedIndex == 4) currentSelection.subService = "eco";
            }
            else if (service.selectedIndex == 5)
            {
                currentSelection.service = "extractor";
                if (subService.selectedIndex == 0) currentSelection.subService = "farming";
                else if (subService.selectedIndex == 1) currentSelection.subService = "forest";
                else if (subService.selectedIndex == 2) currentSelection.subService = "oil";
                else if (subService.selectedIndex == 3) currentSelection.subService = "ore";

            }
            else if (service.selectedIndex == 6)
            {
                currentSelection.service = "dummy";
                currentSelection.subService = "none";

            }

            // Save workplaces.
            var a = new int[]
            {
                int.Parse(uneducated.text),
                int.Parse(educated.text),
                int.Parse(welleducated.text),
                int.Parse(highlyeducated.text)
            };
            // Yeah, it's a bit clunky to add the elements individually like this, but saves bringing in System.Linq for just this one case.
            if (a[0] + a[1] + a[2] + a[3] == 0)
            {
                // No workplace breakdown provided (all fields zero); use total workplaces ('manual') and allocate.
                var d = Util.WorkplaceDistributionOf(currentSelection.service, currentSelection.subService, "Level" + currentSelection.level);
                a = WorkplaceAIHelper.distributeWorkplaceLevels(int.Parse(manual.text), d, new int[] { 0, 0, 0, 0 });

                // Check and adjust for any rounding errors, assigning 'leftover' jobs to the lowest education level.
                a[0] += (int.Parse(manual.text) - a[0] - a[1] - a[2] - a[3]);
            }

            currentSelection.workplaces = a;

            currentSelection.constructionCost = int.Parse(construction.text);
            // Construction cost should be at least 10 to maintain compatibility with other mods (Real Time, Real Construction).
            if (currentSelection.constructionCost < 10)
            {
                currentSelection.constructionCost = 10;
                // If we've overridden the value (set it to 10), write that back to the construction cost text field so the user knows.
                construction.text = currentSelection.constructionCost.ToString();
            }

            currentSelection.homeCount = int.Parse(manual.text);

            // UI categories from menu.
            if (uiCategory.selectedIndex == 0) currentSelection.uiCategory = "reslow";
            else if (uiCategory.selectedIndex == 1) currentSelection.uiCategory = "reshigh";
            else if (uiCategory.selectedIndex == 2) currentSelection.uiCategory = "comlow";
            else if (uiCategory.selectedIndex == 3) currentSelection.uiCategory = "comhigh";
            else if (uiCategory.selectedIndex == 4) currentSelection.uiCategory = "office";
            else if (uiCategory.selectedIndex == 5) currentSelection.uiCategory = "industrial";
            else if (uiCategory.selectedIndex == 6) currentSelection.uiCategory = "farming";
            else if (uiCategory.selectedIndex == 7) currentSelection.uiCategory = "forest";
            else if (uiCategory.selectedIndex == 8) currentSelection.uiCategory = "oil";
            else if (uiCategory.selectedIndex == 9) currentSelection.uiCategory = "ore";
            else if (uiCategory.selectedIndex == 10) currentSelection.uiCategory = "tourist";
            else if (uiCategory.selectedIndex == 11) currentSelection.uiCategory = "leisure";
            else if (uiCategory.selectedIndex == 12) currentSelection.uiCategory = "organic";
            else if (uiCategory.selectedIndex == 13) currentSelection.uiCategory = "hightech";
            else if (uiCategory.selectedIndex == 14) currentSelection.uiCategory = "selfsufficient";
            else if (uiCategory.selectedIndex == 15) currentSelection.uiCategory = "none";

            // Remaining items.
            currentSelection.level = level.selectedIndex + 1;
            currentSelection.ricoEnabled = ricoEnabled.isChecked;
            currentSelection.growable = growable.isChecked;
            currentSelection.RealityIgnored = !realityIgnored.isChecked;
            currentSelection.pollutionEnabled = pollutionEnabled.isChecked;
        }


        public void SelectionChanged(BuildingData buildingData)
        {
            // Disable the event logic while dropdowns are being updated.
            disableEvents = true;

            // Disable all input controls by default; activate them later if needed.
            ricoEnabled.Disable();
            growable.Disable();
            service.Disable();
            subService.Disable();
            level.Disable();
            uiCategory.Disable();
            construction.Disable();
            manual.Disable();
            realityIgnored.Disable();
            uneducated.Disable();
            educated.Disable();
            welleducated.Disable();
            highlyeducated.Disable();

            // Update option UI elements, in priority order (local, author, mod).
            if (buildingData.hasLocal)
            {
                currentSelection = buildingData.local;
                UpdateElements(buildingData.local.service);
                UpdateValues(buildingData.local);
                label.text = Translations.GetTranslation("Local settings");

                // If the building has local settings, enable input fields.
                ricoEnabled.Enable();
                growable.Enable();
                service.Enable();
                subService.Enable();
                level.Enable();
                uiCategory.Enable();
                construction.Enable();
                manual.Enable();
                realityIgnored.Enable();
                uneducated.Enable();
                educated.Enable();
                welleducated.Enable();
                highlyeducated.Enable();

                // Re-enable event logic now that dropdowns are up-to-date before returning.
                disableEvents = false;
                return;
            }
            else if (buildingData.hasAuthor)
            {
                // If the building has author settings, then disable input fields.
                currentSelection = buildingData.author;
                UpdateElements(buildingData.author.service);
                UpdateValues(buildingData.author);
                label.text = Translations.GetTranslation("Author settings");

                // Re-enable event logic now that dropdowns are up-to-date before returning.
                disableEvents = false;
                return;
            }
            else if (buildingData.hasMod)
            {
                // If the building has mod settings, then disable input fields.
                currentSelection = buildingData.mod;
                label.text = Translations.GetTranslation("Mod settings");
                UpdateElements(buildingData.mod.service);
                UpdateValues(buildingData.mod);

                // Re-enable event logic now that dropdowns are up-to-date before returning.
                disableEvents = false;
                return;
            }
            else
            {
                // Fallback - building has no Ploppable RICO data anywhere, disable Ploppable RICO.
                ricoEnabled.isChecked = false;
                ricoEnabled.Disable();
                label.text = Translations.GetTranslation("No settings");
            }

            // Catchall to ensure event logic re-enabled before we leave.
            disableEvents = false;
        }


        public void NoSettings()
        {

            // Hide all options if selected building has no RICO settings.

            ricoEnabled.Disable();
        }


        public void UpdateValues(RICOBuilding buildingData)
        {
            // Updates the values in the RICO options panel to match the selected building.

            // Workplaces.
            manual.text = buildingData.workplaceCount.ToString();
            uneducated.text = buildingData.workplaces[0].ToString();
            educated.text = buildingData.workplaces[1].ToString();
            welleducated.text = buildingData.workplaces[2].ToString();
            highlyeducated.text = buildingData.workplaces[3].ToString();

            // Service and sub-service.
            if (buildingData.service == "residential")
            {
                manual.text = buildingData.homeCount.ToString();
                service.selectedIndex = 1;

                if (currentSelection.subService == "high") subService.selectedIndex = 0;
                else if (currentSelection.subService == "low") subService.selectedIndex = 1;
                else if (currentSelection.subService == "high eco") subService.selectedIndex = 2;
                else if (currentSelection.subService == "low eco") subService.selectedIndex = 3;
            }
            else if (buildingData.service == "industrial")
            {
                service.selectedIndex = 2;
                subService.items = IndustrialSub;

                if (currentSelection.subService == "generic") subService.selectedIndex = 0;
                else if (currentSelection.subService == "farming") subService.selectedIndex = 1;
                else if (currentSelection.subService == "forest") subService.selectedIndex = 2;
                else if (currentSelection.subService == "oil") subService.selectedIndex = 3;
                else if (currentSelection.subService == "ore") subService.selectedIndex = 4;
            }
            else if (buildingData.service == "office")
            {
                service.selectedIndex = 3;
                subService.items = OfficeSub;

                if (currentSelection.subService == "none") subService.selectedIndex = 0;
                else if (currentSelection.subService == "high tech") subService.selectedIndex = 1;
            }
            else if (buildingData.service == "commercial")
            {
                service.selectedIndex = 4;
                subService.items = ComSub;

                if (currentSelection.subService == "high") subService.selectedIndex = 0;
                else if (currentSelection.subService == "low") subService.selectedIndex = 1;
                else if (currentSelection.subService == "tourist") subService.selectedIndex = 2;
                else if (currentSelection.subService == "leisure") subService.selectedIndex = 3;
                else if (currentSelection.subService == "eco") subService.selectedIndex = 4;
            }
            else if (buildingData.service == "extractor")
            {
                service.selectedIndex = 5;
                subService.items = ExtractorSub;

                if (currentSelection.subService == "farming") subService.selectedIndex = 0;
                else if (currentSelection.subService == "forest") subService.selectedIndex = 1;
                else if (currentSelection.subService == "oil") subService.selectedIndex = 2;
                else if (currentSelection.subService == "ore") subService.selectedIndex = 3;
            }
            else if (buildingData.service == "dummy")
            {
                service.selectedIndex = 6;
                subService.selectedIndex = 0;
                subService.items = OfficeSub;
            }

            // UI category.
            if (buildingData.uiCategory == "reslow") uiCategory.selectedIndex = 0;
            else if (buildingData.uiCategory == "reshigh") uiCategory.selectedIndex = 1;
            else if (buildingData.uiCategory == "comlow") uiCategory.selectedIndex = 2;
            else if (buildingData.uiCategory == "comhigh") uiCategory.selectedIndex = 3;
            else if (buildingData.uiCategory == "office") uiCategory.selectedIndex = 4;
            else if (buildingData.uiCategory == "industrial") uiCategory.selectedIndex = 5;
            else if (buildingData.uiCategory == "farming") uiCategory.selectedIndex = 6;
            else if (buildingData.uiCategory == "forest") uiCategory.selectedIndex = 7;
            else if (buildingData.uiCategory == "oil") uiCategory.selectedIndex = 8;
            else if (buildingData.uiCategory == "ore") uiCategory.selectedIndex = 9;
            else if (buildingData.uiCategory == "tourist") uiCategory.selectedIndex = 10;
            else if (buildingData.uiCategory == "leisure") uiCategory.selectedIndex = 11;
            else if (buildingData.uiCategory == "none") uiCategory.selectedIndex = 12;
            else if (buildingData.uiCategory == "organic") uiCategory.selectedIndex = 12;
            else if (buildingData.uiCategory == "hightech") uiCategory.selectedIndex = 13;
            else if (buildingData.uiCategory == "selfsufficient") uiCategory.selectedIndex = 14;
            else if (buildingData.uiCategory == "none") uiCategory.selectedIndex = 15;

            // Building level.
            level.selectedIndex = (buildingData.level - 1);

            // Construction cost.
            construction.text = buildingData.constructionCost.ToString();

            // Use realistic population.
            realityIgnored.isChecked = !buildingData.RealityIgnored;

            // Pollution enabled
            pollutionEnabled.isChecked = buildingData.pollutionEnabled;

            // Growable.
            growable.isChecked = buildingData.growable;

            // Enable RICO.
            ricoEnabled.isChecked = buildingData.ricoEnabled;
        }


        public void UpdateElements(string service)
        {
            // Reconfigure the RICO options panel to display relevant options for a given service.
            // This simply hides/shows different option fields for the various services.

            // Defaults by probability.  Pollution is only needed for industrial and extractor, and workplaces are shown for everything except residential.
            pollutionEnabled.enabled = false;
            pollutionEnabled.parent.Hide();
            uneducated.parent.Show();
            educated.parent.Show();
            welleducated.parent.Show();
            highlyeducated.parent.Show();

            if (service == "residential")
            {
                level.items = resLevel;
                subService.items = ResSub;

                // No workplaces breakdown for residential - hide them.
                uneducated.parent.Hide();
                educated.parent.Hide();
                welleducated.parent.Hide();
                highlyeducated.parent.Hide();
            }
            else if (service == "office")
            {
                level.items = Level;
                subService.items = OfficeSub;
                // Maximum legitimate level is 3 (selectedIndex is level - 1)
                level.selectedIndex = Math.Min(level.selectedIndex, 2);
            }
            else if (service == "industrial")
            {
                level.items = Level;
                subService.items = IndustrialSub;
                // Industries can pollute.
                pollutionEnabled.enabled = true;
                pollutionEnabled.parent.Show();
                // Maximum legitimate level is 3 (selectedIndex is level - 1)
                level.selectedIndex = Math.Min(level.selectedIndex, 2);
            }
            else if (service == "extractor")
            {
                level.items = extLevel;
                subService.items = ExtractorSub;
                // Extractors can pollute.
                pollutionEnabled.enabled = true;
                // Maximum legitimate level is 1 (selectedIndex is level - 1)
                level.selectedIndex = 0;
            }
            else if (service == "commercial")
            {
                level.items = Level;
                subService.items = ComSub;
                // Maximum legitimate level is 3 (selectedIndex is level - 1)
                level.selectedIndex = Math.Max(level.selectedIndex, 2);
            }

            // Reset subservice and UI category on change.
            subService.selectedIndex = 0;
            UpdateUICategory();
        }

        public void UpdateUICategory()
        {
            // Updates UI category selection based on selected service and subservice.

            switch (service.selectedIndex)
            {
                case 0:
                    // None
                    uiCategory.selectedIndex = 15;
                    break;
                case 1:
                    // Residential.
                    switch (subService.selectedIndex)
                    {
                        case 0:
                            // High residential.
                            uiCategory.selectedIndex = 1;
                            break;
                        case 1:
                            // Low residential.
                            uiCategory.selectedIndex = 0;
                            break;
                        case 2:
                        case 3:
                            // High and low eco.
                            uiCategory.selectedIndex = 14;
                            break;
                    }
                    break;
                case 2:
                    // Industrial.
                    uiCategory.selectedIndex = subService.selectedIndex + 5;
                    // Reset level for specialised industry.
                    if (subService.selectedIndex > 0)
                    {
                        level.selectedIndex = 0;
                    }
                    break;
                case 3:
                    // Office.
                    switch (subService.selectedIndex)
                    {
                        case 0:
                            // Generic office.
                            uiCategory.selectedIndex = 4;
                            break;
                        case 1:
                            // IT cluster - also reset level.
                            uiCategory.selectedIndex = 13;
                            level.selectedIndex = 0;
                            break;
                    }
                    break;
                case 4:
                    // Commercial.
                    switch (subService.selectedIndex)
                    {
                        // For commercial, also set the correct available levels in the menus depending on specialisation.
                        case 0:
                            // High commercial.
                            uiCategory.selectedIndex = 3;
                            level.items = Level;
                            break;
                        case 1:
                            // Low commercial.
                            uiCategory.selectedIndex = 2;
                            subService.items = Level;
                            break;
                        default:
                            // Tourist, leisure or eco - also reset level.
                            uiCategory.selectedIndex = subService.selectedIndex + 8;
                            level.selectedIndex = 0;
                            level.items = extLevel;
                            break;
                    }
                    break;
                case 5:
                    // Extractor - also reset level.
                    level.selectedIndex = 0;
                    uiCategory.selectedIndex = subService.selectedIndex + 6;
                    break;
            }
        }
    }
}
