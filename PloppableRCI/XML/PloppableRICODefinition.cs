
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System;


namespace PloppableRICO
{
    public class PloppableRICODefinition
    {
        public List<RICOBuilding> Buildings { get; set; }

        [XmlIgnore]
        public FileInfo sourceFile;

        public PloppableRICODefinition()
        {
            Buildings = new List<RICOBuilding>();
        }

        public RICOBuilding removeBuilding(RICOBuilding buildingDef)
        {
            Buildings.Remove(buildingDef);
            return buildingDef;
        }
    }
}