using ColossalFramework.Math;


namespace PloppableRICO
{
    /// <summary>
    /// Replacement for Commercial AI for growable RICO buildings.
    /// </summary>
    public class GrowableCommercialAI : CommercialBuildingAI, IWorkplaceLevelCalculator
    {
        // RICO data record.
        public RICOBuilding m_ricoData;

        // Construction cost of this building.  Ignored for growables - having it here saves having extra checks in ConvertPrefabs().
        // Set a reasonable default, but will be overwritten by ConvertPrefabs() for ploppables.
        public int m_constructionCost = 10;

        // Number of workplaces in this building; set a reasonable default, but will be overwritten by ConvertPrefabs().
        public int m_workplaceCount = 1;

        // Cache to store workplace count calculations (saving a full calculation every update), one per level.
        private readonly int[][] workplaceCache;


        /// <summary>
        /// Constructor - initializes workplace cache.
        /// </summary>
        public GrowableCommercialAI()
        {
            // Initialise first dimension of workplace cache array here, so we don't have to waste CPU cycles anywhere else checking if it's been done or not.
            workplaceCache = new int[3][];
        }


        /// <summary>
        /// Calculates the workplaces for this building according to RICO settings.
        /// </summary>
        /// <param name="level">Building level</param>
        /// <param name="r">Randomizer</param>
        /// <param name="width">Building plot width (in cells)</param>
        /// <param name="length">Building plot length (in cells)</param>
        /// <param name="level0">The number of uneducated jobs</param>
        /// <param name="level1">The number of educated jobs</param>
        /// <param name="level2">The number of well-educated jobs</param>
        /// <param name="level3">The number of highly-educated jobs</param>
        public override void CalculateWorkplaceCount(ItemClass.Level level, Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3)
        {
            // CalculateWorkplaceCount is called by the game every couple of seconds.  Why?  Who knows?
            // This makes it a potential performance bottleneck; thus, we cache results to save some CPU cycles.
            // Results are cached in workplaceCount.

            // Bounds check for level.
            int buildingLevel = (int)level;
            if (buildingLevel > 2)
            {
                buildingLevel = 2;
            }

            // Check to see if there's a cached value, and if so, use it.
            int[] cachedWorkers = workplaceCache[buildingLevel];
            if (cachedWorkers != null)
            {
                WorkplaceAIHelper.SetWorkplaceLevels(out level0, out level1, out level2, out level3, cachedWorkers);
            }
            else
            {
                // If nothing is cached, then perform initial calculation.
                WorkplaceAIHelper.CalculateWorkplaceCount(level, m_ricoData, this, r, width, length, out level0, out level1, out level2, out level3);

                // Cache result.
                workplaceCache[buildingLevel] = new int[] { level0, level1, level2, level3 };
            }
        }


        /// <summary>
        /// Clears the workplace cache for this prefab.
        /// </summary>
        public void ClearWorkplaceCache()
        {
            workplaceCache[0] = null;
            workplaceCache[1] = null;
            workplaceCache[2] = null;
        }


        /// <summary>
        /// Calculates the workplaces for this building according to base method (non-RICO settings).
        /// Called by WorkPlaceAIHelper to access the base game method; for implementing functionality of mods that have detoured/patched that method (e.g. Realistic Population mods).
        /// </summary>
        /// <param name="level">Building level</param>
        /// <param name="r">Randomizer</param>
        /// <param name="width">Building plot width (in cells)</param>
        /// <param name="length">Building plot length (in cells)</param>
        /// <param name="level0">The number of uneducated jobs</param>
        /// <param name="level0">The number of uneducated jobs</param>
        /// <param name="level1">The number of educated jobs</param>
        /// <param name="level2">The number of well-educated jobs</param>
        /// <param name="level3">The number of highly-educated jobs</param>
        public void CalculateBaseWorkplaceCount(ItemClass.Level level, Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3)
        {
            base.CalculateWorkplaceCount(level, r, width, length, out level0, out level1, out level2, out level3); ;
        }


        /// <summary>
        /// Check to see if this building is unlocked (by progression level or other prerequisites).
        /// RICO buildings are always unlocked.
        /// </summary>
        /// <returns>Whether the building is currently unlocked</returns>
        public override bool CheckUnlocking()
        {
            return true;
        }
    }


