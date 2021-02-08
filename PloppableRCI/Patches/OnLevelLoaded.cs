using HarmonyLib;
using PloppableRICO.MessageBox;


namespace PloppableRICO
{
    /// <summary>
    /// Harmony Postfix patch for OnLevelLoaded.  This enables us to perform setup tasks after all loading has been completed.
    /// </summary>
    [HarmonyPatch(typeof(LoadingWrapper))]
    [HarmonyPatch("OnLevelLoaded")]
    public static class OnLevelLoadedPatch
    {
        /// <summary>
        /// Harmony postfix to perform actions require after the level has loaded.
        /// </summary>
        public static void Postfix()
        {
            // Check watchdog flag.
            if (!Loading.patchOperating)
            {
                // Patch wasn't operating; display harmony error and abort.
                Loading.harmonyLoaded = false;
                Loading.isModEnabled = false;
            }

            // Check to see that Harmony 2 was properly loaded.
            if (!Loading.harmonyLoaded)
            {
                // Harmony 2 wasn't loaded; display warning notification and exit.
                ListMessageBox harmonyBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                harmonyBox.AddParas(Translations.Translate("ERR_HAR0"), Translations.Translate("PRR_ERR_HAR"), Translations.Translate("PRR_ERR_FAT"), Translations.Translate("ERR_HAR1"));

                // List of dot points.
                harmonyBox.AddList(Translations.Translate("ERR_HAR2"), Translations.Translate("ERR_HAR3"));

                // Closing para.
                harmonyBox.AddParas(Translations.Translate("MES_PAGE"));
            }

            // Check to see if a conflicting mod has been detected.
            if (Loading.conflictingMod)
            {
                // Mod conflict detected - display warning notification and exit.
                ListMessageBox modConflictBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                modConflictBox.AddParas(Translations.Translate("ERR_CON0"), Translations.Translate("PRR_ERR_FAT"), Translations.Translate("PRR_ERR_CON0"), Translations.Translate("ERR_CON1"));

                // Add conflicting mod name(s).
                modConflictBox.AddList(ModUtils.conflictingModNames.ToArray());

                // Closing para.
                modConflictBox.AddParas(Translations.Translate("PRR_ERR_CON1"));
            }

            // Don't do anything further if mod hasn't activated for whatever reason (mod conflict, harmony error, something else).
            if (!Loading.isModEnabled)
            {
                return;
            }

            // Report any 'soft' mod conflicts.
            if (Loading.softModConflct)
            {
                // Soft conflict detected - display warning notification for each one.
                foreach (string mod in ModUtils.conflictingModNames)
                {
                    if (mod.Equals("PTG") && ModSettings.dsaPTG == 0)
                    {
                        // Plop the Growables.
                        DontShowAgainMessageBox softConflictBox = MessageBoxBase.ShowModal<DontShowAgainMessageBox>();
                        softConflictBox.AddParas(Translations.Translate("PRR_CON_PTG0"), Translations.Translate("PRR_CON_PTG1"), Translations.Translate("PRR_CON_PTG2"));
                        softConflictBox.DSAButton.eventClicked += (component, clickEvent) => { ModSettings.dsaPTG = 1; SettingsUtils.SaveSettings(); };
                    }
                }
            }

            // Report any broken assets and remove from our prefab dictionary.
            foreach (BuildingInfo prefab in Loading.brokenPrefabs)
            {
                Logging.Error("broken prefab: ", prefab.name);
                Loading.xmlManager.prefabHash.Remove(prefab);
            }
            Loading.brokenPrefabs.Clear();

            // Init Ploppable Tool panel.
            PloppableTool.Initialize();

            // Add buttons to access building details from zoned building info panels.
            SettingsPanel.AddInfoPanelButtons();

            Logging.KeyMessage("loading complete");

            // Display update notification.
            WhatsNew.ShowWhatsNew();

            // Set up options panel event handler.
            OptionsPanel.OptionsEventHook();
        }
    }
}