using ICities;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Options panel for setting growable building behaviour options.
    /// </summary>
    internal class PloppableOptions
    {
        /// <summary>
        /// Adds growable options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal PloppableOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("PRR_OPTION_PLO"), tabIndex);
            UIHelper helper = new UIHelper(panel);
            panel.autoLayout = true;

            // Demolition options.
            UIHelperBase demolishGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_DEM"));

            // Add 'warn if bulldozing ploppables' checkbox.
            demolishGroup.AddCheckbox(Translations.Translate("PRR_OPTION_BDZ"), ModSettings.warnBulldoze, isChecked =>
            {
                ModSettings.warnBulldoze = isChecked;
                SettingsUtils.SaveSettings();

                // Iterate through dictionary, looking for RICO ploppable buildings and updating their auto-remove flags.
                foreach (BuildingInfo prefab in Loading.xmlManager.prefabHash.Keys)
                {
                    // Get active RICO settings.
                    RICOBuilding building = RICOUtils.CurrentRICOSetting(Loading.xmlManager.prefabHash[prefab]);

                    // Check that it's enabled and isn't growable.
                    if (building != null && building.ricoEnabled && !building.growable)
                    {
                        // Apply flag.
                        prefab.m_autoRemove = !isChecked;
                    }
                }
            });

            // Add auto-demolish checkbox.
            UICheckBox impCheck = (UICheckBox)demolishGroup.AddCheckbox(Translations.Translate("PRR_OPTION_IMP"), ModSettings.autoDemolish, isChecked =>
            {
                ModSettings.autoDemolish = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Tweak auto-demolish checkbox label layout, to allow wrapping text.
            impCheck.label.wordWrap = true;
            impCheck.label.autoSize = false;
            impCheck.label.autoHeight = true;
            impCheck.label.width = 670f;
            impCheck.label.anchor = UIAnchorStyle.Left | UIAnchorStyle.Top;
            impCheck.label.relativePosition = new UnityEngine.Vector2(impCheck.label.relativePosition.x, 0f);

            // Cost options.
            UIHelperBase costGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_CST"));
            costGroup.AddCheckbox(Translations.Translate("PRR_OPTION_COV"), ModSettings.overrideCost, isChecked => ModSettings.overrideCost = isChecked);

            // Household costs.
            costGroup.AddTextfield(Translations.Translate("PRR_OPTION_CPH"), ModSettings.costPerHousehold.ToString(), value => { }, value => { ModSettings.costPerHousehold = int.Parse(value); });
            costGroup.AddTextfield(Translations.Translate("PRR_OPTION_CHM"), ModSettings.costMultResLevel.ToString(), value => { }, value => { ModSettings.costMultResLevel = int.Parse(value); });

            // Workplace costs.
            costGroup.AddTextfield(Translations.Translate("PRR_OPTION_CJ0"), ModSettings.costPerJob0.ToString(), value => { }, value => { ModSettings.costPerJob0 = int.Parse(value); });
            costGroup.AddTextfield(Translations.Translate("PRR_OPTION_CJ1"), ModSettings.costPerJob1.ToString(), value => { }, value => { ModSettings.costPerJob1 = int.Parse(value); });
            costGroup.AddTextfield(Translations.Translate("PRR_OPTION_CJ2"), ModSettings.costPerJob2.ToString(), value => { }, value => { ModSettings.costPerJob2 = int.Parse(value); });
            costGroup.AddTextfield(Translations.Translate("PRR_OPTION_CJ3"), ModSettings.costPerJob3.ToString(), value => { }, value => { ModSettings.costPerJob3 = int.Parse(value); });
        }
    }
}