    /// <summary>
    /// Replacement for Commercial AI for ploppable RICO buildings.
    /// </summary>
    public class PloppableCommercialAI : GrowableCommercialAI
    {
        /// <summary>
        ///  Returns the construction cost of the building.
        /// </summary>
        /// <returns>Construction cost</returns>
        public override int GetConstructionCost() => AIUtils.WorkplaceConstructionCost(this, m_constructionCost);


        /// <summary>
        /// Returns the construction time of the building.
        /// For ploppable RICO buildings this is always zero.
        /// </summary>
        /// <returns>Construction time (always 0)</returns>
        protected override int GetConstructionTime()
        {
            return 0;
        }

        /// <summary>
        /// Returns the acceptable width for this class of building AI.
        /// For ploppable RICO buildings  minimum is always 1 and maximum is always 16.
        /// </summary>
        /// <param name="minWidth">Minimum building width (always 1)</param>
        /// <param name="maxWidth">Maximum building width (always 16)</param>
        public override void GetWidthRange(out int minWidth, out int maxWidth)
        {
            minWidth = 1;
            maxWidth = 16;
        }


        /// <summary>
        /// Returns the acceptable length for this class of building AI.
        /// For ploppable RICO buildings  minimum is always 1 and maximum is always 16.
        /// </summary>
        /// <param name="minLength">Minimum building length (always 1)</param>
        /// <param name="maxLength">Maximum building width (always 16)</param>
        public override void GetLengthRange(out int minLength, out int maxLength)
        {
            minLength = 1;
            maxLength = 16;
        }


        /// <summary>
        /// Returns the name for the building.
        /// For ploppable RICO buildings this is always the base name of the prefab (no autogenerated names).
        /// </summary>
        /// <param name="buildingID">Instance ID of the building (unused)</param>
        /// <param name="caller">Calling instance (unused)</param>
        /// <returns></returns>
        public override string GenerateName(ushort buildingID, InstanceID caller)
        {
            return base.m_info.GetUncheckedLocalizedTitle();
        }


        /// <summary>
        /// Returns whether or not the building clears any zoning it's placed on.
        /// For ploppable RICO buildings this is always true.
        /// </summary>
        /// <returns>Whether this building clears away zoning (always true)</returns>
        public override bool ClearOccupiedZoning()
        {
            return true;
        }


        /// <summary>
        /// Determines what building (if any) this will upgrade to.
        /// For ploppable RICO buildings, this is always null.
        /// That causes a check to fail in CheckBuildingLevel and prevents the building from upgrading.
        /// </summary>
        /// <param name="buildingID">Instance ID of the original building</param>
        /// <param name="data">Building data struct</param>
        /// <returns>The BuildingInfo record of the building to upgrade to (always null)</returns>
        public override BuildingInfo GetUpgradeInfo(ushort buildingID, ref Building data)
        {
            return null;
        }


        /// <summary>
        /// Calculations performed on each simulation step.
        /// For a ploppable RICO building we want to force certain building flags to be set before and after each step.
        /// </summary>
        /// <param name="buildingID">Instance ID of the building</param>
        /// <param name="buildingData">Building data struct</param>
        /// <param name="frameData">Frame data</param>
        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            // Apply flags.
            AIUtils.SetBuildingFlags(ref buildingData);

            // Execute base method.
            base.SimulationStep(buildingID, ref buildingData, ref frameData);

            // Ensure flags are still applied.
            AIUtils.SetBuildingFlags(ref buildingData);
        }


        /// <summary>
        /// Calculations performed on each simulation step.
        /// For a ploppable RICO building we want to force certain building flags to be set before and after each step.
        /// </summary>
        /// <param name="buildingID">Instance ID of the building</param>
        /// <param name="buildingData">Building data struct</param>
        /// <param name="frameData">Frame data</param>
        protected override void SimulationStepActive(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            // Apply flags.
            AIUtils.SetBuildingFlags(ref buildingData);

            // Execute base method.
            base.SimulationStepActive(buildingID, ref buildingData, ref frameData);

            // Ensure flags are still applied.
            AIUtils.SetBuildingFlags(ref buildingData);
        }
    }
}