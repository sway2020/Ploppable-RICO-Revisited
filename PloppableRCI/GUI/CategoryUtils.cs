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

    internal class CategoryIcons
    {

        internal static readonly string[] atlases = {"Ingame", "Ingame" , "Ingame", "Ingame", "Ingame", "Ingame",

            "Thumbnails" , "Thumbnails","Thumbnails", "Thumbnails" };

        internal static readonly string[] spriteNames = {

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

        internal static readonly string[] tooltips = {
            Translations.GetTranslation("Monuments"),
            Translations.GetTranslation("Beautification"),
            Translations.GetTranslation("Education"),
            Translations.GetTranslation("Power"),
            Translations.GetTranslation("Water"),
            Translations.GetTranslation("Health"),
            Translations.GetTranslation("Residential"),
            Translations.GetTranslation("Commercial"),
            Translations.GetTranslation("Office"),
            Translations.GetTranslation("Industrial")
        };
    }
}