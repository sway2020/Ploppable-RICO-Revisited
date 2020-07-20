using ICities;
using ColossalFramework.UI;
using CitiesHarmony.API;


namespace PloppableRICO
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class PloppableRICOMod : IUserMod
    {
        public static string ModName => "RICO Revisited";
        public static string Version => "2.3 BETA";

        public string Name => ModName + " " + Version;
        public string Description => Translations.Translate("PRR_DESCRIPTION");


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());

            // Load settings file.
            SettingsUtils.LoadSettings();
        }


        /// <summary>
        /// Called by the game when the mod is disabled.
        /// </summary>
        public void OnDisabled()
        {
            // Unapply Harmony patches via Cities Harmony.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.UnpatchAll();
            }
        }


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // General options.
            UIHelperBase otherGroup = helper.AddGroup(" ");

            UIDropDown translationDropDown = (UIDropDown)otherGroup.AddDropdown(Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index, (value) =>
            {
                Translations.Index = value;
                SettingsUtils.SaveSettings();
            });
            translationDropDown.autoSize = false;
            translationDropDown.width = 270f;


            // Add plop growables checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_PLOPGROW"), ModSettings.plopGrowables, isChecked =>
            {
                ModSettings.plopGrowables = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add no zone checks checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_NOZONES"), ModSettings.noZoneChecks, isChecked =>
            {
                ModSettings.noZoneChecks = isChecked;
                SettingsUtils.SaveSettings();
            });


            // Add 'ignore low value complaint' checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_IGVAL"), ModSettings.ignoreValue, isChecked =>
            {
                ModSettings.ignoreValue = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add 'ignore too few services complaint' checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_IGSVS"), ModSettings.ignoreServices, isChecked =>
            {
                ModSettings.ignoreServices = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add 'make plopped growables historical' checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_HIST"), ModSettings.makeHistorical, isChecked =>
            {
                ModSettings.makeHistorical = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add logging checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_MOREDEBUG"), ModSettings.debugLogging, isChecked =>
            {
                ModSettings.debugLogging = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add reset on load checkbox.
            otherGroup.AddCheckbox(Translations.Translate("PRR_OPTION_FORCERESET"), ModSettings.resetOnLoad, isChecked =>
            {
                ModSettings.resetOnLoad = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Add thumbnail background dropdown.
            otherGroup.AddDropdown(Translations.Translate("PRR_OPTION_THUMBACK"), ModSettings.ThumbBackNames, ModSettings.thumbBacks, (value) =>
            {
                ModSettings.thumbBacks = value;
                SettingsUtils.SaveSettings();
            });

            // Add regenerate thumbnails button.
            otherGroup.AddButton(Translations.Translate("PRR_OPTION_REGENTHUMBS"), () => PloppableTool.Instance.RegenerateThumbnails());

            // Add speed boost checkbox.
            UIHelperBase speedGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SPDHDR"));
            speedGroup.AddCheckbox(Translations.Translate("PRR_OPTION_SPEED"), ModSettings.speedBoost, isChecked =>
            {
                ModSettings.speedBoost = isChecked;
                SettingsUtils.SaveSettings();
            });
        }
    }
}
