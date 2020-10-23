using HarmonyLib;
using System.Reflection;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(BombNoteController), "HandleDidPassHalfJump")]
    static class BombNoteControllerHandleDidPassHalfJump
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
