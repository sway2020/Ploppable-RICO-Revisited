using System;
using ColossalFramework.UI;
using UnityEngine;


namespace PloppableRICO
{
    /// <summary>
    ///The far right column of the settigns panel. Contains the drop downs and entry fields that allows players to assign RICO settings. 
    /// </summary>
    public class UIBuildingOptions : UIPanel
    {
        // Whole buncha UI strings.
        private string[] Service = new string[]
        {
            Translations.Translate("PRR_SRV_NON"),
            Translations.Translate("PRR_SRV_RES"),
            Translations.Translate("PRR_SRV_IND"),
            Translations.Translate("PRR_SRV_OFF"),
            Translations.Translate("PRR_SRV_COM"),
            Translations.Translate("PRR_SRV_EXT"),
            Translations.Translate("PRR_SRV_DUM")
        };

        private string[] OfficeSub = new string[]
        {
            Translations.Translate("PRR_SUB_GEN"),
            Translations.Translate("PRR_SUB_ITC")
        };

        private string[] ResSub = new string[]
        {
            Translations.Translate("PRR_SUB_HIG"),
            Translations.Translate("PRR_SUB_LOW"),
            Translations.Translate("PRR_SUB_HEC"),
            Translations.Translate("PRR_SUB_LEC")
        };

        private string[] ComSub = new string[]
        {
            Translations.Translate("PRR_SUB_HIG"),
            Translations.Translate("PRR_SUB_LOW"),
            Translations.Translate("PRR_SUB_LEI"),
            Translations.Translate("PRR_SUB_TOU"),
            Translations.Translate("PRR_SUB_ORG")
        };

        private string[] IndustrialSub = new string[]
        {
            Translations.Translate("PRR_SUB_GEN"),
            Translations.Translate("PRR_SUB_FAR"),
            Translations.Translate("PRR_SUB_FOR"),
            Translations.Translate("PRR_SUB_OIL"),
            Translations.Translate("PRR_SUB_ORE")
        };

        private string[] ExtractorSub = new string[]
        {
            Translations.Translate("PRR_SUB_FAR"),
            Translations.Translate("PRR_SUB_FOR"),
            Translations.Translate("PRR_SUB_OIL"),
            Translations.Translate("PRR_SUB_ORE")
        };

        private string[] DummySub = new string[]
        {
            Translations.Translate("PRR_SRV_NON")
        };

        private string[] Level = new string[]
        {
            "1",
            "2",
            "3",
        };

        private string[] resLevel = new string[]
        {
            "1",
            "2",
            "3",
            "4",
            "5"
        };

        private string[] extLevel = new string[]
        {
            "1"
        };

        // Flages.
        public bool disableEvents;

        // Selection.
        public RICOBuilding currentSelection;

        // Panel components.
        // Panel title.
        public UILabel label;
        public UIPanel labelpanel;

        // Enable RICO.
        public UICheckBox ricoEnabled;
        public UIPanel enableRICOPanel;

        // Growable.
        public UICheckBox growable;

        // Fundamental attributes.
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

        // Realistic population.
        public UICheckBox realityIgnored;
        //public UICheckBox manualWorkersEnabled;
        //public UIPanel manualPanel;

        // Construction.
        public UICheckBox constructionCostEnabled;
        public UIPanel constructionPanel;
        public UITextField construction;


        /// <summary>
        /// Performs initial setup for the panel; we no longer use Start() as that's not sufficiently reliable (race conditions), and is no longer needed, with the new create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Basic setup.
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "UnlockingPanel";

            // Layout.
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding.top = 5;
            autoLayoutPadding.right = 5;
            clipChildren = true;

            // Controls.
            builtinKeyNavigation = true;

            // Subpanels.
            labelpanel = this.AddUIComponent<UIPanel>();
            labelpanel.height = 20;

            // Title panel.
            label = labelpanel.AddUIComponent<UILabel>();
            label.relativePosition = new Vector3(80, 0);
            label.width = 270;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.text = Translations.Translate("PRR_SET_HASNON");

            // RICO enabled.
            ricoEnabled = UIUtils.CreateCheckBar(this, Translations.Translate("PRR_OPT_ENA"));
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

            // Dropdown menu - service.
            service = UIUtils.CreateDropDown(enableRICOPanel, 30, Translations.Translate("PRR_OPT_SER"));
            service.items = Service;
            service.selectedIndex = 0;
            service.eventSelectedIndexChanged += UpdateService;


