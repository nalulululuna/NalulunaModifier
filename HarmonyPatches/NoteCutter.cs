using HarmonyLib;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteCutter), "Cut")]
    static class NoteCutterCut
    {
        internal static Saber currentSaber;

        static void Prefix(Saber saber)
        {
            currentSaber = saber;
        }
    }
}
