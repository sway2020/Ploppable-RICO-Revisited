using System.Collections.Generic;


namespace PloppableRICO
{
    public class Translations
    {
        public static string GetTranslation(string phrase)
        {
            string _translation;

            if(UITranslations.TryGetValue(phrase, out _translation))
            {
                return _translation;
            }
            else
            {
                return phrase;
            }
        }


        public static readonly Dictionary<string, string> UITranslations = new Dictionary<string, string>()
        {
            { "Settings", "设置" },
            { "RICO Settings", "RICO设置" },
            { "All", "全选" },
            { "None", "全不选" },
            { "Name", "名称" },
            { "Enable RICO", "成为RICO建筑" },
            { "Service", "建筑类型" },
            { "Sub-service", "建筑子类型" },
            { "UI category", "建筑分类" },
            { "Level", "建筑等级" },
            { "Construction cost", "建筑造价" },
            { "Worker/Home count", "容纳工人/居民数量" },
            { "Use Realistic Pop mod", "使用真实人口MOD" },
            { "Enable pollution", "建筑物污染" },
            { "Uneducated jobs", "未受教育工作" },
            { "Educated jobs", "受初等教育工作" },
            { "Well-educated jobs", "受中等教育工作" },
            { "Highly-educated jobs", "受高等教育工作" },
            { "Residential", "住宅区" },
            { "Industrial", "工业区" },
            { "Office", "办公区" },
            { "Commercial", "商业区" },
            { "Extractor", "工业建筑" },
            { "Dummy", "混合型" },
            { "Generic", "通用的" },
            { "IT cluster", "信息产业区" },
            { "High", "高密度" },
            { "Low", "低密度" },
            { "High eco", "高密度ECO" },
            { "Low eco", "低密度ECO" },
            { "Tourist", "旅游业" },
            { "Leisure", "休闲业" },
            { "Eco (organic)", "本地有机作物" },
            { "Forest", "林业" },
            { "Oil", "石油业" },
            { "Ore", "矿业" },
            { "Farming", "农业" },
            { "Res low", "低密度住宅区" },
            { "Res high", "高密度住宅区" },
            { "Com low", "低密度商业区" },
            { "Com high", "高密度商业区" },
            { "Organic Com", "本地有机作物" },
            { "Self-sufficient", "自给建筑" },
            { "Save", "保存" },
            { "Add local", "添加到本地" },
            { "Remove local", "从本地删除" },
            { "Monuments", "独特建筑" },
            { "Beautification", "环境美化" },
            { "Education", "教育服务" },
            { "Power", "电力能源" },
            { "Water", "水源与供暖" },
            { "Health", "医疗服务" }
        };
    }
}