            // Dropdown menu - sub-service.
            subService = UIUtils.CreateDropDown(enableRICOPanel, 60, Translations.Translate("PRR_OPT_SUB"));
            subService.selectedIndex = 0;
            subService.eventSelectedIndexChanged += UpdateSubService;

            // Dropdown menu - UI category.
            uiCategory = UIUtils.CreateDropDown(enableRICOPanel, 90, Translations.Translate("PRR_OPT_UIC"));
            uiCategory.selectedIndex = 0;
            uiCategory.items = (new UICategories()).names;

            // Dropdown menu - building level.
            level = UIUtils.CreateDropDown(enableRICOPanel, 120, Translations.Translate("PRR_LEVEL"));
            level.selectedIndex = 0;
            level.items = Level;

            // Update workplace allocations on level, service, and subservice change.
            level.eventSelectedIndexChanged += (control, value) =>
            {
                UpdateWorkplaceBreakdowns();
            };
            service.eventSelectedIndexChanged += (control, value) =>
            {
                UpdateWorkplaceBreakdowns();
            };
            subService.eventSelectedIndexChanged += (control, value) =>
            {
                UpdateWorkplaceBreakdowns();
            };

            // Base text fields.
            construction = UIUtils.CreateTextField(enableRICOPanel, 150, Translations.Translate("PRR_OPT_CST"));
            manual = UIUtils.CreateTextField(enableRICOPanel, 180, Translations.Translate("PRR_OPT_CNT"));

            // Base checkboxes.
            realityIgnored = UIUtils.CreateCheckBox(enableRICOPanel, 210, Translations.Translate("PRR_OPT_POP"));
            pollutionEnabled = UIUtils.CreateCheckBox(enableRICOPanel, 240, Translations.Translate("PRR_OPT_POL"));
            growable = UIUtils.CreateCheckBox(enableRICOPanel, 0, Translations.Translate("PRR_OPT_GRO"));

            // Workplace breakdown by education level.
            uneducated = UIUtils.CreateTextField(enableRICOPanel, 300, Translations.Translate("PRR_OPT_JB0"));
            educated = UIUtils.CreateTextField(enableRICOPanel, 330, Translations.Translate("PRR_OPT_JB1"));
            welleducated = UIUtils.CreateTextField(enableRICOPanel, 360, Translations.Translate("PRR_OPT_JB2"));
            highlyeducated = UIUtils.CreateTextField(enableRICOPanel, 390, Translations.Translate("PRR_OPT_JB3"));

            // Event handlers to update employment totals on change.
            manual.eventTextChanged += (control, value) => UpdateWorkplaceBreakdowns();
            uneducated.eventTextChanged += (control, value) => UpdateTotalJobs();
            educated.eventTextChanged += (control, value) => UpdateTotalJobs();
            welleducated.eventTextChanged += (control, value) => UpdateTotalJobs();
            highlyeducated.eventTextChanged += (control, value) => UpdateTotalJobs();
        }


        /// <summary>
        /// Event handler - updates the options panel when the service dropdown is changed.
        /// </summary>
        /// <param name="component">Calling component (ignored)</param>
        /// <param name="value">New service dropdown selected index</param>
        public void UpdateService(UIComponent component, int value)
        {
            // Ignore event if disabled flag is set.
            if (!disableEvents)
            {
                // Translate index to relevant UpdateElements parameter.
                switch(value)
                {
                    case 0:
                        UpdateElements("none");
                        break;
                    case 1:
                        UpdateElements("residential");
                        break;
                    case 2:
                        UpdateElements("industrial");
                        break;
                    case 3:
                        UpdateElements("office");
                        break;
                    case 4:
                        UpdateElements("commercial");
                        break;
                    case 5:
                        UpdateElements("extractor");
                        break;
                    case 6:
                        UpdateElements("dummy");
                        break;
                }
            }
        }


        /// <summary>
        /// Event handler - updates the options panel when the sub-service dropdown is changed.
        /// </summary>
        /// <param name="component">Calling component (ignored)</param>
        /// <param name="value">New service dropdown selected index (ignored)</param>
        public void UpdateSubService(UIComponent component, int value)
        {
            // Ignore event if disabled flag is set.
            if (!disableEvents)
            {
                UpdateUICategory();
            }
        }


