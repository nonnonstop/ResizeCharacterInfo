using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResizeCharacterInfo
{
    [HarmonyPatch]
    public class CharacterCardUtilityPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterCardUtility), "PawnCardSize")]
        public static void UpdateSizePostfix(ref Vector2 __result)
        {
            var settings = LoadedModManager.GetMod<MyMod>().GetSettings<MyModSettings>();
            __result.y += settings.bioHeightOffset;
        }

        #if RW13
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
        public static IEnumerable<CodeInstruction> DrawCharacterCardTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;

            foreach (var instruction in instructions)
            {
                if (instruction.LoadsConstant(355f))
                {
                    found = true;
                    yield return instruction;
                    yield return CodeInstruction.Call(typeof(CharacterCardUtilityPatch), "GetSize");
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
                Log.Error("[ResizeCharacterInfo] Cannot find target instruction (DrawCharacterCard)");
        }

        public static float GetSize(float value)
        {
            if (!ITabPawnCharacterPatch.IsCharacterTab)
                return value;
            var settings = LoadedModManager.GetMod<MyMod>().GetSettings<MyModSettings>();
            return value + settings.bioHeightOffset;
        }
        #endif
    }
}
