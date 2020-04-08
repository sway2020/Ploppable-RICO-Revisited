using System;
using System.Xml.Serialization;
using System.IO;

using UnityEngine;


namespace PloppableRICO
{
#if DEBUG
   // [ProfilerAspect()]
#endif
    public class RicoWriter
    {
        public static bool saveRicoData(string fileName, PloppableRICODefinition RicoDefinition)
        {
            try
            {
                var streamWriter = new System.IO.StreamWriter(fileName);
                var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition));
                xmlSerializer.Serialize(streamWriter, RicoDefinition);
                streamWriter.Close();
                return true;
            }
            catch
            { }
            return false;
        }


        public static string ricoDataXml( PloppableRICODefinition RicoDefinition )
        {
            try
            {
                var ms = new MemoryStream();
                var xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition));
                xmlSerializer.Serialize( ms, RicoDefinition );
                ms.Seek(0 , SeekOrigin.Begin);
                return ms.ToString();
            }
            catch
            { }
            return "";
        }
    }

#if DEBUG
    //[ProfilerAspect()]
#endif
    public class RICOReader
    {
        public static ICrpDataProvider crpDataProvider;

        public static PloppableRICODefinition ParseRICODefinition( string packageName, string ricoDefPath, bool insanityOK = false  )
        {
            var s = new FileStream( ricoDefPath, FileMode.Open);
            var r = DeserializeRICODefinition(packageName, s);

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


        public static PloppableRICODefinition DeserializeRICODefinition( string packageName, Stream ricoDefStream)
        {

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

                if (result.Buildings.Count == 0)
                {
                    Debugging.ErrorBuffer.AppendLine("No parseable buildings in XML settings file.");
                }
                else
                {
                    foreach (var building in result.Buildings)
                    {
                        if (building.errorCount == 0)
                        {
                            building.parent = result;
                        }
                        else
                        {
                            Debug.Log("RICO Revisited: failure to parse building " + building.name);
                        }
                    }
                }

                streamReader.Close();
                result.clean();
                return result;
            }
            catch (Exception e)
            {
                Debugging.ErrorBuffer.AppendLine(String.Format( "Unexpected Exception while deserializing RICO file {0} ({1} [{2}])", packageName, e.Message, e.InnerException != null ? e.InnerException.Message : ""));
                return null;
            }
        }
    }
}