        /// <summary>
        /// Updates the total workplaces textfield with the sum of the workplace breakdown boxes.
        /// Does nothing if any workplace textfield cannot be parsed directly to int.
        /// </summary>
        public void UpdateTotalJobs()
        {
            // Ignore event if disabled flag is set.
            if (!disableEvents)
            {
                // Disable events while we update, to avoid an infinite loop.
                disableEvents = true;

                // Try to add up all workplace breakdown fields and update the total.  If an exception is thrown (most likely parsing error) then just do nothing.
                try
                {
                    manual.text = (int.Parse(uneducated.text) + int.Parse(educated.text) + int.Parse(welleducated.text) + int.Parse(highlyeducated.text)).ToString();
                }
                catch
                {
                    // Don't care.
                }

                // Resume event handling.
                disableEvents = false;
            }
        }



        /// <summary>
        /// Reads current settings from UI elements, and saves them to XML.
        /// </summary>
        internal void SaveRICO()
        {
            // Set service and subservice.
            GetService(out string serviceString, out string subServiceString);
            currentSelection.service = serviceString;
            currentSelection.subService = subServiceString;

            // Set level.
            currentSelection.level = level.selectedIndex + 1;

            // Get home/total worker count, with default of zero.
            int manualCount = 0;
            int.TryParse(manual.text, out manualCount);
            currentSelection.homeCount = manualCount;

            // Get workplace breakdown.
            int[] a = new int[4] { 0, 0, 0, 0 };
            int.TryParse(uneducated.text, out a[0]);
            int.TryParse(educated.text, out a[1]);
            int.TryParse(welleducated.text, out a[2]);
            int.TryParse(highlyeducated.text, out a[3]);

            // If no breakdown has been provided, then we try the total jobs instead.
            // Yeah, it's a bit clunky to add the elements individually like this, but saves bringing in System.Linq for just this one case.
            if (a[0] + a[1] + a[2] + a[3] == 0)
            {
                // No workplace breakdown provided (all fields zero); use total workplaces ('manual', previously parsed as manualCount) and allocate.
                int[] d = Util.WorkplaceDistributionOf(currentSelection.service, currentSelection.subService, "Level" + currentSelection.level);
                a = WorkplaceAIHelper.distributeWorkplaceLevels(manualCount, d, new int[] { 0, 0, 0, 0 });

                // Check and adjust for any rounding errors, assigning 'leftover' jobs to the lowest education level.
                a[0] += (manualCount - a[0] - a[1] - a[2] - a[3]);
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

            // UI categories from menu.
            switch (uiCategory.selectedIndex)
            {
                case 0:
                    currentSelection.uiCategory = "reslow";
                    break;
                case 1:
                    currentSelection.uiCategory = "reshigh";
                    break;
                case 2:
                    currentSelection.uiCategory = "comlow";
                    break;
                case 3:
                    currentSelection.uiCategory = "comhigh";
                    break;
                case 4:
                    currentSelection.uiCategory = "office";
                    break;
                case 5:
                    currentSelection.uiCategory = "industrial";
                    break;
                case 6:
                    currentSelection.uiCategory = "farming";
                    break;
                case 7:
                    currentSelection.uiCategory = "forest";
                    break;
                case 8:
                    currentSelection.uiCategory = "oil";
                    break;
                case 9:
                    currentSelection.uiCategory = "ore";
                    break;
                case 10:
                    currentSelection.uiCategory = "leisure";
                    break;
                case 11:
                    currentSelection.uiCategory = "tourist";
                    break;
                case 12:
                    currentSelection.uiCategory = "organic";
                    break;
                case 13:
                    currentSelection.uiCategory = "hightech";
                    break;
                case 14:
                    currentSelection.uiCategory = "selfsufficient";
                    break;
                default:
                    currentSelection.uiCategory = "none";
                    break;
            }

            // Remaining items.
            currentSelection.ricoEnabled = ricoEnabled.isChecked;
            currentSelection.growable = growable.isChecked;
            currentSelection.RealityIgnored = !realityIgnored.isChecked;
            currentSelection.pollutionEnabled = pollutionEnabled.isChecked;
        }


        /// <summary>
        /// Updates the options panel when the building selection changes, including showing/hiding relevant controls.
        /// </summary>
        /// <param name="buildingData">RICO building data</param>
        internal void SelectionChanged(BuildingData buildingData)
        {
            // Disable the event logic while dropdowns are being updated.
            disableEvents = true;

            // Disable all input controls by default; activate them later if needed.
            ricoEnabled.Disable();
            growable.Disable();
            growable.parent.Hide();
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
                label.text = Translations.Translate("PRR_SET_HASLOC");

                // If the building has local settings, enable input fields.
                ricoEnabled.Enable();
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

                // 'Growable' can only be set in local settings.
                // Only show growable checkbox where assets meet the prequisites:
                // Growables can't have any dimension greater than 4 or contain any net structures.
                if (buildingData.prefab.GetWidth() <= 4 && buildingData.prefab.GetLength() <= 4 && !(buildingData.prefab.m_paths != null && buildingData.prefab.m_paths.Length != 0))
                {
                    growable.Enable();
                    growable.parent.Show();
                }

            }
            else if (buildingData.hasAuthor)
            {
                // If the building has author settings, then disable input fields.
                currentSelection = buildingData.author;
                UpdateElements(buildingData.author.service);
                UpdateValues(buildingData.author);
                label.text = Translations.Translate("PRR_SET_HASAUT");
            }
            else if (buildingData.hasMod)
            {
                // If the building has mod settings, then disable input fields.
                currentSelection = buildingData.mod;
                label.text = Translations.Translate("PRR_SET_HASMOD");
                UpdateElements(buildingData.mod.service);
                UpdateValues(buildingData.mod);
            }
            else
            {
                // Fallback - building has no Ploppable RICO data anywhere, disable Ploppable RICO.
                ricoEnabled.isChecked = false;
                ricoEnabled.Disable();
                label.text = Translations.Translate("PRR_SET_HASNON");
            }

            // Re-enable event logic now that dropdowns are up-to-date before returning.
            disableEvents = false;
        }



