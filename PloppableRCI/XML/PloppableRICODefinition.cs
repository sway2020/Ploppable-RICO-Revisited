

using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace PloppableRICO
{
    /// <summary>
    /// Ploppable RICO XML file definition.
    /// </summary>
    public class PloppableRICODefinition
    {
        /// <summary>
        /// List of RICO building definitions.
        /// </summary>
        public List<RICOBuilding> Buildings { get; set; }


        /// <summary>
        /// Constructor - initialises building list.
        /// </summary>
        public PloppableRICODefinition()
        {
            Buildings = new List<RICOBuilding>();
        }
    }
}