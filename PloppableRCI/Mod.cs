using ICities;


namespace PloppableRICO
{
    public class PloppableRICOMod : IUserMod
	{
		public static string version = "1.2.5";

		public string Name => "RICO Revisited " + version;

		public string Description => Translations.GetTranslation("Allows Plopping of RICO Buildings (fork of AJ3D's original with bugfixes and new features)");
	}
}
