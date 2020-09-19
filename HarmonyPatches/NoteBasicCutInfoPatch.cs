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

        static void Postfix(NoteType noteType, ref bool directionOK, ref bool speedOK, ref bool saberTypeOK)
        {
            if (Config.vacuum && (noteType != NoteType.Bomb))
            {
                directionOK = true;
                saberTypeOK = true;
            }
        }
    }
}
