using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(VRController), "Update")]
    internal class VRControllerUpdate
    {
        private static bool Prefix(VRPlatformHelper ____vrPlatformHelper)
        {
            if (____vrPlatformHelper == null)
                return false;
            else
                return true;
        }
    }

    [HarmonyPatch(typeof(SaberModelContainer), "Awake")]
    internal class SaberModelContainerAwake
    {
        private static bool Prefix(ISaberModelController ____saberModelController)
        {
            if (____saberModelController == null)
                return false;
            else
                return true;
        }
    }
}
