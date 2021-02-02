using System;
using ColossalFramework.UI;
using UnityEngine;


namespace PloppableRICO
{
    /// <summary>
    ///The far right column of the settings panel. Contains the drop downs and entry fields that allows players to assign RICO settings. 
    /// </summary>
    public class UIBuildingOptions : UIPanel
    {
        // Whole buncha UI strings.
        private readonly string[] Service = new string[]
        {
            Translations.Translate("PRR_SRV_NON"),
            Translations.Translate("PRR_SRV_RES"),
            Translations.Translate("PRR_SRV_IND"),
            Translations.Translate("PRR_SRV_OFF"),
            Translations.Translate("PRR_SRV_COM"),
            Translations.Translate("PRR_SRV_EXT"),
            Translations.Translate("PRR_SRV_DUM")
        };

        private readonly string[] OfficeSub = new string[]
        {
            Translations.Translate("PRR_SUB_GEN"),
            Translations.Translate("PRR_SUB_ITC")
        };

        private readonly string[] ResSub = new string[]
        {
            Translations.Translate("PRR_SUB_HIG"),
            Translations.Translate("PRR_SUB_LOW"),
            Translations.Translate("PRR_SUB_HEC"),
            Translations.Translate("PRR_SUB_LEC")
        };

        private readonly string[] ComSub = new string[]
        {
            Translations.Translate("PRR_SUB_HIG"),
            Translations.Translate("PRR_SUB_LOW"),
            Translations.Translate("PRR_SUB_LEI"),
            Translations.Translate("PRR_SUB_TOU"),
            Translations.Translate("PRR_SUB_ORG")
        };

        private readonly string[] IndustrialSub = new string[]
        {
            Translations.Translate("PRR_SUB_GEN"),
            Translations.Translate("PRR_SUB_FAR"),
            Translations.Translate("PRR_SUB_FOR"),
            Translations.Translate("PRR_SUB_OIL"),
            Translations.Translate("PRR_SUB_ORE")
        };

        private readonly string[] ExtractorSub = new string[]
        {
            Translations.Translate("PRR_SUB_FAR"),
            Translations.Translate("PRR_SUB_FOR"),
            Translations.Translate("PRR_SUB_OIL"),
            Translations.Translate("PRR_SUB_ORE")
        };

        private readonly string[] DummySub = new string[]
        {
            Translations.Translate("PRR_SRV_NON")
        };

        private readonly string[] Level = new string[]
        {
            "1",
            "2",
            "3",
        };

        private readonly string[] resLevel = new string[]
        {
            "1",
            "2",
            "3",
            "4",
            "5"
        };

        private readonly string[] extLevel = new string[]
        {
            "1"
        };

        // Flages.
        private bool disableEvents;

        // Selection.
        private BuildingData currentBuildingData;
        private RICOBuilding currentSettings;

        // Panel components.
        // Panel title.
        private UILabel label;
        private UIPanel labelpanel;

        // Settings selection.
        private UIDropDown settingDropDown;

        // Enable RICO.
        private UICheckBox ricoEnabled;
        private UIPanel enableRICOPanel;

        // Growable.
        private UICheckBox growable;

        // Fundamental attributes.
        private UIDropDown service, subService, level, uiCategory;

        // Households / workplaces.
        private UITextField manual, uneducated, educated, welleducated, highlyeducated;

        // Pollution.
        private UICheckBox pollutionEnabled;

        // Realistic population.
        private UICheckBox realityIgnored;

        // Construction.
        private UITextField construction;


        /// <summary>
        /// Updates the options panel when the building selection changes, including showing/hiding relevant controls.
        /// </summary>
        /// <param name="buildingData">RICO building data</param>
        internal void SelectionChanged(BuildingData buildingData)
        {
            // Set current data.
            currentBuildingData = buildingData;

            // Set displayed settings via settings dropdown menu event handler.
            if (buildingData.hasLocal)
            {
                // Local settings have priority - select them if they exist.
                settingDropDown.selectedIndex = 0;
            }
            else if (buildingData.hasAuthor)
            {
                // Then author settings - select them if no local settings.
                settingDropDown.selectedIndex = 1;
            }
            else if (buildingData.hasMod)
            {
                // Finally, set mod settings if no other settings.
                settingDropDown.selectedIndex = 2;
            }
            else
            {
                // No settings are available for this builidng - default to local.
                settingDropDown.selectedIndex = 0;
            }
        }


