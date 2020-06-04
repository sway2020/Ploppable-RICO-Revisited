using System.Collections.Generic;
using UnityEngine;
using ColossalFramework.Globalization;


namespace PloppableRICO
{
    // This is a really basic translation framework; it's the start of a process, and not the end.
    // At this point just directly translating the English default (no standardised keys).
    internal class Translations
    {
        internal static string GetTranslation(string phrase)
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
        internal static readonly Dictionary<string, string> UITranslations_zh = new Dictionary<string, string>()
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
            { "Use plain backgrounds for thumbnails", "将纯背景用作缩略图" },
            { "Enable additional debugging logging", "启用其他调试日志记录" },
            { "Force reset of existing building stats on game load", "在游戏加载时强制重置现有建筑统计数据" },
            { "Regenerate thumbnails", "重新生成缩略图" },
            { "Original Ploppable RICO mod detected - RICO Revisited is shutting down to protect your game.\r\n\r\nOnly ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!", "请取消原先版本的Ploppable RICO" },
            { "Enhanced Building Capacity mod detected - RICO Revisited is shutting down to protect your game.\r\n\r\nOnly ONE of these mods can be enabled at the same time - please choose one and unsubscribe from the other!", "请取消原先版本的Enhanced Building Capacity" },
            { "Ploppable RICO Revisited has been updated to version 2.1.  Some key features of this update are:", "可访问的RICO重访版本已更新至2.1版。此更新的一些主要功能包括" },
            { "New mod options panel (accessed via game options), including option for plain thumbnail backgrounds.\r\n\r\nAdjusted lighting of thumnail image renders to help users with over-saturated map themes.\r\n\r\nLocal RICO settings created from existing growable buildings will be growable by default and inherit the default household/workplace counts of the original.\r\n\r\nAdditional failsafes to reduce risk of residential building household counts being reduced on game load if your city is close to hitting internal game limits.", "新的mod选项面板（可通过游戏选项访问），包括纯缩略图背景的选项\r\n\r\n调整后的缩略图图像照明可以帮助用户使用饱和度更高的地图主题\r\n\r\n从现有可增长建筑物创建的本地RICO设置将默认为可增长，并继承原始建筑物的默认家庭/工作场所计数\r\n\r\n代如果您的城市接近内部游戏限制，则额外的故障保护措施可减少住宅建筑的家庭人数减少游戏负荷的风险" },
            { "Close", "关" },
            { "Don't show again", "不再显示" },
            { "", "" }
        };


        internal static string[] UICategory = 
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


        internal static void TranslateUICategories()
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
