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
            { "Lvl ", "建筑等级" },
            { "Level", "建筑等级" },
            { "Construction cost", "建筑造价" },
            { "Households", "居民数量" },
            { "Workplaces", "容纳工人" },
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
            { "\r\nCAUTION: EXPERIMENTAL", "\r\n小心：实验性" },
            { "Save and apply changes", "保存并应用更改" },
            { "Growable [EXPERIMENTAL]", "可成长[实验性]" },
            { "Allows Plopping of RICO Buildings (fork of AJ3D's original with bugfixes and new features)", "允许对RICO建筑（AJ3D原始版本的分叉，带有错误修复和新功能）进行噗通" },
            { "Original Ploppable RICO mod detected - RICO Revisited is shutting down to protect your game.\r\n\r\nOnly ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!", "请取消原先版本的Ploppable RICO" },
            { "Enhanced Building Capacity mod detected - RICO Revisited is shutting down to protect your game.\r\n\r\nOnly ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!", "请取消原先版本的Enhanced Building Capacity" },
            { "Ploppable RICO Revisited has been updated to version 2.0.  Some key features of this update are:", "可访问的RICO重访版本已更新至2.0版。此更新的一些主要功能包括" },
            { "Support for growable buildings (experimental)\r\n\r\nLive application of building configuration changes(experimental)\r\n\r\nDirectly access the Ploppable RICO settings for any building by clicking on the Ploppable RICO icon in the top - right of any info panel\r\n\r\nCode updates and improvements to fix a number of longstanding issues\r\n\r\nMany other minor changes and improvements", "支持可增长的建筑物（实验性）\r\n\r\n实时应用建筑配置更改（实验性）\r\n\r\n通过单击任何建筑物信息面板右上角的Ploppable RICO图标，直接访问任何建筑物的Ploppable RICO设置\r\n\r\n代码更新和改进，解决了许多长期存在的问题\r\n\r\n许多其他小的变化和改进" },
            { "NOTE: Please make sure to make a backup of your savegames before trying out the new experimental features, just in case.", "注意：为了以防万一，在尝试新的实验性功能之前，请确保对保存的游戏进行备份" },
            { "Close", "关" },
            { "Don't show again", "不再显示" },
            { "", "" }
        };


        public static string[] UICategory = 
        {
            "Residential low",
            "Residential high",
            "Commercial low",
            "Commercial high",
            "Office",
            "Industrial",
            "Farming",
            "Forestry",
            "Oil",
            "Ore",
            "Leisure",
            "Tourism",
            "Organic commercial",
            "IT cluster",
            "Self-sufficient homes",
            "None"
        };


        public static void TranslateUICategories()
        {
            if(LocaleManager.instance.language == "zh")
            {
                UICategory[0] = "低密度住宅区";
                UICategory[1] = "高密度住宅区";
                UICategory[2] = "低密度商业区";
                UICategory[3] = "高密度商业区";

                UICategory[4] = "办公区";
                UICategory[5] = "工业区";
                UICategory[6] = "农业";
                UICategory[7] = "林业";
                UICategory[8] = "石油业";
                UICategory[9] = "矿业";

                UICategory[10] = "休闲业";
                UICategory[11] = "旅游业";
                UICategory[12] = "本地有机作物";
                UICategory[13] = "信息产业区";
                UICategory[14] = "自给建筑";

                UICategory[15] = "全不选";
            }
        }
    }
}
