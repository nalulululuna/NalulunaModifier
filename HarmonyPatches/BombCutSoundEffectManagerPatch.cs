using HarmonyLib;
using System.Reflection;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(BombCutSoundEffectManager), "HandleNoteWasCut")]
    static class BombCutSoundEffectManagerHandleNoteWasCut
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