        /// <summary>
        /// Updates the values in the RICO options panel to match the selected building (control visibility should already be set).
        /// </summary>
        /// <param name="buildingData">RICO building record</param>
        internal void UpdateValues(RICOBuilding building)
        {
            // Updates the values in the RICO options panel to match the selected building.

            // Workplaces.
            manual.text = building.workplaceCount.ToString();
            uneducated.text = building.workplaces[0].ToString();
            educated.text = building.workplaces[1].ToString();
            welleducated.text = building.workplaces[2].ToString();
            highlyeducated.text = building.workplaces[3].ToString();

            // Service and sub-service.
            switch (building.service)
            {
                case "residential":

                    manual.text = building.homeCount.ToString();
                    service.selectedIndex = 1;

                    if (currentSelection.subService == "high") subService.selectedIndex = 0;
                    else if (currentSelection.subService == "low") subService.selectedIndex = 1;
                    else if (currentSelection.subService == "high eco") subService.selectedIndex = 2;
                    else if (currentSelection.subService == "low eco") subService.selectedIndex = 3;

                    break;

                case "industrial":

                    service.selectedIndex = 2;
                    subService.items = IndustrialSub;

                    if (currentSelection.subService == "generic") subService.selectedIndex = 0;
                    else if (currentSelection.subService == "farming") subService.selectedIndex = 1;
                    else if (currentSelection.subService == "forest") subService.selectedIndex = 2;
                    else if (currentSelection.subService == "oil") subService.selectedIndex = 3;
                    else if (currentSelection.subService == "ore") subService.selectedIndex = 4;

                    break;

                case "office":

                    service.selectedIndex = 3;
                    subService.items = OfficeSub;

                    if (currentSelection.subService == "none") subService.selectedIndex = 0;
                    else if (currentSelection.subService == "high tech") subService.selectedIndex = 1;
                    break;

                case "commercial":

                    service.selectedIndex = 4;
                    subService.items = ComSub;

                    if (currentSelection.subService == "high") subService.selectedIndex = 0;
                    else if (currentSelection.subService == "low") subService.selectedIndex = 1;
                    else if (currentSelection.subService == "leisure") subService.selectedIndex = 2;
                    else if (currentSelection.subService == "tourist") subService.selectedIndex = 3;
                    else if (currentSelection.subService == "eco") subService.selectedIndex = 4;
                    break;

                case "extractor":

                    service.selectedIndex = 5;
                    subService.items = ExtractorSub;

                    if (currentSelection.subService == "farming") subService.selectedIndex = 0;
                    else if (currentSelection.subService == "forest") subService.selectedIndex = 1;
                    else if (currentSelection.subService == "oil") subService.selectedIndex = 2;
                    else if (currentSelection.subService == "ore") subService.selectedIndex = 3;

                    break;

                case "dummy":

                    service.selectedIndex = 6;
                    subService.selectedIndex = 0;
                    subService.items = DummySub;

                    break;

                default:

                    service.selectedIndex = 0;
                    subService.selectedIndex = 0;
                    subService.items = DummySub;
                    break;
            }

            // UI category.
            switch (building.uiCategory)
            {
                case "reslow":
                    uiCategory.selectedIndex = 0;
                    break;
                case "reshigh":
                    uiCategory.selectedIndex = 1;
                    break;
                case "comlow":
                    uiCategory.selectedIndex = 2;
                    break;
                case "comhigh":
                    uiCategory.selectedIndex = 3;
                    break;
                case "office":
                    uiCategory.selectedIndex = 4;
                    break;
                case "industrial":
                    uiCategory.selectedIndex = 5;
                    break;
                case "farming":
                    uiCategory.selectedIndex = 6;
                    break;
                case "forest":
                    uiCategory.selectedIndex = 7;
                    break;
                case "oil":
                    uiCategory.selectedIndex = 8;
                    break;
                case "ore":
                    uiCategory.selectedIndex = 9;
                    break;
                case "leisure":
                    uiCategory.selectedIndex = 10;
                    break;
                case "tourist":
                    uiCategory.selectedIndex = 11;
                    break;
                case "organic":
                    uiCategory.selectedIndex = 12;
                    break;
                case "hightech":
                    uiCategory.selectedIndex = 13;
                    break;
                case "selfsufficient":
                    uiCategory.selectedIndex = 14;
                    break;
                default:
                    uiCategory.selectedIndex = 15;
                    break;
            }

            // Building level.
            level.selectedIndex = (building.level - 1);

            // Construction cost.
            construction.text = building.constructionCost.ToString();

            // Use realistic population.
            realityIgnored.isChecked = !building.RealityIgnored;

            // Pollution enabled
            pollutionEnabled.isChecked = building.pollutionEnabled;

            // Growable.
            growable.isChecked = building.growable;

            // Enable RICO.
            ricoEnabled.isChecked = building.ricoEnabled;
        }


