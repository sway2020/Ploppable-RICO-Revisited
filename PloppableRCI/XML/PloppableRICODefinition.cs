
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

        public RICOBuilding addBuilding(RICOBuilding buildingDef = null)
        {
            if (buildingDef == null)
            {
                buildingDef = new RICOBuilding();
                buildingDef.name = "* unnamed";
                buildingDef.parent = this;
            }

            Buildings.Add(buildingDef);

            return buildingDef;
        }

        public RICOBuilding removeBuilding(int index)
        {
            if (index < 0 && index >= this.Buildings.Count)
                return null;

            return removeBuilding(Buildings[index]);
        }

        public RICOBuilding removeBuilding(RICOBuilding buildingDef)
        {
            Buildings.Remove(buildingDef);
            return buildingDef;
        }
    }
}