        /// <summary>
        /// Reads current settings from UI elements, and saves them to XML.
        /// </summary>
        internal void SaveRICO()
        {
            // Set service and subservice.
            GetService(out string serviceString, out string subServiceString);
            currentSettings.service = serviceString;
            currentSettings.subService = subServiceString;

            // Set level.
            currentSettings.level = level.selectedIndex + 1;

            // Get home/total worker count, with default of zero.
            int.TryParse(manual.text, out int manualCount);
            currentSettings.homeCount = manualCount;

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
                int[] d = Util.WorkplaceDistributionOf(currentSettings.service, currentSettings.subService, "Level" + currentSettings.level);
                a = WorkplaceAIHelper.DistributeWorkplaceLevels(manualCount, d);

                // Check and adjust for any rounding errors, assigning 'leftover' jobs to the lowest education level.
                a[0] += (manualCount - a[0] - a[1] - a[2] - a[3]);
            }

            currentSettings.Workplaces = a;

            currentSettings.ConstructionCost = int.Parse(construction.text);
            // Write parsed (and filtered, e.g. minimum value 10) back to the construction cost text field so the user knows.
            construction.text = currentSettings.ConstructionCost.ToString();

            // UI categories from menu.
            switch (uiCategory.selectedIndex)
            {
                case 0:
                    currentSettings.UiCategory = "reslow";
                    break;
                case 1:
                    currentSettings.UiCategory = "reshigh";
                    break;
                case 2:
                    currentSettings.UiCategory = "comlow";
                    break;
                case 3:
                    currentSettings.UiCategory = "comhigh";
                    break;
                case 4:
                    currentSettings.UiCategory = "office";
                    break;
                case 5:
                    currentSettings.UiCategory = "industrial";
                    break;
                case 6:
                    currentSettings.UiCategory = "farming";
                    break;
                case 7:
                    currentSettings.UiCategory = "forest";
                    break;
                case 8:
                    currentSettings.UiCategory = "oil";
                    break;
                case 9:
                    currentSettings.UiCategory = "ore";
                    break;
                case 10:
                    currentSettings.UiCategory = "leisure";
                    break;
                case 11:
                    currentSettings.UiCategory = "tourist";
                    break;
                case 12:
                    currentSettings.UiCategory = "organic";
                    break;
                case 13:
                    currentSettings.UiCategory = "hightech";
                    break;
                case 14:
                    currentSettings.UiCategory = "selfsufficient";
                    break;
                default:
                    currentSettings.UiCategory = "none";
                    break;
            }

            // Remaining items.
            currentSettings.ricoEnabled = ricoEnabled.isChecked;
            currentSettings.growable = growable.isChecked;
            currentSettings.RealityIgnored = !realityIgnored.isChecked;
            currentSettings.pollutionEnabled = pollutionEnabled.isChecked;
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
            labelpanel.height = 20f;

            // Title panel.
            label = labelpanel.AddUIComponent<UILabel>();
            label.relativePosition = new Vector3(80f, 0f);
            label.width = 270f;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.text = Translations.Translate("PRR_SET_HASNON");

            // Setting selection dropdown.
            settingDropDown = RICODropDown(this, 30f, Translations.Translate("PRR_OPT_SET"));
            settingDropDown.items = new String[] { Translations.Translate("PRR_OPT_LOC"), Translations.Translate("PRR_OPT_AUT"), Translations.Translate("PRR_OPT_MOD") };
            // Set selected index to -1 to ensure correct application of initial settings via event handler.
            settingDropDown.selectedIndex = -1;
            settingDropDown.eventSelectedIndexChanged += UpdateSettingSelection;

            // RICO enabled.
            ricoEnabled = RICOCheckBar(this, Translations.Translate("PRR_OPT_ENA"), this.width);
            enableRICOPanel = this.AddUIComponent<UIPanel>();
            enableRICOPanel.height = 0;
            enableRICOPanel.isVisible = false;
            enableRICOPanel.name = "OptionsPanel";
            ricoEnabled.Disable();

