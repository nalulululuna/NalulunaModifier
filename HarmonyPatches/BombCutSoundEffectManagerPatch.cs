using HarmonyLib;
using System.Reflection;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(BombCutSoundEffectManager), "HandleNoteWasCut")]
    class BombCutSoundEffectManagerHandleNoteWasCut
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
