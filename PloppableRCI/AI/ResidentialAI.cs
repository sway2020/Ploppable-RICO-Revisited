using ColossalFramework.Math;
using UnityEngine;
using System.Linq;
using ICities;
using ColossalFramework;


namespace PloppableRICO
{
	public class PloppableResidential : ResidentialBuildingAI
	{
		public int m_constructionCost = 1;
		public int m_homeCount = 1;
        public int m_pbhomeCount = 0;
        public PloppableRICODefinition.Building m_ricoData;

        public BuildingTracker Bdata;

        // This is the interesting part where shit gets done
        public override int GetConstructionCost()
        {
            return WorkplaceAIHelper.GetConstructionCost(m_constructionCost, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
        }

        public override int CalculateHomeCount(Randomizer r, int width, int length)
        {
            if (m_ricoData.useReality)
            {
                return base.CalculateHomeCount(r, width, length);
            }

            return Mathf.Max(100, m_homeCount * 100 + r.Int32(100u)) / 100;            
		}

        // This is the boring part, just boilerplate stuff 
        public override void GetWidthRange(out int minWidth, out int maxWidth)
        {
            minWidth = 1;
            maxWidth = 16;
        }

        public override void GetLengthRange(out int minLength, out int maxLength)
        {
            minLength = 1;
            maxLength = 16;
        }

        public override string GenerateName(ushort buildingID, InstanceID caller)
        {
            return base.m_info.GetUncheckedLocalizedTitle();
        }

        public override bool ClearOccupiedZoning()
        {
             return false;
            //else return true;
        }


        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            BuildingTracker[] dataArray = BuildingDataManager.buildingData;
            Bdata = dataArray[(int)buildingID];

            if (Bdata == null)
            {
                Bdata = new BuildingTracker();
                dataArray[(int)buildingID] = Bdata;
                Bdata.itemclass = buildingData.Info.m_class;
                Bdata.isGrowable = true;
            }

            //If building is plopped, remove flags. 
            if (Bdata.isGrowable) Util.buildingFlags(ref buildingData);

			base.SimulationStep(buildingID, ref buildingData, ref frameData);

            Debug.Log(Singleton<SimulationManager>.instance.m_currentBuildIndex);
		

           if (Bdata.isGrowable) Util.buildingFlags(ref buildingData);

        }

		protected override void SimulationStepActive(ushort buildingID, ref Building buildingData, ref Building.Frame frameData){

            Util.buildingFlags(ref buildingData);

            base.SimulationStepActive(buildingID, ref buildingData, ref frameData);

            Util.buildingFlags(ref buildingData);

        }

		public override BuildingInfo GetUpgradeInfo(ushort buildingID, ref Building data){

            return null; //this will cause a check to fail in CheckBuildingLevel, and prevent the building form leveling. 

            //else return base.GetUpgradeInfo(buildingID, ref data);
		}
	}
}