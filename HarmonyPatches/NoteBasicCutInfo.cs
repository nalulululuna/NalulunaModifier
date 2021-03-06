﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteBasicCutInfo), "GetBasicCutInfo")]
	static class NoteBasicCutInfoGetBasicCutInfo
	{
        static void Prefix(ref float saberBladeSpeed)
        {
            if (Config.boxing || Config.headbang || Config.vacuum || Config.noDirection || Config.fourSabers)
            {
                saberBladeSpeed = 3.0f;
            }
            else if (Config.feet)
            {
                saberBladeSpeed = saberBladeSpeed * 2f;
            }
        }

        static void Postfix(ColorType colorType, ref bool directionOK, ref bool speedOK, ref bool saberTypeOK)
        {
            if (Config.vacuum && (colorType != ColorType.None))
            {
                directionOK = true;
                saberTypeOK = true;
            }

            if (Config.oneColorRed && (colorType != ColorType.None))
            {
                saberTypeOK = true;
            }
        }
    }
}
