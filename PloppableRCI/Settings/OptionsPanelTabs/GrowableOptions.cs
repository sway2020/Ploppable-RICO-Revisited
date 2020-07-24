using ICities;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Options panel for setting growable building behaviour options.
    /// </summary>
    internal class GrowableOptions
    {
        /// <summary>
        /// Adds growable options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal GrowableOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("PRR_OPTION_GRO"), tabIndex);
            UIHelper helper = new UIHelper(panel);
            panel.autoLayout = true;

            // Add plop growables checkboxes.
            UIHelperBase plopGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_PLP"));
            plopGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.plopRico, isChecked =>
            {
                ModSettings.plopRico = isChecked;
                SettingsUtils.SaveSettings();
            });
            plopGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.plopOther, isChecked =>
            {
                ModSettings.plopOther = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add no zone checks checkboxes.
            UIHelperBase zoneGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_ZON"));
            zoneGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noZonesRico, isChecked =>
            {
                ModSettings.noZonesRico = isChecked;
                SettingsUtils.SaveSettings();
            });
            zoneGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noZonesOther, isChecked =>
            {
                ModSettings.noZonesOther = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add no specialisation checks checkboxes.
            UIHelperBase specGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SPC"));
            specGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noSpecRico, isChecked =>
            {
                ModSettings.noSpecRico = isChecked;
                SettingsUtils.SaveSettings();
            });
            specGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noSpecOther, isChecked =>
            {
                ModSettings.noSpecOther = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add 'ignore low value complaint' checkboxes.
            UIHelperBase valueGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_VAL"));
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noValueRico, isChecked =>
            {
                ModSettings.noValueRico = isChecked;
                SettingsUtils.SaveSettings();
            });
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noValueOther, isChecked =>
            {
                ModSettings.noValueOther = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add 'ignore too few services complaint' checkboxes.
            UIHelperBase servicesGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SVC"));
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noServicesRico, isChecked =>
            {
                ModSettings.noServicesRico = isChecked;
                SettingsUtils.SaveSettings();
            });
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noServicesOther, isChecked =>
            {
                ModSettings.noServicesOther = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add 'make plopped growables historical' checkboxes.
            UIHelperBase histGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_HST"));
            histGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.historicalRico, isChecked =>
            {
                ModSettings.historicalRico = isChecked;
                SettingsUtils.SaveSettings();
            });
            histGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.historicalOther, isChecked =>
            {
                ModSettings.historicalOther = isChecked;
                SettingsUtils.SaveSettings();
            });
        }
    }
}