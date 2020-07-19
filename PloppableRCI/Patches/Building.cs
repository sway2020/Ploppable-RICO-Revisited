using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;


namespace PloppableRICO
{
    /// <summary>
    /// Harmony transpiler to patch PrivateBuildingAI.SimulationStep calls to Building.CheckZoning, to implement 'no zoning checks' functionality.
    /// Done this way (instead of e.g. patching Building.CheckZoning directly) for compatibility with 81 tiles mod, which detours (!) Building.CheckZoning.
    /// </summary>
    [HarmonyPatch(typeof(PrivateBuildingAI), "SimulationStep")]
    public static class SimulationStepPatch
    {
        /// <summary>
        /// Harmony Transpiler - finds calls to Building.CheckZoning in PrivateBuildingAI.SimulationStep and replaces them with calls to our own 'detour-compatible-prefix'.
        /// </summary>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Replacement (patched) ILCode</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Debugging.Message("transpiler patching CheckZoning");

            // Get list of original opcodes and create our replacement list.
            List<CodeInstruction> originalCode = new List<CodeInstruction>(instructions);
            List<CodeInstruction> patchedCode = new List<CodeInstruction>();

            // Iterate through each opcode in the original CIL.
            for (int i = 0; i < originalCode.Count; i++)
            {
                // Get local reference.
                CodeInstruction thisInstruction = originalCode[i];


                // Look for call to Building.CheckZoning.
                if (thisInstruction.opcode == OpCodes.Call && thisInstruction.operand == AccessTools.Method(typeof(Building), "CheckZoning", parameters: new Type[] { typeof(ItemClass.Zone), typeof(ItemClass.Zone), typeof(bool) }))
                {
                    // Found one - just change the operand to our 'detour-compatibile-prefix'.
                    // The Building struct reference for the original method call should be left on the stack and will be picked up as the first argument of our custom method.
                    thisInstruction.operand = AccessTools.Method(typeof(SimulationStepPatch), "NewZoneCheck");

                    // Also add a new 'this' instance reference for our patch.
                    patchedCode.Add(new CodeInstruction(OpCodes.Ldarg_0));
                }

                // Add this instruction to our patched code.
                patchedCode.Add(thisInstruction);
            }

            // Return patched code.
            return patchedCode.AsEnumerable();
        }


        /// <summary>
        /// A 'detour-compatible-prefix' replacement for Building.CheckZoning calls.
        /// Our transpiler inserts calls to this in place of original calls to BuildingData.CheckZoning.
        /// </summary>
        /// <param name="buildingData">Original Building reference (left on stack as the original reference to the Building instance for the original method call)</param>
        /// <param name="zone1">Original method argument (untouched)</param>
        /// <param name="zone2">Original method argument (untouched)</param>
        /// <param name="allowCollapse">Original method (untouched)</param>
        /// <param name="_instance">Calling PrivateBuildingAI instance reference (added by our transpiler)</param>
        /// <returns>True if this is a building covered by 'no zoning checks', otherwise will return whatever the base method result is</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool NewZoneCheck(ref Building buildingData, ItemClass.Zone zone1, ItemClass.Zone zone2, bool allowCollapse, PrivateBuildingAI _instance)
        {
            // Do we have no zone checks selected?
            if (ModSettings.noZoneChecks)
            {
                // Test to see if this AI is one of ours.
                if (_instance != null && (_instance is GrowableResidentialAI || _instance is GrowableCommercialAI || _instance is GrowableOfficeAI || _instance is GrowableIndustrialAI || _instance is GrowableExtractorAI))
                {
                    Debugging.Message("saved from destruction!");
                    // It's one of ours; always return true (tell the game we're in a valid zone).
                    return true;
                }
            }

            // If we got here, this isn't a building covered by our settings: call original method and return its result.
            return buildingData.CheckZoning(zone1, zone2, allowCollapse);
        }
    }
}