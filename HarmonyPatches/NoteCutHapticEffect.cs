using HarmonyLib;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteCutHapticEffect), "HitNote")]
    static class NoteCutHapticEffectHitNote
    {
        static void Prefix(ref SaberType saberType)
        {
            if (Config.vacuum)
            {
                saberType = SaberType.SaberB;
            }
        }
    }
}
