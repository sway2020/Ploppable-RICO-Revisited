using System;
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
            // Don't do anything if mod hasn't activated for whatever reason (mod conflict, harmony error, something else).
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

            // Display update notification.
            try
            {
                WhatsNew.ShowWhatsNew();
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception showing WhatsNew panel");
            }

            // Set up options panel event handler.
            try
            {
                OptionsPanel.OptionsEventHook();
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception hooking options panel");
            }

            Logging.KeyMessage("loading complete");
        }
    }
}