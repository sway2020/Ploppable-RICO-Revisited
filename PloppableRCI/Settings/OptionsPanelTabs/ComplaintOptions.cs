using ICities;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Options panel for setting growable building behaviour options.
    /// </summary>
    internal class ComplaintOptions
    {
        /// <summary>
        /// Adds growable options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal ComplaintOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("PRR_OPTION_COM"), tabIndex);
            UIHelper helper = new UIHelper(panel);
            panel.autoLayout = true;

            // Add 'ignore low value complaint' checkboxes.
            UIHelperBase valueGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_VAL"));
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RPL"), ModSettings.noValueRicoPlop, isChecked =>
            {
                ModSettings.noValueRicoPlop = isChecked;
                SettingsUtils.SaveSettings();
            });
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noValueRicoGrow, isChecked =>
            {
                ModSettings.noValueRicoGrow = isChecked;
                SettingsUtils.SaveSettings();
            });
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noValueOther, isChecked =>
            {
                ModSettings.noValueOther = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add 'ignore too few services complaint' checkboxes.
            UIHelperBase servicesGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SVC"));
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RPL"), ModSettings.noServicesRicoPlop, isChecked =>
            {
                ModSettings.noServicesRicoPlop = isChecked;
                SettingsUtils.SaveSettings();
            });
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noServicesRicoGrow, isChecked =>
            {
                ModSettings.noServicesRicoGrow = isChecked;
                SettingsUtils.SaveSettings();
            });
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noServicesOther, isChecked =>
            {
                ModSettings.noServicesOther = isChecked;
                SettingsUtils.SaveSettings();
            });
        }
    }
}