        /// <summary>
        /// Reconfigures the RICO options panel to display relevant options for a given service.
        /// This simply hides/shows different option fields for the various services.
        /// </summary>
        /// <param name="service">RICO service</param>
        internal void UpdateElements(string service)
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

            switch (service)
            {
                case "residential":
                    level.items = resLevel;
                    subService.items = ResSub;

                    // No workplaces breakdown for residential - hide them.
                    uneducated.parent.Hide();
                    educated.parent.Hide();
                    welleducated.parent.Hide();
                    highlyeducated.parent.Hide();
                    break;

                case "office":
                    level.items = Level;
                    subService.items = OfficeSub;

                    // Maximum legitimate level is 3 (selectedIndex is level - 1)
                    level.selectedIndex = Math.Min(level.selectedIndex, 2);
                    break;

                case "industrial":
                    level.items = Level;
                    subService.items = IndustrialSub;

                    // Industries can pollute.
                    pollutionEnabled.enabled = true;
                    pollutionEnabled.parent.Show();

                    // Maximum legitimate level is 3 (selectedIndex is level - 1)
                    level.selectedIndex = Math.Min(level.selectedIndex, 2);
                    break;

                case "extractor":
                    level.items = extLevel;
                    subService.items = ExtractorSub;

                    // Extractors can pollute.
                    pollutionEnabled.enabled = true;

                    // Maximum legitimate level is 1 (selectedIndex is level - 1)
                    level.selectedIndex = 0;
                    break;

                case "commercial":
                    level.items = Level;
                    subService.items = ComSub;

                    // Maximum legitimate level is 3 (selectedIndex is level - 1)
                    level.selectedIndex = Math.Min(level.selectedIndex, 2);
                    break;

                default:
                    level.items = extLevel;
                    subService.items = DummySub;
                    break;
            }

