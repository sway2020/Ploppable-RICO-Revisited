using ICities;
using ColossalFramework;


namespace PloppableRICO
{

    /// <summary>
    /// Adds RICO complaint overriding to game levelling up process.
    /// </summary>
    public class LevelUpExtension : LevelUpExtensionBase
    {

        /// <summary>
        /// Residential level up control - game extension method.
        /// Used to override 'low land value' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageEducation">Average education for the building (ignored)</param>
        /// <param name="landValue">Land value for the building (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        public override ResidentialLevelUp OnCalculateResidentialLevelUp(ResidentialLevelUp levelUp, int averageEducation, int landValue,
            ushort buildingID, Service service, SubService subService, Level currentLevel)
        {
            // Check if the 'ignore low land value complaint' setting is set.
            if (ModSettings.ignoreValue)
            {
                // Check if this building is one of ours.
                if (IsRICOBuilding(buildingID))
                {
                    // It is - force land value complaint off.
                    levelUp.landValueTooLow = false;
                }
            }

            return levelUp;
        }


        /// <summary>
        /// Commercial level up control - game extension method.
        /// Used to override 'low land value' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageWealth">Building average wealth (ignored)</param>
        /// <param name="landValue">Land value for the building (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        /// <returns></returns>
        public override CommercialLevelUp OnCalculateCommercialLevelUp(CommercialLevelUp levelUp, int averageWealth, int landValue,
            ushort buildingID, Service service, SubService subService, Level currentLevel)
        {
            // Check if the 'ignore low land value complaint' setting is set.
            if (ModSettings.ignoreValue)
            {
                // Check if this building is one of ours.
                if (IsRICOBuilding(buildingID))
                {
                    // It is - force land value complaint off.
                    levelUp.landValueTooLow = false;
                }
            }

            return levelUp;
        }


        /// <summary>
        /// Industrial level up control - game extension method.
        /// Used to override 'too few services' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageEducation">Average education for the building (ignored)</param>
        /// <param name="serviceScore">Building service score (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        /// <returns></returns>
        public override IndustrialLevelUp OnCalculateIndustrialLevelUp(IndustrialLevelUp levelUp, int averageEducation, int serviceScore,
            ushort buildingID, Service service, SubService subService, Level currentLevel)
        {
            // Check if the 'ignore too few services complaint' setting is set.
            if (ModSettings.ignoreServices)
            {
                // Check if this building is one of ours.
                if (IsRICOBuilding(buildingID))
                {
                    // It is - force too few services complaint off.
                    levelUp.tooFewServices = false;
                }
            }

            return levelUp;
        }


        /// <summary>
        /// Office level up control - game extension method.
        /// Used to override 'too few services' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageEducation">Average education for the building (ignored)</param>
        /// <param name="serviceScore">Building service score (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        /// <returns></returns>
        public override OfficeLevelUp OnCalculateOfficeLevelUp(OfficeLevelUp levelUp, int averageEducation, int serviceScore, ushort buildingID,
            Service service, SubService subService, Level currentLevel)
        {
            // Check if the 'ignore too few services complaint' setting is set.
            if (ModSettings.ignoreServices)
            {
                // Check if this building is one of ours.
                if (IsRICOBuilding(buildingID))
                {
                    // It is - force too few services complaint off.
                    levelUp.tooFewServices = false;
                }
            }

            return levelUp;
        }


        /// <summary>
        /// Checks to see whether or not the specified building is a Ploppable RICO building.
        /// </summary>
        /// <param name="buildingID">Building instance ID</param>
        /// <returns>True if this is a Ploppable RICO building, false otherwise</returns>
        private bool IsRICOBuilding(ushort buildingID)
        {
            // Get AI instance.
            PrivateBuildingAI thisAI = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.GetAI() as PrivateBuildingAI;

            // Check AI against our AI types to determine result.
            return thisAI != null && (thisAI is GrowableResidentialAI || thisAI is GrowableCommercialAI || thisAI is GrowableOfficeAI || thisAI is GrowableIndustrialAI || thisAI is GrowableExtractorAI);
        }
    }
}