using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResizeCharacterInfo
{
    [HarmonyPatch]
    public class ITabPawnCharacterPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ITab_Pawn_Character), "FillTab")]
        public static IEnumerable<CodeInstruction> FillTabPatch(IEnumerable<CodeInstruction> instructions)
        {
            var sizeMethod = typeof(CharacterCardUtility).GetMethod("PawnCardSize");
            var sizePatchMethod = typeof(ITabPawnCharacterPatch).GetMethod("PawnCardSize");
            var foundSizeMethod = false;

            #if RW13
            var drawMethod = typeof(CharacterCardUtility).GetMethod("DrawCharacterCard");
            var drawPatchMethod = typeof(ITabPawnCharacterPatch).GetMethod("DrawCharacterCard");
            var foundDrawMethod = false;
            #endif
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(sizeMethod))
                {
                    foundSizeMethod = true;
                    yield return new CodeInstruction(OpCodes.Call, sizePatchMethod);
                }
                #if RW13
                else if (instruction.Calls(drawMethod))
                {
                    foundDrawMethod = true;
                    yield return new CodeInstruction(OpCodes.Call, drawPatchMethod);
                }
                #endif
                else
                {
                    yield return instruction;
                }
            }

            if (!foundSizeMethod)
                Log.Error("[ResizeCharacterInfo] Cannot find target instruction (FillTab: PawnCardSize)");
            #if RW13
            if (!foundDrawMethod)
                Log.Error("[ResizeCharacterInfo] Cannot find target instruction (FillTab: DrawCharacterCard)");
            #endif
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ITab_Pawn_Character), "UpdateSize")]
        public static IEnumerable<CodeInstruction> UpdateSizePatch(IEnumerable<CodeInstruction> instructions)
        {
            var sizeMethod = typeof(CharacterCardUtility).GetMethod("PawnCardSize");
            var sizePatchMethod = typeof(ITabPawnCharacterPatch).GetMethod("PawnCardSize");

            var foundSizeMethod = false;
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(sizeMethod))
                {
                    foundSizeMethod = true;
                    yield return new CodeInstruction(OpCodes.Call, sizePatchMethod);
                }
                else
                {
                    yield return instruction;
                }
            }

            if (!foundSizeMethod)
                Log.Error("[ResizeCharacterInfo] Cannot find target instruction (UpdateSize: PawnCardSize)");
        }

        #if RW13
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
        public static void DrawCharacterCard(Rect rect, Pawn pawn, Action randomizeCallback = null, Rect creationRect = default(Rect))
        {
            #pragma warning disable CS8321
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            #pragma warning restore CS8321
            {
                var getHeightMethod = typeof(ITabPawnCharacterPatch).GetMethod("GetBioLeftHeight");

                var found = false;
                foreach (var instruction in instructions)
                {
                    if (instruction.LoadsConstant(355f))
                    {
                        found = true;
                        yield return new CodeInstruction(OpCodes.Call, getHeightMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }

                if (!found)
                    Log.Error("[ResizeCharacterInfo] Cannot find target instruction (DrawCharacterCard)");
            }
        }

        public static float GetBioLeftHeight()
        {
            var settings = LoadedModManager.GetMod<MyMod>().GetSettings<MyModSettings>();
            return settings.bioLeftHeight;
        }
        #endif

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(CharacterCardUtility), "PawnCardSize")]
        public static Vector2 PawnCardSize(Pawn pawn)
        {
            #pragma warning disable CS8321
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            #pragma warning restore CS8321
            {
                var baseSizeField = typeof(CharacterCardUtility).GetField("BasePawnCardSize");
                var getSizeMethod = typeof(ITabPawnCharacterPatch).GetMethod("GetBaseSize");

                var found = false;
                foreach (var instruction in instructions)
                {
                    if (instruction.LoadsField(baseSizeField))
                    {
                        found = true;
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call, getSizeMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }

                if (!found)
                    Log.Error("[ResizeCharacterInfo] Cannot find target instruction (PawnCardSize)");

            }
            return new Vector2();
        }

        public static Vector2 GetBaseSize(Vector2 size)
        {
            var settings = LoadedModManager.GetMod<MyMod>().GetSettings<MyModSettings>();
            size.y = size.y - MyModSettings.DefaultBioLeftHeight + settings.bioLeftHeight;
            return size;
        }
    }
}
