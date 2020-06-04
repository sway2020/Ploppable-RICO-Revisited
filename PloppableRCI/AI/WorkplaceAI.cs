using ColossalFramework;
using ColossalFramework.Math;
using System.Linq;


namespace PloppableRICO
{
    public interface IWorkplaceLevelCalculator
    {
        //void CalculateWorkplaceCount(Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3);
        //void CalculateLevels(int width, int length, out int level0, out int level1, out int level2, out int level3);
        void CalculateBaseWorkplaceCount(ItemClass.Level level, Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3);
    }

    internal static class WorkplaceAIHelper
    {
        internal static int GetConstructionCost(int constructionCostValue, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            int result = (constructionCostValue * 100);
            Singleton<EconomyManager>.instance.m_EconomyWrapper.OnGetConstructionCost(ref result, service, subService, level);
            return result;
        }

        internal static void CalculateWorkplaceCount(ItemClass.Level level, RICOBuilding ricoData, IWorkplaceLevelCalculator ai, Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3)
        {
            SetWorkplaceLevels(out level0, out level1, out level2, out level3, 0, 0, 0, 0);
            RICOBuilding rc = ricoData;

            if ( rc != null )
            {
                // reality mod is running and the xml file says ignore-reality="false"
                if ( rc.useReality )
                    ai.CalculateBaseWorkplaceCount(level, r, width, length, out level0, out level1, out level2, out level3 );
                else
                    SetWorkplaceLevels( out level0, out level1, out level2, out level3, deviateWorkplaces( ricoData.workplaces, ricoData.workplaceDeviation ) );
            }
        }

        private static void SetWorkplaceLevels(out int level0, out int level1, out int level2, out int level3, int l0, int l1, int l2, int l3)
        {
            SetWorkplaceLevels(out level0, out level1, out level2, out level3, new int[] { l0, l1, l2, l3 });
        }

        internal static void SetWorkplaceLevels(out int level0, out int level1, out int level2, out int level3, int[] values)
        {
            level0 = values[0];
            level1 = values[1];
            level2 = values[2];
            level3 = values[3];
        }

        internal static int[] distributeWorkplaceLevels( int workplaces, int[] workplaceDistribution, int[] workplaceDeviation)
        {
            int[] wd = workplaceDistribution;
            int[] wv = workplaceDeviation;
           
            if (wd == null)
            {
                return new int[] { };
            }
            float @base = (float)wd[0];

            // distribute 
            int[] jobs = wd.Skip(1).Select(
                    share => (int)((float)workplaces * ((float)share / @base))
                  ).ToArray();

            return jobs;
        }

        private static int[] deviateWorkplaces( int[] workplaces, int[] deviatons )
        {
            if (deviatons != null)
                return workplaces.Select(
                    (jobc, i) => (int)new System.Random().Next( jobc - deviatons[i], jobc + deviatons[i] )
                ).ToArray();
            return workplaces;
        }
    }
}