            ricoEnabled.eventCheckChanged += (control, isEnabled) =>
            {
                // Show RICO options panel if enabled and there's a valid current selection.
                if (isEnabled)
                {
                    enableRICOPanel.height = 240f;
                    enableRICOPanel.isVisible = true;
                }
                else
                {
                    enableRICOPanel.height = 0f;
                    enableRICOPanel.isVisible = false;
                }
            };

            // Dropdown menu - service.
            service = RICODropDown(enableRICOPanel, 30f, Translations.Translate("PRR_OPT_SER"));
            service.items = Service;
            service.selectedIndex = 0;
            service.eventSelectedIndexChanged += UpdateService;

            // Dropdown menu - sub-service.
            subService = RICODropDown(enableRICOPanel, 60f, Translations.Translate("PRR_OPT_SUB"));
            subService.selectedIndex = 0;
            subService.eventSelectedIndexChanged += UpdateSubService;

            // Dropdown menu - UI category.
            uiCategory = RICODropDown(enableRICOPanel, 90f, Translations.Translate("PRR_OPT_UIC"));
            uiCategory.selectedIndex = 0;
            uiCategory.items = (new UICategories()).names;

            // Dropdown menu - building level.
            level = RICODropDown(enableRICOPanel, 120f, Translations.Translate("PRR_LEVEL"));
            level.selectedIndex = 0;
            level.items = Level;

            // Update workplace allocations on level, service, and subservice change.
            level.eventSelectedIndexChanged += (control, value) => UpdateWorkplaceBreakdowns();
            service.eventSelectedIndexChanged += (control, value) => UpdateWorkplaceBreakdowns();
            subService.eventSelectedIndexChanged += (control, value) => UpdateWorkplaceBreakdowns();

            // Base text fields.
            construction = UIUtils.CreateTextField(enableRICOPanel, 150f, Translations.Translate("PRR_OPT_CST"));
            manual = UIUtils.CreateTextField(enableRICOPanel, 180f, Translations.Translate("PRR_OPT_CNT"));

            // Base checkboxes.
            growable = RICOLabelledCheckBox(enableRICOPanel, 0f, Translations.Translate("PRR_OPT_GRO"));
            pollutionEnabled = RICOLabelledCheckBox(enableRICOPanel, 210f, Translations.Translate("PRR_OPT_POL"));
            realityIgnored = RICOLabelledCheckBox(enableRICOPanel, 240f, Translations.Translate("PRR_OPT_POP"));

            // Workplace breakdown by education level.
            uneducated = UIUtils.CreateTextField(enableRICOPanel, 300f, Translations.Translate("PRR_OPT_JB0"));
            educated = UIUtils.CreateTextField(enableRICOPanel, 330f, Translations.Translate("PRR_OPT_JB1"));
            welleducated = UIUtils.CreateTextField(enableRICOPanel, 360f, Translations.Translate("PRR_OPT_JB2"));
            highlyeducated = UIUtils.CreateTextField(enableRICOPanel, 390f, Translations.Translate("PRR_OPT_JB3"));

            // Event handlers to update employment totals on change.
            manual.eventTextChanged += (control, value) => UpdateWorkplaceBreakdowns();
            uneducated.eventTextChanged += (control, value) => UpdateTotalJobs();
            educated.eventTextChanged += (control, value) => UpdateTotalJobs();
            welleducated.eventTextChanged += (control, value) => UpdateTotalJobs();
            highlyeducated.eventTextChanged += (control, value) => UpdateTotalJobs();

            // Event handler for realistic population checkbox to toggle state of population textfields.
            realityIgnored.eventCheckChanged += SetTextfieldState;
        }