            // Reset subservice and UI category on change.
            subService.selectedIndex = 0;
            UpdateUICategory();
        }


        /// <summary>
        /// Automatically pdates UI category selection based on selected service and subservice.
        /// </summary>
        internal void UpdateUICategory()
        {
            switch (service.selectedIndex)
            {
                case 0:
                    // None - also reset level.
                    level.selectedIndex = 0;
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
                        level.items = extLevel;
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
                            level.items = extLevel;
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
                            level.items = Level;
                            break;
                        default:
                            // Tourist, leisure or eco - also reset level.
                            uiCategory.selectedIndex = subService.selectedIndex + 8;
                            level.items = extLevel;
                            level.selectedIndex = 0;
                            break;
                    }
                    break;
                case 5:
                    // Extractor - also reset level.
                    level.selectedIndex = 0;
                    uiCategory.selectedIndex = subService.selectedIndex + 6;
                    break;
                case 6:
                    // Dummy - also reset level.
                    level.selectedIndex = 0;
                    uiCategory.selectedIndex = 15;
                    break;
            }
        }


        /// <summary>
        /// Returns the current service and subservice based on current menu selections.
        /// </summary>
        private void GetService(out string serviceName, out string subServiceName)
        {
            // Default return value for subservice if we can't match it below.
            subServiceName = "none";

            switch (service.selectedIndex)
            {
                case 0:
                    serviceName = "none";
                    subServiceName = "none";
                    break;
                case 1:
                    serviceName = "residential";
                    switch (subService.selectedIndex)
                    {
                        case 0:
                            subServiceName = "high";
                            break;
                        case 1:
                            subServiceName = "low";
                            break;
                        case 2:
                            subServiceName = "high eco";
                            break;
                        case 3:
                            subServiceName = "low eco";
                            break;
                    }
                    break;

                case 2:
                    serviceName = "industrial";
                    switch (subService.selectedIndex)
                    {
                        case 0:
                            subServiceName = "generic";
                            break;
                        case 1:
                            subServiceName = "farming";
                            break;
                        case 2:
                            subServiceName = "forest";
                            break;
                        case 3:
                            subServiceName = "oil";
                            break;
                        case 4:
                            subServiceName = "ore";
                            break;
                    }
                    break;

                case 3:
                    serviceName = "office";
                    if (subService.selectedIndex == 0) subServiceName = "none";
                    else if (subService.selectedIndex == 1) subServiceName = "high tech";
                    break;

                case 4:
                    serviceName = "commercial";
                    switch (subService.selectedIndex)
                    {
                        case 0:
                            subServiceName = "high";
                            break;
                        case 1:
                            subServiceName = "low";
                            break;
                        case 2:
                            subServiceName = "leisure";
                            break;
                        case 3:
                            subServiceName = "tourist";
                            break;
                        case 4:
                            subServiceName = "eco";
                            break;
                    }
                    break;

                case 5:
                    serviceName = "extractor";
                    switch (subService.selectedIndex)
                    {
                        case 0:
                            subServiceName = "farming";
                            break;
                        case 1:
                            subServiceName = "forest";
                            break;
                        case 2:
                            subServiceName = "oil";
                            break;
                        case 3:
                            subServiceName = "ore";
                            break;
                    }
                    break;

                default:
                    serviceName = "dummy";
                    subServiceName = "none";
                    break;
            }
        }


        /// <summary>
        /// Updates workplace breakdowns to ratios applicable to current settings.
        /// </summary>
        private void UpdateWorkplaceBreakdowns()
        {
            int[] allocation = new int[4];
            int totalJobs;


            // Ignore event if disabled flag is set.
            if (disableEvents)
            {
                return;
            }

            // If we catch an exception while parsing the manual textfield, it's probably because it's not ready yet (initial asset selection).
            // Simply return without doing anything.
            try
            {
                totalJobs = int.Parse(manual.text);
            }
            catch
            {
                return;
            }

            if (totalJobs > 0)
            {
                // Get current service and sub-service.
                GetService(out string serviceString, out string subServiceString);

                // Allocate out total workplaces ('manual').
                int[] distribution = Util.WorkplaceDistributionOf(serviceString, subServiceString, "Level" + (level.selectedIndex + 1));
                allocation = WorkplaceAIHelper.distributeWorkplaceLevels(int.Parse(manual.text), distribution, new int[] { 0, 0, 0, 0 });

                // Check and adjust for any rounding errors, assigning 'leftover' jobs to the lowest education level.
                allocation[0] += (int.Parse(manual.text) - allocation[0] - allocation[1] - allocation[2] - allocation[3]);
            }

            // Disable event handling while we update textfields.
            disableEvents = true;

            // Update workplace textfields.
            uneducated.text = allocation[0].ToString();
            educated.text = allocation[1].ToString();
            welleducated.text = allocation[2].ToString();
            highlyeducated.text = allocation[3].ToString();

            // Resume event handling.
            disableEvents = false;
        }
    }
}
