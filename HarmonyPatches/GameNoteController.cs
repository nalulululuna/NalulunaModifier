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
    [HarmonyPatch(typeof(GameNoteController), "HandleCut")]
	static class GameNoteControllerHandleCut
	{
		static bool Prefix(GameNoteController __instance, Saber saber)
		{
			if (Config.ignoreBadColor ||
				(Config.fourSabers && (saber.name == NalulunaModifierController.saberFootLName || saber.name == NalulunaModifierController.saberFootRName)))
			{
				ColorType colorType = __instance.noteData.colorType;
				SaberType saberType = saber.saberType;
				bool saberTypeOK = ((colorType == ColorType.ColorA && saberType == SaberType.SaberA) || (colorType == ColorType.ColorB && saberType == SaberType.SaberB));
				return saberTypeOK;
			}
			else
			{
				return true;
			}
		}
	}
}
