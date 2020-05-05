using UnityEngine;
using HarmonyLib;


namespace PloppableRICO
{
    /// <summary>
    /// Class to manage the mod's Harmony patches.
    /// </summary>
    public static class Patcher
    {
        // Unique harmony identifier.
        private const string harmonyID = "com.github.algernon-A.csl.ploppablericorevisited";

        // Instance and flag.
        private static Harmony harmonyInstance;
        private static bool patched = false;


        /// <summary>
        /// Apply all Harmony patches.
        /// </summary>
        public static void PatchAll()
        {
            // Don't do anything if already patched.
            if (!patched)
            {
                // Create harmony instance.
                Debug.Log("RICO Revisited v" + PloppableRICOMod.version + ": deploying Harmony patches.");
                harmonyInstance = new Harmony(harmonyID);

                // Apply all annotated patches and update flag.
                harmonyInstance.PatchAll();
                patched = true;
            }
        }


        public static void UnpatchAll()
        {
            // Only unapply if patches appplied.
            if (patched)
            {
                Debug.Log("RICO Revisited: reverting Harmony patches.");

                // Unapply patches, but only with our HarmonyID.
                harmonyInstance.UnpatchAll(harmonyID);
                patched = false;
            }
        }
    }
}