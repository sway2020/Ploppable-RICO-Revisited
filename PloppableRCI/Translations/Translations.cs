using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using ICities;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.Globalization;


namespace PloppableRICO
{
    /// <summary>
    /// Static class to provide translation interface.
    /// </summary>
    public static class Translations
    {
        // Instance reference.
        private static Translator _instance;


        /// <summary>
        /// Initialise the translations framework.
        /// </summary>
        public static void Setup()
        {
            _instance = new Translator();
        }


        /// <summary>
        /// Static interface to instance's translate method
        /// </summary>
        /// <param name="text">Key to translate</param>
        /// <returns>Translation (or key if translation failed)</returns>
        public static string Translate(string key) => _instance.Translate(key);
    }


    /// <summary>
    /// Handles translations.  Based off BloodyPenguin's framework.
    /// </summary>
    public class Translator
    {
        private Language currentLanguage;
        private List<Language> languages;
        private string fallbackLanguage = "en";

        /// <summary>
        /// Constructor.
        /// </summary>
        public Translator()
        {
            // Initialise languages list.
            languages = new List<Language>();

            // Load translation files and set the current language.
            LoadLanguages();
            SetLanguage();

            // Event handler to update the current language when system locale changes.
            LocaleManager.eventLocaleChanged += SetLanguage;
        }


        /// <summary>
        /// Returns the translation for the given key in the current language.
        /// </summary>
        /// <param name="key">Translation key to transate</param>
        /// <returns>Translation </returns>
        public string Translate(string key)
        {
            // Check that a valid current language is set.
            if (currentLanguage != null)
            {
                // Check that the current key is included in the translation.
                if (currentLanguage.translationDictionary.ContainsKey(key))
                {
                    // All good!  Return translation.
                    return currentLanguage.translationDictionary[key];
                }
                else
                {
                    Debug.Log("RICO Revisited: no translation for language '" + currentLanguage.uniqueName + "' found for key '" + key + "'.");

                    // Attempt fallack language; if even that fails, just return the key.
                    return FallbackLanguage().translationDictionary.ContainsKey(key) ? FallbackLanguage().translationDictionary[key] ?? key : key;
                }
            }
            else
            {
                Debug.Log("RICO Revisited: no current language set when translating key '" + key + "'.");
            }

            // If we've made it this far, something went wrong; just return the key.
            return key;
        }


        /// <summary>
        /// Sets the current language based on system settings.
        /// </summary>
        private void SetLanguage()
        {
            // Don't do anything if no languages have been loaded, or the LocaleManager isn't available.
            if (languages != null && languages.Count > 0 && LocaleManager.exists)
            {
                // Try to set current language, falling back to default if null.
                currentLanguage = languages.Find(language => language.uniqueName == LocaleManager.instance.language) ?? FallbackLanguage();
            }
        }


        /// <summary>
        /// Returns a fallback language reference in case the primary one fails (for whatever reason).
        /// </summary>
        /// <returns>Fallback language reference</returns>
        private Language FallbackLanguage()
        {
            return languages.Find(language => language.uniqueName == fallbackLanguage);
        }


        /// <summary>
        /// Loads languages from XML files.
        /// </summary>
        private void LoadLanguages()
        {
            // Clear existing dictionary.
            languages.Clear();

            // Get the current assembly path and append our locale directory name.
            string assemblyPath = GetAssemblyPath();
            if (!assemblyPath.IsNullOrWhiteSpace())
            {
                string localePath = Path.Combine(assemblyPath, "Translations");

                // Ensure that the directory exists before proceeding.
                if (Directory.Exists(localePath))
                {
                    // Load each file in directory and attempt to deserialise as a translation file.
                    string[] translationFiles = Directory.GetFiles(localePath);
                    foreach(string translationFile in translationFiles)
                    {
                        using (StreamReader reader = new StreamReader(translationFile))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Language));
                            if (xmlSerializer.Deserialize(reader) is Language translation)
                            {
                                // Got one!  add it to the list.
                                languages.Add(translation);
                            }
                            else
                            {
                                Debug.Log("RICO Revisited: couldn't deserialize translation file '" + translationFile + "'.");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("RICO Revisited: translations directory not found!");
                }
            }
            else
            {
                Debug.Log("RICO Revisited: assembly path was empty!");
            }
        }


        /// <summary>
        /// Returns the filepath of the Ploppable RICO Revisited assembly.
        /// </summary>
        /// <returns>Ploppable RICO revisited assembly filepath</returns>
        private string GetAssemblyPath()
        {
            // Get list of currently active plugins.
            IEnumerable<PluginManager.PluginInfo> plugins = PluginManager.instance.GetPluginsInfo();

            // Iterate through list.
            foreach (PluginManager.PluginInfo plugin in plugins)
            {
                try
                {
                    // Get all (if any) mod instances from this plugin.
                    IUserMod[] mods = plugin.GetInstances<IUserMod>();

                    // Check to see if the primary instance is this mod.
                    if (mods.FirstOrDefault() is PloppableRICOMod)
                    {
                        // Found it! Return path.
                        return plugin.modPath;
                    }
                }
                catch
                {
                    // Don't care.
                }
            }

            // If we got here, then we didn't find the assembly.
            Debug.Log("RICO Revisited: assembly path not found!");
            throw new FileNotFoundException("RICO Revisited: assembly path not found!");
        }
    }
}