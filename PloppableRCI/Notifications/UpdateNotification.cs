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
            headerText = "Plopplable RICO Revisited 2.1 update";
            messageText = Translations.GetTranslation("Ploppable RICO Revisited has been updated to version 2.1.  Some key features of this update are:");
            listText = Translations.GetTranslation("New mod options panel (accessed via game options), including option for plain thumbnail backgrounds.\r\n\r\nAdjusted lighting of thumnail image renders to help users with over-saturated map themes.\r\n\r\nLocal RICO settings created from existing growable buildings will be growable by default and inherit the default household/workplace counts of the original.\r\n\r\nAdditional failsafes to reduce risk of residential building household counts being reduced on game load if your city is close to hitting internal game limits.");

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
                    Loading.settingsFile.NotificationVersion = 2;
                    Configuration<SettingsFile>.Save();

                    // Just hide this panel and destroy the game object - nothing more to do.
                    this.Hide();
                    GameObject.Destroy(uiGameObject);
                };
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
    }
}