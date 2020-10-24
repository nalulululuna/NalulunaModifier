using HarmonyLib;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteCutHapticEffect), "HitNote")]
    static class NoteCutHapticEffectHitNote
    {
        [HarmonyPriority(Priority.High)]
        static void Prefix(ref SaberType saberType)
        {
            if (Config.vacuum)
            {
                saberType = SaberType.SaberB;
            }

            if (Config.ninjaMaster)
            {
                //Logger.log.Debug(NoteCutterCut.currentSaber.name);
                if (NoteCutterCut.currentSaber.name == NalulunaModifierController.saberLeft2Name ||
                    NoteCutterCut.currentSaber.name == NalulunaModifierController.saberRight2Name)
                {
                    saberType = saberType == SaberType.SaberA ? SaberType.SaberB : SaberType.SaberA;
                }
            }
        }
    }
}
