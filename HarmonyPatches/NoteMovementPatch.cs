using HarmonyLib;
using UnityEngine;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(NoteMovement), "Init")]
    static class NoteMovementInit
    {
        static void Prefix(ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos)
        {
            if (Config.parabola)
            {
                //Logger.log.Debug($"NoteMovementInit:1: moveStartPos={moveStartPos}, moveEndPos={moveEndPos}, jumpEndPos={jumpEndPos}");
                moveStartPos.z = moveStartPos.z * 2;
                moveEndPos.z = moveEndPos.z * 2;
                jumpEndPos.z = jumpEndPos.z * 2;

                float offsetY = Config.parabolaOffsetY;
                if (Config.boxing)
                {
                    offsetY -= 0.3f;
                }
                moveStartPos.y = moveStartPos.y + offsetY;
                moveEndPos.y = moveEndPos.y + offsetY;
                jumpEndPos.y = jumpEndPos.y + offsetY;
                //Logger.log.Debug($"NoteMovementInit:2: moveStartPos={moveStartPos}, moveEndPos={moveEndPos}, jumpEndPos={jumpEndPos}");
            }

            if (Config.feet)
            {
                moveStartPos.y = 0.1f;
                moveEndPos.y = 0.1f;
                jumpEndPos.y = 0.1f;
            }
        }
    }
}
