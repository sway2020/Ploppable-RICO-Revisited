using System.Xml.Serialization;


namespace PloppableRICO
{

    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    internal static class ModSettings
    {
        internal static bool plopGrowables = true;
        internal static bool noZoneChecks = true;
        internal static bool speedBoost = false;
        internal static bool debugLogging = false;
        internal static bool resetOnLoad = true;
        internal static int thumbBacks = (byte)ThumbBackCats.skybox;


        // Thumbnail background category enum.
        public enum ThumbBackCats
        {
            color,
            plain,
            skybox,
            numCats
        }


        /// <summary>
        /// Thumbnail backround names (for dropdown menu).
        /// </summary>
        internal static string[] ThumbBackNames => new string[]
        {
            Translations.Translate("PRR_THUMB_COLOR"),
            Translations.Translate("PRR_THUMB_PLAIN"),
            Translations.Translate("PRR_THUMB_SKY")
        };
    }


    /// <summary>
    /// Defines the XML settings file.
    /// </summary>
    [XmlRoot("SettingsFile")]
    public class XMLSettingsFile
    {
        [XmlElement("NotificationVersion")]
        public int NotificationVersion { get => UpdateNotification.notificationVersion; set => UpdateNotification.notificationVersion = value; }
        [XmlElement("PlopGrowables")]
        public bool PlopGrowables { get => ModSettings.plopGrowables; set => ModSettings.plopGrowables = value; }
        [XmlElement("NoZoneChecks")]
        public bool NoZoneChecks { get => ModSettings.noZoneChecks; set => ModSettings.noZoneChecks = value; }
        [XmlElement("SpeedBoost")]
        public bool SpeedBoost { get => ModSettings.speedBoost; set => ModSettings.speedBoost = value; }
        [XmlElement("DebugLogging")]
        public bool DebugLogging { get => ModSettings.debugLogging; set => ModSettings.debugLogging = value; }
        [XmlElement("ResetOnLoad")]
        public bool ResetOnLoad { get => ModSettings.resetOnLoad; set => ModSettings.resetOnLoad = value; }
        [XmlElement("ThumbBacks")]
        public int ThumbBacks
        {
            get => ModSettings.thumbBacks;
            set
            {
                ModSettings.thumbBacks = value;

                // Bounds check.
                if ((int)ModSettings.thumbBacks > (int)ModSettings.ThumbBackCats.numCats - 1 || ModSettings.thumbBacks < 0)
                {
                    ModSettings.thumbBacks = (int)ModSettings.ThumbBackCats.skybox;
                }
            }
        }
        [XmlElement("Language")]
        public string Language
        {
            get
            {
                return Translations.Language;
            }
            set
            {
                Translations.Language = value;
            }
        }


        // Legacy 'use plain thumbnails' conversion.
        [XmlElement("PlainThumbs")]
        public bool PlainThumbs
        {
            set
            {
                if (value)
                {
                    ModSettings.thumbBacks = (int)ModSettings.ThumbBackCats.plain;
                }
            }
        }
    }
}
