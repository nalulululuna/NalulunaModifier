using HarmonyLib;
using UnityEngine;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteMovement), "Init")]
    static class NoteMovementInit
    {
        [HarmonyPriority(Priority.VeryLow)]
        static void Prefix(ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos)
        {
            Logger.log.Debug($"NoteMovementInit:1: moveStartPos={moveStartPos}, moveEndPos={moveEndPos}, jumpEndPos={jumpEndPos}");

            if (Config.parabola)
            {
                //Logger.log.Debug($"NoteMovementInit:1: moveStartPos={moveStartPos}, moveEndPos={moveEndPos}, jumpEndPos={jumpEndPos}");
                moveStartPos.z = moveStartPos.z * 2f;
                moveEndPos.z = moveEndPos.z * 2f;
                jumpEndPos.z = jumpEndPos.z * 2f;

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

            if (Config.centering)
            {
                moveStartPos.x = moveStartPos.x / 2f;
                moveEndPos.x = moveEndPos.x / 2f;
                jumpEndPos.x = jumpEndPos.x / 2f;
            }

            if (Config.feet)
            {
                moveStartPos.y = Config.feetNotesY;
                moveEndPos.y = Config.feetNotesY;
                jumpEndPos.y = Config.feetNotesY;
            }
        }
    }
}
