using System.Windows.Forms;

namespace PloppableRICO
{
    /// <summary>
    /// A simple UI panel to show any mod conflict notifications.
    /// </summary>
    class HarmonyNotification : ErrorNotification
    {
        /// <summary>
        /// Creates the panel object in-game.
        /// </summary>
        public override void Create()
        {
            base.Create();
            instance = uiGameObject.AddComponent<HarmonyNotification>();
        }


        /// <summary>
        /// Sets up the panel; called by Unity just before any of the Update methods is called for the first time..
        /// </summary>
        public override void Start()
        {
            headerText = "Harmony patching error";
            messageText = Translations.GetTranslation("Ploppable RICO Revisited was unable to complete its required Harmony patches.  This means that the mod is not able to operate, and has shut down to protect your save file from damage.\r\n\r\nPossible causes of this problem include:");
            listText = Translations.GetTranslation("The required Harmony 2 mod dependency was not installed\r\n\r\nAn old and/or broken mod is preventing Harmony 2 from operating properly (the old Painter mod is known to cause this)");

            base.Start();
        }
    }
}