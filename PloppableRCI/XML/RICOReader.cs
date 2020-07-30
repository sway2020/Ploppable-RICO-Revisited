using System;
using System.Xml.Serialization;
using System.IO;
using System.Text;


namespace PloppableRICO
{
    /// <summary>
    /// RICO configuration file reader.
    /// </summary>
    public class RICOReader
    {

        /// <summary>
        /// Loads and parses the given RICO file.
        /// </summary>
        /// <param name="packageName">Package name</param>
        /// <param name="ricoDefPath">Definition file path</param>
        /// <param name="isLocal">True if this is a local settings file, false for author settings file</param>
        /// <returns>Parsed Ploppable RICO definition file</returns>
        public static PloppableRICODefinition ParseRICODefinition( string packageName, string ricoDefPath, bool isLocal = false)
        {
            // Note here we're using insanityOK as a local settings flag.
            string localOrAuthor = isLocal ? "local" : "author";

            try
            {
                // Open file.
                using (StreamReader reader = new StreamReader(ricoDefPath))
                {
                    // Create new XML (de)serializer
                    XmlAttributes attrs = new XmlAttributes();
                    XmlElementAttribute attr = new XmlElementAttribute();
                    attr.ElementName = "RICOBuilding";
                    attr.Type = typeof(RICOBuilding);
                    XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
                    attrOverrides.Add(typeof(RICOBuilding), "Building", attrs);

                    // Read XML.
                    var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition), attrOverrides);
                    var result = xmlSerializer.Deserialize(reader) as PloppableRICODefinition;

                    StringBuilder errorList;

                    if (result.Buildings.Count == 0)
                    {
                        Debugging.Message("no parseable buildings in " + localOrAuthor + " XML settings file");
                    }
                    else
                    {
                        foreach (var building in result.Buildings)
                        {
                            // Check for fatal errors in each building.
                            errorList = building.fatalErrors;
                            if (errorList.Length == 0)
                            {
                                // No fatal errors; check for non-fatal errors.
                                errorList = building.nonFatalErrors;

                                if (errorList.Length != 0)
                                {
                                    // Errors found - how we report them depends on whether its local or author settings (we're assuming mod settings are fine).

                                    if (isLocal && building.ricoEnabled)
                                    {
                                        // Errors in local settings need to be reported direct to user, except for buildings that aren't activated in RICO.
                                        Debugging.ErrorBuffer.Append(errorList.ToString());
                                        Debugging.Message("non-fatal errors for building '" + building.name + "' in local settings");
                                    }
                                    else if (ModSettings.debugLogging)
                                    {
                                        // Errors in other settings should be logged if verbose logging is enabled, but otherwise continue.
                                        errorList.Insert(0, "found the following non-fatal errors for building '" + building.name + "' in author settings:\r\n");
                                        Debugging.Message(errorList.ToString());
                                    }
                                }
                            }
                            else
                            {
                                // Fatal errors!  Need to be reported direct to user and the building ignored.
                                Debugging.ErrorBuffer.Append(errorList.ToString());
                                Debugging.Message("fatal errors for building '" + building.name + "' in " + localOrAuthor + " settings");
                            }
                        }
                    }

                    reader.Close();

                    return result;
                }
            }
            catch (Exception e)
            {
                Debugging.ErrorBuffer.AppendLine(String.Format( "Unexpected Exception while deserializing " + localOrAuthor + " RICO file {0} ({1} [{2}])", packageName, e.Message, e.InnerException != null ? e.InnerException.Message : ""));
                return null;
            }
        }
    }
}
