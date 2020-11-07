using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorManager), "ColorForSaberType")]
	static class ColorManagerColorForSaberType
    {
        static void Prefix(ref SaberType type)
        {
            if (Config.oneColorRed && type == SaberType.SaberB && NalulunaModifierController.instance?.inGame == true)
            {
                type = SaberType.SaberA;
            }
        }
    }

    [HarmonyPatch(typeof(ColorManager), "EffectsColorForSaberType")]
    static class ColorManagerEffectsColorForSaberType
    {
        static void Prefix(ref SaberType type)
        {
            if (Config.oneColorRed && type == SaberType.SaberB && NalulunaModifierController.instance?.inGame == true)
            {
                type = SaberType.SaberA;
            }
        }
    }
}
