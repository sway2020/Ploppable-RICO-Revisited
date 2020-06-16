using ICities;
using CitiesHarmony.API;


namespace PloppableRICO
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class PloppableRICOMod : IUserMod
    {
        public static string Version => "2.2";

        public string Name => "RICO Revisited " + Version;
        public string Description => Translations.Translate("PRR_DESCRIPTION");


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());
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
            // Read configuration file.
            SettingsFile settingsFile = Configuration<SettingsFile>.Load();

            // Add logging checkbox.
            helper.AddCheckbox(Translations.Translate("PRR_OPTION_MOREDEBUG"), settingsFile.DebugLogging, isChecked =>
            {
                Settings.debugLogging = isChecked;
                settingsFile.DebugLogging = isChecked;
                Configuration<SettingsFile>.Save();
            });

            // Add reset on load checkbox.
            helper.AddCheckbox(Translations.Translate("PRR_OPTION_FORCERESET"), settingsFile.ResetOnLoad, isChecked =>
            {
                settingsFile.ResetOnLoad = isChecked;
                Configuration<SettingsFile>.Save();
            });

            // Add thumbnail background checkbox.
            helper.AddCheckbox(Translations.Translate("PRR_OPTION_PLAINTHUMBS"), settingsFile.PlainThumbs, isChecked =>
            {
                Settings.plainThumbs = isChecked;
                settingsFile.PlainThumbs = isChecked;
                Configuration<SettingsFile>.Save();
            });

            // Add regenerate thumbnails button.
            helper.AddButton(Translations.Translate("PRR_OPTION_REGENTHUMBS"), () => PloppableTool.Instance.RebuildButtons());
        }
    }
}
