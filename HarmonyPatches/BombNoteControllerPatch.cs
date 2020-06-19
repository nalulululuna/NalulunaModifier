using HarmonyLib;
using System.Reflection;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(BombNoteController), "HandleDidPassHalfJump")]
    class BombNoteControllerHandleDidPassHalfJump
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
