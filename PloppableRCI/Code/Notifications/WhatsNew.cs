using System;
using System.Reflection;
using PloppableRICO.MessageBox;


namespace PloppableRICO
{
    /// <summary>
    /// "What's new" message box.  Based on macsergey's code in Intersection Marking Tool (Node Markup) mod.
    /// </summary>
    internal static class WhatsNew
    {
        // List of versions and associated update message lines (as translation keys).
        private readonly static WhatsNewMessage[] WhatsNewMessages = new WhatsNewMessage[]
        {
            new WhatsNewMessage
            {
                version = new Version("2.4.3.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_243_0"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.4.2.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_242_0"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.4.1.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_241_0"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.4.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_240_0",
                    "PRR_UPD_240_1",
                    "PRR_UPD_240_2",
                    "PRR_UPD_240_3"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.3.7.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_237_0"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.3.6.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_236_0"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.3.5.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_235_0",
                    "PRR_UPD_235_1",
                    "PRR_UPD_235_2"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.3.4.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_234_0",
                    "PRR_UPD_234_1",
                    "PRR_UPD_234_2"
                }
            },
            new WhatsNewMessage
            {
                version = new Version("2.3.0"),
                messageKeys = true,
                messages = new string[]
                {
                    "PRR_UPD_23_2",
                    "PRR_UPD_23_3",
                    "PRR_UPD_23_4",
                    "PRR_UPD_23_5"
                }
            }
        };


        /// <summary>
        /// Close button action.
        /// </summary>
        /// <returns>True (always)</returns>
        public static bool Confirm() => true;

        /// <summary>
        /// 'Don't show again' button action.
        /// </summary>
        /// <returns>True (always)</returns>
        public static bool DontShowAgain()
        {
            // Save current version to settings file.
            ModSettings.whatsNewVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ModSettings.whatsNewBetaVersion = WhatsNewMessages[0].betaVersion;
            SettingsUtils.SaveSettings();

            return true;
        }


        /// <summary>
        /// Check if there's been an update since the last notification, and if so, show the update.
        /// </summary>
        internal static void ShowWhatsNew()
        {
            // Don't do anything if the settings isn't set.
            if (ModSettings.showWhatsNew)
            {
                Logging.KeyMessage("checking for update notifications");


                // Get last notified version and current mod version.
                Version whatsNewVersion = new Version(ModSettings.whatsNewVersion);
                WhatsNewMessage latestMessage = WhatsNewMessages[0];

                // Don't show notification if we're already up to (or ahead of) the first what's new message (including Beta updates).
                if (whatsNewVersion < latestMessage.version || (whatsNewVersion == latestMessage.version && latestMessage.betaVersion < ModSettings.whatsNewBetaVersion))
                {
                    // Show messagebox.
                    Logging.KeyMessage("showing What's New messagebox");
                    WhatsNewMessageBox messageBox = MessageBoxBase.ShowModal<WhatsNewMessageBox>();
                    messageBox.Title = PloppableRICOMod.ModName + " " + PloppableRICOMod.Version;
                    messageBox.DSAButton.eventClicked += (component, clickEvent) => DontShowAgain();
                    messageBox.SetMessages(whatsNewVersion, WhatsNewMessages);
                    Logging.KeyMessage("What's New messagebox complete");
                }
            }
        }
    }


    /// <summary>
    /// Version message struct.
    /// </summary>
    public struct WhatsNewMessage
    {
        public Version version;
        public string versionHeader;
        public int betaVersion;
        public bool messageKeys;
        public string[] messages;
    }
}