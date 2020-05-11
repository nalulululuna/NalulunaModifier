using HarmonyLib;
using UnityEngine;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(NoteMovement), "Init")]
    class NoteMovementInit
    {
        static void Prefix(ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos)
        {
            if (Config.parabola)
            {
                //Logger.log.Debug($"NoteMovementInit:1: moveStartPos={moveStartPos}, moveEndPos={moveEndPos}, jumpEndPos={jumpEndPos}");
                moveStartPos.z = moveStartPos.z * 2;
                moveEndPos.z = moveEndPos.z * 2;
                jumpEndPos.z = jumpEndPos.z * 2;

                moveStartPos.y = moveStartPos.y + Config.parabolaOffsetY;
                moveEndPos.y = moveEndPos.y + Config.parabolaOffsetY;
                jumpEndPos.y = jumpEndPos.y + Config.parabolaOffsetY;
                //Logger.log.Debug($"NoteMovementInit:2: moveStartPos={moveStartPos}, moveEndPos={moveEndPos}, jumpEndPos={jumpEndPos}");
            }
        }
    }
}
