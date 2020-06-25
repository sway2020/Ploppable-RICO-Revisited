using ColossalFramework.UI;
using UnityEngine;

namespace PloppableRICO
{

    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    internal static class Settings
    {
        internal static bool speedBoost = false;
        internal static bool debugLogging = false;
        internal static bool resetOnLoad = true;
        internal static bool fastThumbs = false;
        internal static int thumbBacks = (byte)ThumbBackCats.color;


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
    [ConfigurationPath("RICORevisited.xml")]
    public class SettingsFile
    {
        public int NotificationVersion { get; set; } = 0;
        public bool SpeedBoost { get; set; } = false;
        public bool PlainThumbs { get; } = false;
        public bool DebugLogging { get; set; } = false;
        public bool ResetOnLoad { get; set; } = true;
        public bool FastThumbs { get; set; } = false;
        public int ThumbBacks { get; set; } = (int)Settings.ThumbBackCats.color;
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
    }
}
