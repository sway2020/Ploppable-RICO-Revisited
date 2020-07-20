using System.Xml.Serialization;


namespace PloppableRICO
{

    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    internal static class ModSettings
    {
        internal static bool plopRico = true;
        internal static bool plopOther = true;
        internal static bool noZonesRico = true;
        internal static bool noZonesOther = true;
        internal static bool noValueRico = true;
        internal static bool noValueOther = true;
        internal static bool noServicesRico = true;
        internal static bool noServicesOther = true;
        internal static bool historicalRico = false;
        internal static bool historicalOther = false;
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

        [XmlElement("PlopRico")]
        public bool PlopRico { get => ModSettings.plopRico; set => ModSettings.plopRico = value; }

        [XmlElement("PlopOther")]
        public bool PlopOther { get => ModSettings.plopOther; set => ModSettings.plopOther = value; }

        [XmlElement("NoZonesRico")]
        public bool NoZonesRico { get => ModSettings.noZonesRico; set => ModSettings.noZonesRico = value; }

        [XmlElement("NoZonesOther")]
        public bool NoZonesOther { get => ModSettings.noZonesOther; set => ModSettings.noZonesOther = value; }

        [XmlElement("NoValueRico")]
        public bool NoValueRico { get => ModSettings.noValueRico; set => ModSettings.noValueRico = value; }

        [XmlElement("NoValueOther")]
        public bool NoValueOther { get => ModSettings.noValueOther; set => ModSettings.noValueOther = value; }

        [XmlElement("NoServicesRico")]
        public bool NoServicesRico { get => ModSettings.noServicesRico; set => ModSettings.noServicesRico = value; }

        [XmlElement("NoServicesOther")]
        public bool NoServicesOther { get => ModSettings.noServicesOther; set => ModSettings.noServicesOther = value; }

        [XmlElement("MakeRicoHistorical")]
        public bool MakeRicoHistorical { get => ModSettings.historicalRico; set => ModSettings.historicalRico = value; }

        [XmlElement("MakeOtherHistorical")]
        public bool MakeOtherHistorical { get => ModSettings.historicalOther; set => ModSettings.historicalOther = value; }

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
