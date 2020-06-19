namespace PloppableRICO
{
    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    internal static class Settings
    {
        internal static bool speedBoost = false;
        internal static bool plainThumbs = false;
        internal static bool debugLogging = false;
        internal static bool resetOnLoad = true;
    }


    /// <summary>
    /// Defines the XML settings file.
    /// </summary>
    [ConfigurationPath("RICORevisited.xml")]
    public class SettingsFile
    {
        /// <summary>
        /// Stores the version of the most recent update notification that the user has decided to "Don't show again".
        /// </summary>
        public int NotificationVersion { get; set; } = 0;
        public bool SpeedBoost { get; set; } = false;
        public bool PlainThumbs { get; set; } = false;
        public bool DebugLogging { get; set; } = false;
        public bool ResetOnLoad { get; set; } = true;
    }
}
