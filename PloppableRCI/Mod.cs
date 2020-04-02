using ICities;
using UnityEngine;


namespace PloppableRICO
{
    public class PloppableRICOMod : IUserMod
	{
		public string Name => "RICO Revisited";

		public string Description => Translations.GetTranslation("Allows Plopping of RICO Buildings (fork of AJ3D's original with bugfixes and new features)");
	}
}
