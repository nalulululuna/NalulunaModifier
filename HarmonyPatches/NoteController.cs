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
    [HarmonyPatch(typeof(NoteController), "Init")]
	static class NoteControllerInit
	{
		static void Prefix(ref NoteData noteData, ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos, Transform ____noteTransform)
		{
			NoteLineLayer noteLineLayer;
			if ((int)noteData.noteLineLayer <= 2)
			{
				noteLineLayer = noteData.noteLineLayer;
			}
			else if ((int)noteData.noteLineLayer < 1667)
			{
				noteLineLayer = NoteLineLayer.Base;
			}
			else if ((int)noteData.noteLineLayer < 2334)
			{
				noteLineLayer = NoteLineLayer.Upper;
			}
			else
			{
				noteLineLayer = NoteLineLayer.Top;
			}

			if (Config.noDirection)
			{
				if (noteData.cutDirection != NoteCutDirection.None)
				{
					noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Any);
				}
			}

			if ((Config.topNotesToFeet && (noteLineLayer == NoteLineLayer.Top)) ||
				(Config.middleNotesToFeet && (noteLineLayer == NoteLineLayer.Upper)) ||
				(Config.bottomNotesToFeet && (noteLineLayer == NoteLineLayer.Base)))
			{
				noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Any);
				____noteTransform.localScale = new Vector3(1f, 0.5f, 1f);
			}
			else
			{
				____noteTransform.localScale = Vector3.one;
			}

			if (Config.topNotesToFeet && (noteLineLayer == NoteLineLayer.Top))
			{
				moveStartPos = new Vector3(moveStartPos.x, Config.feetNotesY, moveStartPos.z);
				moveEndPos = new Vector3(moveEndPos.x, Config.feetNotesY, moveEndPos.z);
				jumpEndPos = new Vector3(jumpEndPos.x, Config.feetNotesY, jumpEndPos.z);
			}
			if (Config.middleNotesToFeet && (noteLineLayer == NoteLineLayer.Upper))
			{
				moveStartPos = new Vector3(moveStartPos.x, Config.feetNotesY, moveStartPos.z);
				moveEndPos = new Vector3(moveEndPos.x, Config.feetNotesY, moveEndPos.z);
				jumpEndPos = new Vector3(jumpEndPos.x, Config.feetNotesY, jumpEndPos.z);
			}
			if (Config.bottomNotesToFeet && (noteLineLayer == NoteLineLayer.Base))
			{
				moveStartPos = new Vector3(moveStartPos.x, Config.feetNotesY, moveStartPos.z);
				moveEndPos = new Vector3(moveEndPos.x, Config.feetNotesY, moveEndPos.z);
				jumpEndPos = new Vector3(jumpEndPos.x, Config.feetNotesY, jumpEndPos.z);
			}
		}

		static void Postfix(Transform ____noteTransform)
		{
			if (Config.centering)
			{
				____noteTransform.localScale = Vector3.one * Config.centeringNotesScale;
				if (Config.flatNotes)
				{
					____noteTransform.localScale = new Vector3(____noteTransform.localScale.x, ____noteTransform.localScale.y * 0.5f, ____noteTransform.localScale.z);
				}
			}
			else if (Config.flatNotes)
			{
				____noteTransform.localScale = new Vector3(____noteTransform.localScale.x, 0.5f, ____noteTransform.localScale.z);
			}
		}
	}
}