        /// <summary>
        /// Event handler - handles changes to settings selection.
        /// </summary>
        /// <param name="component">Calling UI component (unused)</param>
        /// <param name="index">Settings menu index</param>
        private void UpdateSettingSelection(UIComponent component, int index)
        {
            // Disable the event logic while dropdowns are being updated.
            disableEvents = true;

            // Combination 'no settings' message text and status flag (left as null if valid settings are selected). 
            string noSettingMessage = null;

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

            // Update UI components based on current setting selection.
            switch (index)
            {
                // Local settings.
                case 0:
                    // Does the current building have local settings?
                    if (currentBuildingData.hasLocal)
                    {
                        // Yes - update display.
                        currentSettings = currentBuildingData.local;
                        UpdateElements(currentBuildingData.local.service);
                        UpdateValues(currentBuildingData.local);
                        label.text = Translations.Translate("PRR_SET_HASLOC");

                        // (Re)enable input fields.
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
                        if (currentBuildingData.prefab.GetWidth() <= 4 && currentBuildingData.prefab.GetLength() <= 4 && !(currentBuildingData.prefab.m_paths != null && currentBuildingData.prefab.m_paths.Length != 0))
                        {
                            growable.Enable();
                            growable.parent.Show();
                        }

                    }
                    else
                    {
                        // No local settings for this building.
                        noSettingMessage = Translations.Translate("No local settings");
                    }
                    break;

                // Author settings.
                case 1:
                    // Does the current building have author settings?
                    if (currentBuildingData.hasAuthor)
                    {
                        // Yes - leave input fields disabled and update display.
                        currentSettings = currentBuildingData.author;
                        UpdateElements(currentBuildingData.author.service);
                        UpdateValues(currentBuildingData.author);
                        label.text = Translations.Translate("PRR_SET_HASAUT");
                    }
                    else
                    {
                        // No author settings for this building.
                        noSettingMessage = Translations.Translate("No author settings");
                    }
                    break;

                // Mod settings.
                case 2:
                    // Does the current building have mod settings?
                    if (currentBuildingData.hasMod)
                    {
                        // Yes - leave input fields disabled and update display.
                        currentSettings = currentBuildingData.mod;
                        label.text = Translations.Translate("PRR_SET_HASMOD");
                        UpdateElements(currentBuildingData.mod.service);
                        UpdateValues(currentBuildingData.mod);
                    }
                    else
                    {
                        // No mod settings for this building.
                        noSettingMessage = Translations.Translate("No mod settings");
                    }
                    break;

                default:
                    Logging.Error("invalid settting index ", index.ToString());
                    noSettingMessage = Translations.Translate("Invalid service selection");
                    break;
            }

            if (!string.IsNullOrEmpty(noSettingMessage))
            {
                ricoEnabled.isChecked = false;
                ricoEnabled.Disable();
                label.text = noSettingMessage;
            }

            // Restore event logic.
            disableEvents = false;
        }


