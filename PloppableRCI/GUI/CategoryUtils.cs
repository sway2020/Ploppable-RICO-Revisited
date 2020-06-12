namespace PloppableRICO
{
    public enum Category
    {
        None = -1,
        Monument,
        Beautification,
        Education,
        Power,
        Water,
        Health,
        Residential,
        Commercial,
        Office,
        Industrial
    }

    /// <summary>
    /// Data struct for original building categories (to sort settings panel building list).
    /// </summary>
    internal static class OriginalCategories
    {
        // Icons representing each category.
        internal static readonly string[] spriteNames =
        {
            "ToolbarIconMonuments",
            "ToolbarIconBeautification",
            "ToolbarIconEducation",
            "ToolbarIconElectricity",
            "ToolbarIconWaterAndSewage",
            "ToolbarIconHealthcare",
            "ZoningResidentialHigh",
            "ZoningCommercialHigh",
            "ZoningOffice" ,
            "ZoningIndustrial"
        };

        // Icon atlas name for each of the above sprites.
        internal static readonly string[] atlases =
        {
            "Ingame",
            "Ingame" ,
            "Ingame",
            "Ingame",
            "Ingame",
            "Ingame",
            "Thumbnails",
            "Thumbnails",
            "Thumbnails",
            "Thumbnails"
        };
    }


    internal class CategoryNames
    {
        // Tooltips.
        internal readonly string[] names =
        {
            Translations.Translate("PRR_CAT_MON"),
            Translations.Translate("PRR_CAT_BEA"),
            Translations.Translate("PRR_CAT_EDU"),
            Translations.Translate("PRR_CAT_POW"),
            Translations.Translate("PRR_CAT_WAT"),
            Translations.Translate("PRR_CAT_HEA"),
            Translations.Translate("PRR_CAT_RES"),
            Translations.Translate("PRR_CAT_COM"),
            Translations.Translate("PRR_CAT_OFF"),
            Translations.Translate("PRR_CAT_IND")
        };
    }


    /// <summary>
    /// Data struct for Ploppable RICO Revisited UI category names.
    /// </summary>
    internal class UICategories
    {
        internal readonly string[] names =
        {
            Translations.Translate("PRR_UIC_REL"),
            Translations.Translate("PRR_UIC_REH"),
            Translations.Translate("PRR_UIC_COL"),
            Translations.Translate("PRR_UIC_COH"),
            Translations.Translate("PRR_UIC_OFF"),
            Translations.Translate("PRR_UIC_IND"),
            Translations.Translate("PRR_UIC_FAR"),
            Translations.Translate("PRR_UIC_FOR"),
            Translations.Translate("PRR_UIC_OIL"),
            Translations.Translate("PRR_UIC_ORE"),
            Translations.Translate("PRR_UIC_LEI"),
            Translations.Translate("PRR_UIC_TOU"),
            Translations.Translate("PRR_UIC_ORG"),
            Translations.Translate("PRR_UIC_ITC"),
            Translations.Translate("PRR_UIC_ECO"),
            Translations.Translate("PRR_UIC_NON")
        };
    }
}