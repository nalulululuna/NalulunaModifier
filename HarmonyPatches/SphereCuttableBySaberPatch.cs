using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace NalulunaModifier
{
    // not use for now
    /*
    [HarmonyPatch(typeof(SphereCuttableBySaber), "Awake")]
    static class SphereCuttableBySaberAwake
    {
        static void Prefix(SphereCollider ____collider)
        {
            // default radius = 0.18f
            //Logger.log.Debug($"radius={____collider.radius}");
            if (Config.feet)
            {
                 ____collider.radius = 0.09f;
            }         
        }
    }
    */
    
    [HarmonyPatch(typeof(SphereCuttableBySaber), "Cut")]
    static class SphereCuttableBySaberCut
    {
        static bool Prefix(SphereCuttableBySaber __instance, Saber saber, Vector3 cutPoint, SphereCollider ____collider)
        {
            if (Config.feet && __instance.canBeCut)
            {
                //Logger.log.Debug($"distance={Vector3.Distance(__instance.transform.position, saber.transform.position)}, instance={__instance.transform.position}, saber={saber.transform.position}, {saber.saberBladeBottomPos}, {saber.saberBladeTopPos}, cutPoint={cutPoint}");
                return (Vector3.Distance(__instance.transform.position, saber.transform.position) < ____collider.radius);
            }
            return true;
        }
    }
}