        /// <summary>
        /// Event handler - updates the options panel when the service dropdown is changed.
        /// </summary>
        /// <param name="component">Calling component (ignored)</param>
        /// <param name="value">New service dropdown selected index</param>
        private void UpdateService(UIComponent component, int value)
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
        private void UpdateSubService(UIComponent component, int value)
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
        private void UpdateTotalJobs()
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
        /// Updates the values in the RICO options panel to match the selected building (control visibility should already be set).
        /// </summary>
        /// <param name="buildingData">RICO building record</param>
        private void UpdateValues(RICOBuilding building)
        {
            // Workplaces.
            manual.text = building.WorkplaceCount.ToString();
            uneducated.text = building.Workplaces[0].ToString();
            educated.text = building.Workplaces[1].ToString();
            welleducated.text = building.Workplaces[2].ToString();
            highlyeducated.text = building.Workplaces[3].ToString();

            // Service and sub-service.
            switch (building.service)
            {
                case "residential":

                    manual.text = building.homeCount.ToString();
                    service.selectedIndex = 1;

                    if (currentSettings.subService == "high") subService.selectedIndex = 0;
                    else if (currentSettings.subService == "low") subService.selectedIndex = 1;
                    else if (currentSettings.subService == "high eco") subService.selectedIndex = 2;
                    else if (currentSettings.subService == "low eco") subService.selectedIndex = 3;

                    break;

                case "industrial":

                    service.selectedIndex = 2;
                    subService.items = IndustrialSub;

                    if (currentSettings.subService == "generic") subService.selectedIndex = 0;
                    else if (currentSettings.subService == "farming") subService.selectedIndex = 1;
                    else if (currentSettings.subService == "forest") subService.selectedIndex = 2;
                    else if (currentSettings.subService == "oil") subService.selectedIndex = 3;
                    else if (currentSettings.subService == "ore") subService.selectedIndex = 4;

                    break;

                case "office":

                    service.selectedIndex = 3;
                    subService.items = OfficeSub;

                    if (currentSettings.subService == "none") subService.selectedIndex = 0;
                    else if (currentSettings.subService == "high tech") subService.selectedIndex = 1;
                    break;

                case "commercial":

                    service.selectedIndex = 4;
                    subService.items = ComSub;

                    if (currentSettings.subService == "high") subService.selectedIndex = 0;
                    else if (currentSettings.subService == "low") subService.selectedIndex = 1;
                    else if (currentSettings.subService == "leisure") subService.selectedIndex = 2;
                    else if (currentSettings.subService == "tourist") subService.selectedIndex = 3;
                    else if (currentSettings.subService == "eco") subService.selectedIndex = 4;
                    break;

                case "extractor":

                    service.selectedIndex = 5;
                    subService.items = ExtractorSub;

                    if (currentSettings.subService == "farming") subService.selectedIndex = 0;
                    else if (currentSettings.subService == "forest") subService.selectedIndex = 1;
                    else if (currentSettings.subService == "oil") subService.selectedIndex = 2;
                    else if (currentSettings.subService == "ore") subService.selectedIndex = 3;

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
            switch (building.UiCategory)
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
            construction.text = building.ConstructionCost.ToString();

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
        private void UpdateElements(string service)
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
        /// Updates the textfield state depending on the state of the 'use realistic population' checkbox state.
        /// </summary>
        private void SetTextfieldState(UIComponent control, bool useReality)
        {
            if (useReality)
            {
                // Using Realistic Population - disable population textfields.
                manual.Disable();
                uneducated.Disable();
                educated.Disable();
                welleducated.Disable();
                highlyeducated.Disable();

                // Set explanatory tooltip.
                string tooltip = Translations.Translate("PRR_OPT_URP");
                manual.tooltip = tooltip;
                uneducated.tooltip = tooltip;
                educated.tooltip = tooltip;
                welleducated.tooltip = tooltip;
                highlyeducated.tooltip = tooltip;
            }
            else
            {
                // Not using Realistic Population - enable population textfields.
                manual.Enable();
                uneducated.Enable();
                educated.Enable();
                welleducated.Enable();
                highlyeducated.Enable();
                
                // Set default tooltips.
                manual.tooltip = Translations.Translate("PRR_OPT_CNT");
                uneducated.tooltip = Translations.Translate("PRR_OPT_JB0");
                educated.tooltip = Translations.Translate("PRR_OPT_JB1");
                welleducated.tooltip = Translations.Translate("PRR_OPT_JB2");
                highlyeducated.tooltip = Translations.Translate("PRR_OPT_JB3");
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
                allocation = WorkplaceAIHelper.DistributeWorkplaceLevels(int.Parse(manual.text), distribution);

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


        /// <summary>
        /// Creates a RICO-style dropdown menu.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="yPos">Relative Y position</param>
        /// <param name="label">Label text</param>
        /// <returns>New dropdown menu</returns>
        public static UIDropDown RICODropDown(UIComponent parent, float yPos, string label)
        {
            // Parent container.
            UIPanel container = parent.AddUIComponent<UIPanel>();
            container.height = 25f;
            container.relativePosition = new Vector2(0, yPos);

            // Label.
            UILabel serviceLabel = container.AddUIComponent<UILabel>();
            serviceLabel.textScale = 0.8f;
            serviceLabel.text = label;
            serviceLabel.relativePosition = new Vector2(15f, 6f);

            // Dropdown menu.
            UIDropDown dropDown = container.AddUIComponent<UIDropDown>();
            dropDown.size = new Vector2(180f, 25f);
            dropDown.listBackground = "GenericPanelLight";
            dropDown.itemHeight = 20;
            dropDown.itemHover = "ListItemHover";
            dropDown.itemHighlight = "ListItemHighlight";
            dropDown.normalBgSprite = "ButtonMenu";
            dropDown.disabledBgSprite = "ButtonMenuDisabled";
            dropDown.hoveredBgSprite = "ButtonMenuHovered";
            dropDown.focusedBgSprite = "ButtonMenu";
            dropDown.listWidth = 180;
            dropDown.listHeight = 500;
            dropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            dropDown.popupColor = new Color32(45, 52, 61, 255);
            dropDown.popupTextColor = new Color32(170, 170, 170, 255);
            dropDown.zOrder = 1;
            dropDown.textScale = 0.7f;
            dropDown.verticalAlignment = UIVerticalAlignment.Middle;
            dropDown.horizontalAlignment = UIHorizontalAlignment.Left;

            dropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            dropDown.itemPadding = new RectOffset(14, 0, 8, 0);

            dropDown.relativePosition = new Vector2(112f, 0);

            UIButton button = dropDown.AddUIComponent<UIButton>();
            dropDown.triggerButton = button;
            button.text = "";
            button.size = new Vector2(180f, 25f);
            button.relativePosition = new Vector3(0f, 0f);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            button.normalFgSprite = "IconDownArrow";
            button.hoveredFgSprite = "IconDownArrowHovered";
            button.pressedFgSprite = "IconDownArrowPressed";
            button.focusedFgSprite = "IconDownArrowFocused";
            button.disabledFgSprite = "IconDownArrowDisabled";
            button.spritePadding = new RectOffset(3, 3, 3, 3);
            button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            button.horizontalAlignment = UIHorizontalAlignment.Right;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.zOrder = 0;
            button.textScale = 0.8f;

            dropDown.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                button.size = t; dropDown.listWidth = (int)t.x;
            });


            // Allow for long translation strings.
            dropDown.autoListWidth = true;

            return dropDown;
        }


        /// <summary>
        /// Creates a RICO-style checkbox with a label.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="yPos">Relative Y position</param>
        /// <param name="label">Label text</param>
        /// <returns>New checkbox</returns>
        public static UICheckBox RICOLabelledCheckBox(UIComponent parent, float yPos, string label)
        {
            // Create containing panel.
            UIPanel container = parent.AddUIComponent<UIPanel>();
            container.height = 25f;
            container.width = 270f;
            container.relativePosition = new Vector2(0, yPos);

            // Add checkbox.
            UICheckBox checkBox = RICOCheckBox(container, 210f);

            // Checkbox label.
            UILabel serviceLabel = container.AddUIComponent<UILabel>();
            serviceLabel.textScale = 0.8f;
            serviceLabel.text = label;
            serviceLabel.relativePosition = new Vector2(15, 6);

            // Label behaviour.
            serviceLabel.autoSize = false;
            serviceLabel.width = 180f;
            serviceLabel.autoHeight = true;
            serviceLabel.wordWrap = true;

            return checkBox;
        }


        /// <summary>
        /// Creates a checkbox bar.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="label">Label text</param>
        /// <param name="width">Bar width</param>
        /// <returns>New checkbox</returns>
        private static UICheckBox RICOCheckBar(UIComponent parent, string label, float width)
        {
            // Create panel.
            UIPanel basePanel = parent.AddUIComponent<UIPanel>();
            basePanel.height = 25f;
            basePanel.backgroundSprite = "ScrollbarTrack";
            basePanel.width = width;
            basePanel.relativePosition = new Vector2(0f, 5f);

            // Add checkbox.
            UICheckBox checkBox = RICOCheckBox(basePanel, 7f);

            // Checkbox label.
            checkBox.label = checkBox.AddUIComponent<UILabel>();
            checkBox.label.text = label;
            checkBox.label.textScale = 0.8f;
            checkBox.label.autoSize = false;
            checkBox.label.size = new Vector2(190f, 18f);
            checkBox.label.textAlignment = UIHorizontalAlignment.Center;
            checkBox.label.relativePosition = new Vector2(25f, 2f);

            return checkBox;
        }


        /// <summary>
        /// Creates a Plopapble RICO-style checkbox.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="xPos">Relative X position</param>
        /// <returns>New checkbox</returns>
        private static UICheckBox RICOCheckBox(UIComponent parent, float xPos)
        {

            // Add checkbox.
            UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
            checkBox.width = parent.width;
            checkBox.height = 20f;
            checkBox.clipChildren = true;
            checkBox.relativePosition = new Vector2(xPos, 4f);

            // Checkbox sprite.
            UISprite sprite = checkBox.AddUIComponent<UISprite>();
            sprite.spriteName = "ToggleBase";
            sprite.size = new Vector2(16f, 16f);
            sprite.relativePosition = Vector3.zero;

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;

            return checkBox;
        }
    }
}
