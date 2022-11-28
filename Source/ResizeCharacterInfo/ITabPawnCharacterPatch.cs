#if RW13
using HarmonyLib;
using RimWorld;

namespace ResizeCharacterInfo
{
    [HarmonyPatch]
    public class ITabPawnCharacterPatch
    {
        public static bool IsCharacterTab = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ITab_Pawn_Character), "FillTab")]
        public static void FillTabPrefix()
        {
            IsCharacterTab = true;
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(ITab_Pawn_Character), "FillTab")]
        public static void FillTabFinalizer()
        {
            IsCharacterTab = false;
        }
    }
}
#endif
