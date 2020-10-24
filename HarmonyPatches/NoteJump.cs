using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteJump), "Init")]
    [HarmonyPriority(Priority.VeryLow)]
    static class NoteJumpInit
    {
        static void Postfix(
            ref Quaternion ____middleRotation,
            ref Quaternion ____endRotation,
            Vector3 startPos)
        {
            if (Config.noDirection || (Config.fourSabers && startPos.y == Config.feetNotesY))
            {
                ____middleRotation = Quaternion.identity;
                ____endRotation = Quaternion.identity;
            }
        }
    }

    [HarmonyPatch(typeof(NoteJump), "ManualUpdate")]
    [HarmonyPriority(Priority.VeryLow)]
    static class NoteJumpManualUpdate
    {
        //static int tick = 0;
        static SaberManager _saberManager = null;

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
                float time = ____audioTimeSyncController.songTime - ____beatTime + ____jumpDuration * 0.5f;
                float amount = time / ____jumpDuration;
                ____localPosition.y = Mathf.Lerp(____localPosition.y, ____localPosition.y / 2f + Config.centeringBaseY, Mathf.Clamp01(amount * 10f));
                __result = ____worldRotation * ____localPosition;
                __instance.transform.position = __result;
            }

            //if (tick++ % 9 == 0) Logger.log.Debug($"NoteJumpManualUpdate: num2={num2}, __result={__result}, ____startPos={____startPos}, ____endPos={____endPos}");

            if (Config.vacuum)
            {
                if (_saberManager == null)
                {
                    _saberManager = Resources.FindObjectsOfTypeAll<SaberManager>().FirstOrDefault();
                }

                if (_saberManager != null)
                {
                    float time = ____audioTimeSyncController.songTime - ____beatTime + ____jumpDuration * 0.5f;
                    float amount = time / ____jumpDuration;
                    amount = Mathf.Clamp01(amount * 2);

                    if (amount > 0.7f)
                    {
                        Vector3 endPos = _saberManager.rightSaber.saberBladeTopPos;
                        float t = (amount - 0.5f) * 2;
                        t = t * t * t * t;
                        ____localPosition.x = Mathf.Lerp(____localPosition.x, endPos.x, t);
                        ____localPosition.y = Mathf.Lerp(____localPosition.y, endPos.y, t);
                        __result = ____worldRotation * ____localPosition;
                        __instance.transform.position = __result;
                    }
                }
            }

            if (Config.feet)
            {
                ____localPosition.y = Config.feetNotesY;
                __result = ____worldRotation * ____localPosition;
                __instance.transform.position = __result;
            }

            if (Config.topNotesToFeet || Config.middleNotesToFeet || Config.bottomNotesToFeet)
            {
                if (____startPos.y == Config.feetNotesY)
                {
                    ____localPosition.y = Config.feetNotesY;
                    __result = ____worldRotation * ____localPosition;
                    __instance.transform.position = __result;
                }
            }
        }
    }
}
