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

            // Add 'warn if bulldozing ploppables' checkbox.
            helper.AddCheckbox(Translations.Translate("PRR_OPTION_BDZ"), ModSettings.warnBulldoze, isChecked =>
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
            UICheckBox impCheck = (UICheckBox)helper.AddCheckbox(Translations.Translate("PRR_OPTION_IMP"), ModSettings.autoDemolish, isChecked =>
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
        }
    }
}