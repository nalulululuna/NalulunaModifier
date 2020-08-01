using HarmonyLib;
using System.Reflection;

namespace NalulunaModifier
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
