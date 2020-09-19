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
    [HarmonyPatch(typeof(GameNoteController), "HandleCut")]
	static class GameNoteControllerHandleCut
	{
		static bool Prefix(GameNoteController __instance, Saber saber)
		{
			if (Config.ignoreBadColor || Config.fourSabers)
			{
				NoteType noteType = __instance.noteData.noteType;
				SaberType saberType = saber.saberType;
				bool saberTypeOK = ((noteType == NoteType.NoteA && saberType == SaberType.SaberA) || (noteType == NoteType.NoteB && saberType == SaberType.SaberB));
				return saberTypeOK;
			}
			else
			{
				return true;
			}
		}
	}
}
