using System.Collections.Generic;
using UnityEngine;
using ColossalFramework.Globalization;


namespace PloppableRICO
{
    // This is a really basic translation framework; it's the start of a process, and not the end.
    // At this point just directly translating the English default (no standardised keys).
    public class Translations
    {
        public static string GetTranslation(string phrase)
        {
            string _translation;

            if(LocaleManager.instance.language == "zh" && (UITranslations_zh.TryGetValue(phrase, out _translation)))
            {
                return _translation;
            }
            else
            {
                return phrase;
            }
        }

        // Chinese translations provided by Yuuki
        public static readonly Dictionary<string, string> UITranslations_zh = new Dictionary<string, string>()
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
            { "Tourism", "旅游业" },
            { "Leisure", "休闲业" },
            { "Eco (organic)", "本地有机作物" },
            { "Forestry", "林业" },
            { "Oil", "石油业" },
            { "Ore", "矿业" },
            { "Farming", "农业" },
            { "Residential low", "低密度住宅区" },
            { "Residential high", "高密度住宅区" },
            { "Commercial low", "低密度商业区" },
            { "Commercial high", "高密度商业区" },
            { "Organic commercial", "本地有机作物" },
            { "Self-sufficient homes", "自给建筑" },
            { "Save", "保存" },
            { "Add local", "添加到本地" },
            { "Remove local", "从本地删除" },
            { "Monuments", "独特建筑" },
            { "Beautification", "环境美化" },
            { "Education", "教育服务" },
            { "Power", "电力能源" },
            { "Water", "水源与供暖" },
            { "Health", "医疗服务" },
            { "No settings", "没有设置项" },
            { "Mod settings", "MOD设置" },
            { "Author settings", "作者设置" },
            { "Local settings", "本地设置" },
            { "Any settings", "任何设置" },
            { "Settings filter: Mod/Author/Local/Any" , "按设置过滤 (MOD/作者/本地/任何设置)" },
            { "Allows Plopping of RICO Buildings (fork of AJ3D's original with bugfixes and new features)", "允许对RICO建筑（AJ3D原始版本的分叉，带有错误修复和新功能）进行噗通" },
            { "Original Ploppable RICO mod detected - RICO Revisited is shutting down to protect your game.  Only ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!", "请取消原先版本的Ploppable RICO" },
            { "", "" }
        };


        public static string[] UICategory = 
        {
            Translations.GetTranslation("Residential low"),
            Translations.GetTranslation("Residential high"),
            Translations.GetTranslation("Commercial low"),
            Translations.GetTranslation("Commercial high"),
            Translations.GetTranslation("Office"),
            Translations.GetTranslation("Industrial"),
            Translations.GetTranslation("Farming"),
            Translations.GetTranslation("Forestry"),
            Translations.GetTranslation("Oil"),
            Translations.GetTranslation("Ore"),
            Translations.GetTranslation("Leisure"),
            Translations.GetTranslation("Tourism"),
            Translations.GetTranslation("Organic commercial"),
            Translations.GetTranslation("IT cluster"),
            Translations.GetTranslation("Self-sufficient homes"),
            Translations.GetTranslation("None")
        };
    }
}