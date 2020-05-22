using HarmonyLib;
using UnityEngine;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(NoteJump), "ManualUpdate")]
    class NoteJumpManualUpdate
    {
        //static int tick = 0;
        public static Vector3 center = new Vector3(0, 0.9f, 0);

        static void Postfix(
            NoteJump __instance,
            ref Vector3 __result,
            AudioTimeSyncController ____audioTimeSyncController,
            float ____beatTime,
            float ____jumpDuration,
            Vector3 ____localPosition,
            Vector3 ____startPos,
            Vector3 ____endPos,
            Quaternion ____worldRotation)
        {
            if (Config.parabola)
            {
                float time = ____audioTimeSyncController.songTime - ____beatTime + ____jumpDuration * 0.5f;
                float amount = time / ____jumpDuration;
                float amountHalf = amount - 0.25f;
                ____localPosition.y = ____localPosition.y - 60f * (amountHalf * amountHalf - 0.0625f);
                //____localPosition.z = playerController.MoveTowardsHead(____startPos.z, ____endPos.z, ____inverseWorldRotation, num2);
                ____localPosition.z = Mathf.Lerp(____startPos.z, ____endPos.z, amount);

                __result = ____worldRotation * ____localPosition;
                __instance.transform.position = __result;
            }

            if (Config.centering)
            {
                ____localPosition.x = ____localPosition.x / 2f + center.x / 2f;
                ____localPosition.y = ____localPosition.y / 2f + center.y / 2f;
                ____localPosition.y = ____localPosition.y + 0.3f;
                __result = ____worldRotation * ____localPosition;
                __instance.transform.position = __result;
            }

            //if (tick++ % 9 == 0) Logger.log.Debug($"NoteJumpManualUpdate: num2={num2}, __result={__result}, ____startPos={____startPos}, ____endPos={____endPos}");
        }
    }
}
