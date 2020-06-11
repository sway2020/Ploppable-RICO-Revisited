using System;
using System.Xml.Serialization;
using System.IO;
using System.Text;


namespace PloppableRICO
{
    public class RICOReader
    {
        public static ICrpDataProvider crpDataProvider;

        public static PloppableRICODefinition ParseRICODefinition( string packageName, string ricoDefPath, bool insanityOK = false  )
        {
            var s = new FileStream( ricoDefPath, FileMode.Open);
            var r = DeserializeRICODefinition(packageName, s, insanityOK);

            if ( r != null )
                r.sourceFile = new FileInfo( ricoDefPath );
            s.Close();
            if ( r != null && crpDataProvider != null )
                addCrpShit( r );
            return r;
        }


        private static void addCrpShit(PloppableRICODefinition ricoDef)
        {
            var crpPath = Util.crpFileIn( ricoDef.sourceFile.Directory );
            if ( crpPath != null )
                foreach ( var building in ricoDef.Buildings )
                    building.crpData = crpDataProvider.getCrpData( crpPath.FullName );
        } 


        public static PloppableRICODefinition DeserializeRICODefinition( string packageName, Stream ricoDefStream, bool insanityOK)
        {
            // Note here we're using insanityOK as a local settings flag.
            string localOrAuthor = insanityOK ? "local" : "author";


            try
            {
                XmlAttributes attrs = new XmlAttributes();

                XmlElementAttribute attr = new XmlElementAttribute();
                attr.ElementName = "RICOBuilding";
                attr.Type = typeof( RICOBuilding );

                XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
                attrOverrides.Add( typeof( RICOBuilding ), "Building", attrs );

                var streamReader = new System.IO.StreamReader(ricoDefStream);
                var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition), attrOverrides);
                var result = xmlSerializer.Deserialize(streamReader) as PloppableRICODefinition;

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

                                if (insanityOK && building.ricoEnabled)
                                {
                                    // Errors in local settings need to be reported direct to user, except for buildings that aren't activated in RICO.
                                    Debugging.ErrorBuffer.Append(errorList.ToString());
                                    Debugging.Message("non-fatal errors for building '" + building.name + "' in local settings");
                                }
                                else if (Settings.debugLogging)
                                {
                                    // Errors in other settings should be logged if verbose logging is enabled, but otherwise continue.
                                    errorList.Insert(0, "found the following non-fatal errors for building '" + building.name + "' in author settings:\r\n");
                                    Debugging.Message(errorList.ToString());
                                }
                            }

                            // No fatal errors; building is good (enough).
                            building.parent = result;
                        }
                        else
                        {
                            // Fatal errors!  Need to be reported direct to user and the building ignored.
                            Debugging.ErrorBuffer.Append(errorList.ToString());
                            Debugging.Message("fatal errors for building '" + building.name + "' in " + localOrAuthor + " settings");
                        }
                    }
                }

                streamReader.Close();
                result.clean();
                return result;
            }
            catch (Exception e)
            {
                Debugging.ErrorBuffer.AppendLine(String.Format( "Unexpected Exception while deserializing " + localOrAuthor + " RICO file {0} ({1} [{2}])", packageName, e.Message, e.InnerException != null ? e.InnerException.Message : ""));
                return null;
            }
        }
    }
}
