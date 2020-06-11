using UnityEngine;
using ColossalFramework.UI;


namespace PloppableRICO
{
    public class UIBuildingFilter : UIPanel
    {
        private const int NumOfCategories = 10;
        internal UICheckBox[] zoningToggles;
        private UIButton allZones;
        private UIButton noZones;
        internal UITextField nameFilter;
        internal UICheckBox[] settingsFilter;

        internal bool IsZoneSelected(Category zone)
        {
            return zoningToggles[(int)zone].isChecked;
        }

        internal bool IsAllZoneSelected()
        {
            return zoningToggles[(int)Category.Monument].isChecked &&
                zoningToggles[(int)Category.Beautification].isChecked &&
                zoningToggles[(int)Category.Education].isChecked &&
                zoningToggles[(int)Category.Power].isChecked &&
                zoningToggles[(int)Category.Water].isChecked &&
                zoningToggles[(int)Category.Health].isChecked&&
                zoningToggles[(int)Category.Residential].isChecked &&
                zoningToggles[(int)Category.Commercial].isChecked &&
                zoningToggles[(int)Category.Office].isChecked &&
                zoningToggles[(int)Category.Industrial].isChecked;
        }


        internal string buildingName
        {
            get { return nameFilter.text.Trim(); }
        }

        public event PropertyChangedEventHandler<int> eventFilteringChanged;

        internal void Setup()
        {
            base.Start();

            // Zoning
            zoningToggles = new UICheckBox[NumOfCategories];
            for (int i = 0; i < NumOfCategories; i++)
            {
                zoningToggles[i] = UIUtils.CreateIconToggle(this, OriginalCategories.atlases[i], OriginalCategories.spriteNames[i], OriginalCategories.spriteNames[i] + "Disabled");
                zoningToggles[i].tooltip = OriginalCategories.tooltips[i];
                zoningToggles[i].relativePosition = new Vector3(40 * i, 0);
                zoningToggles[i].isChecked = true;
                zoningToggles[i].readOnly = true;

                // Single click event handler - toggle state of this button.
                zoningToggles[i].eventClick += (c, p) =>
                {
                    // If either shift or control is NOT held down, deselect all other toggles.
                    if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                    {
                        for (int j = 0; j < NumOfCategories; j++)
                        {
                            zoningToggles[j].isChecked = false;
                        }
                    }

                    // Select this toggle.
                    ((UICheckBox)c).isChecked = true;

                    // Trigger an update.
                    eventFilteringChanged(this, 0);
                };
            }

            allZones = UIUtils.CreateButton(this);
            allZones.width = 55;
            allZones.text = Translations.Translate("PRR_FTR_ALL");
            allZones.relativePosition = new Vector3(405, 5);

            allZones.eventClick += (c, p) =>
            {
                for (int i = 0; i < NumOfCategories; i++)
                {
                    zoningToggles[i].isChecked = true;
                }
                eventFilteringChanged(this, 0);
            };

            noZones = UIUtils.CreateButton(this);
            noZones.width = 55;
            noZones.text = Translations.Translate("PRR_FTR_NON");
            noZones.relativePosition = new Vector3(465, 5);

            noZones.eventClick += (c, p) =>
            {
                for (int i = 0; i < NumOfCategories; i++)
                {
                    zoningToggles[i].isChecked = false;
                }
                eventFilteringChanged(this, 0);
            };

            // Name filter
            UILabel nameLabel = AddUIComponent<UILabel>();
            nameLabel.textScale = 0.8f;
            nameLabel.padding = new RectOffset(0, 0, 8, 0);
            nameLabel.relativePosition = new Vector3(width - 250, 0);
            nameLabel.text = Translations.Translate("PRR_FTR_NAM") + ": ";

            nameFilter = UIUtils.CreateTextField(this);
            nameFilter.width = 200;
            nameFilter.height = 30;
            nameFilter.padding = new RectOffset(6, 6, 6, 6);
            nameFilter.relativePosition = new Vector3(width - nameFilter.width, 0);

            nameFilter.eventTextChanged += (c, s) => eventFilteringChanged(this, 5);
            nameFilter.eventTextSubmitted += (c, s) => eventFilteringChanged(this, 5);

            // Create settings filters.
            UILabel filterLabel = this.AddUIComponent<UILabel>();
            filterLabel.textScale = 0.8f;
            filterLabel.text = Translations.Translate("PRR_FTR_SET");
            filterLabel.relativePosition = new Vector3(10, 50, 0);

            // Setting filter checkboxes.
            settingsFilter = new UICheckBox[4];
            for (int i = 0; i < 4; i++)
            {
                settingsFilter[i] = this.AddUIComponent<UICheckBox>();

                settingsFilter[i].width = 20f;
                settingsFilter[i].height = 20f;
                settingsFilter[i].clipChildren = true;
                settingsFilter[i].relativePosition = new Vector3(280 + (30 * i), 45f);

                UISprite sprite = settingsFilter[i].AddUIComponent<UISprite>();
                sprite.spriteName = "ToggleBase";
                sprite.size = new Vector2(20f, 20f);
                sprite.relativePosition = Vector3.zero;

                settingsFilter[i].checkedBoxObject = sprite.AddUIComponent<UISprite>();
                ((UISprite)settingsFilter[i].checkedBoxObject).spriteName = "ToggleBaseFocused";
                settingsFilter[i].checkedBoxObject.size = new Vector2(20f, 20f);
                settingsFilter[i].checkedBoxObject.relativePosition = Vector3.zero;

                // Special event handling for 'any' checkbox.
                if (i == 3)
                {
                    settingsFilter[i].eventCheckChanged += (c, state) =>
                    {
                        if (state)
                        {
                            // Unselect all other checkboxes if 'any' is checked.
                            settingsFilter[0].isChecked = false;
                            settingsFilter[1].isChecked = false;
                            settingsFilter[2].isChecked = false;
                        }
                    };
                }
                else
                {
                    // Non-'any' checkboxes.
                    // Unselect 'any' checkbox if any other is checked.
                    settingsFilter[i].eventCheckChanged += (c, state) =>
                    {
                        if (state) settingsFilter[3].isChecked = false;
                    };
                }

                // Trigger filtering changed event if any checkbox is changed.
                settingsFilter[i].eventCheckChanged += (c, state) => { eventFilteringChanged(this, 0); };
            }

            // Settings filter tooltips.
            settingsFilter[0].tooltip = Translations.Translate("PRR_SET_HASMOD");
            settingsFilter[1].tooltip = Translations.Translate("PRR_SET_HASAUT");
            settingsFilter[2].tooltip = Translations.Translate("PRR_SET_HASLOC");
            settingsFilter[3].tooltip = Translations.Translate("PRR_SET_HASANY");
        }


        /// <summary>
        /// Sets the category toggles so that the one that includes the provided category is on, and the rest are off.
        /// </summary>
        /// <param name="buildingClass">RICO category of the building (to match toggle categories)</param>
        internal void SelectBuildingCategory(Category category)
        {
            // Iterate through each category.
            for (int i = 0; i < NumOfCategories; i ++)
            {
                if ((int)category == i)
                {
                    // Category match - select this toggle.
                    zoningToggles[i].isChecked = true;
                }
                else
                {
                    // Otherwise, deselect.
                    zoningToggles[i].isChecked = false;
                }
            }

            // Clear setting filter checkboxes.
            for (int i = 0; i < 4; i++)
            {
                settingsFilter[i].isChecked = false;
                settingsFilter[1].isChecked = false;
                settingsFilter[2].isChecked = false;
            }

            // Clear name search.
            nameFilter.text = string.Empty;
        }
    }
}