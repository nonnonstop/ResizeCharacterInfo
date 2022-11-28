using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResizeCharacterInfo
{
    [HarmonyPatch]
    public class NeedsCardUtilityPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(NeedsCardUtility), "GetSize")]
        public static IEnumerable<CodeInstruction> GetSizeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var field = typeof(NeedsCardUtility).GetField("FullSize");
            var found = false;

            foreach (var instruction in instructions)
            {
                if (instruction.LoadsField(field))
                {
                    found = true;
                    yield return instruction;
                    yield return CodeInstruction.Call(typeof(NeedsCardUtilityPatch), "GetSize");
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!found)
                Log.Error("[ResizeCharacterInfo] Cannot find target instruction (DrawCharacterCard)");
        }

        public static Vector2 GetSize(Vector2 value)
        {
            var settings = LoadedModManager.GetMod<MyMod>().GetSettings<MyModSettings>();
            value.y += settings.needsHeightOffset;
            return value;
        }
    }
}
