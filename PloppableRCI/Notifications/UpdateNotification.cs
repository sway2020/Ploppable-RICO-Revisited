using System;
using UnityEngine;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Base class of the update notification panel.
    /// </summary>
    public class UpdateNotification : ErrorNotification
    {
        // Loaded from configuration file.
        internal static int notificationVersion;


        /// <summary>
        /// Creates the panel object in-game.
        /// </summary>
        public override void Create()
        {
            base.Create();
            instance = uiGameObject.AddComponent<UpdateNotification>();
        }


        /// <summary>
        /// Create the update notification panel; called by Unity just before any of the Update methods is called for the first time.
        /// </summary>
        public override void Start()
        {
            // Add text.
            headerText = Translations.Translate("PRR_UPD_23_0");
            messageText = Translations.Translate("PRR_UPD_23_1");
            listText = Translations.Translate("PRR_UPD_23_2") + "\r\n\r\n" + Translations.Translate("PRR_UPD_23_3") + "\r\n\r\n" + Translations.Translate("PRR_UPD_23_4") + "\r\n\r\n" + Translations.Translate("PRR_UPD_23_5");

            base.Start();

            try
            {
                // "Don't show again" button.
                UIButton noShowButton = CreateButton(this);
                noShowButton.relativePosition = new Vector3(this.width - noShowButton.width - spacing, this.height - noShowButton.height - spacing);
                noShowButton.text = "Don't show again";
                noShowButton.Enable();

                // Event handler.
                noShowButton.eventClick += (control, clickEvent) =>
                {
                    // Update and save settings file.
                    notificationVersion = 3;
                    SettingsUtils.SaveSettings();

                    // Just hide this panel and destroy the game object - nothing more to do.
                    this.Hide();
                    GameObject.Destroy(uiGameObject);
                };
            }
            catch (Exception e)
            {
                Debugging.LogException(e);
            }
        }
    }
}