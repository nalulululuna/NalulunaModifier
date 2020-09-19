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
    [HarmonyPatch(typeof(NoteController), "Init")]
	static class NoteControllerInit
	{
		static void Prefix(ref NoteData noteData, ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos, Transform ____noteTransform)
		{
			if (Config.noDirection)
			{
				if (noteData.cutDirection != NoteCutDirection.None)
				{
					noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Any);
				}
			}

			if ((Config.topNotesToFeet && (noteData.noteLineLayer == NoteLineLayer.Top)) ||
			  (Config.middleNotesToFeet && (noteData.noteLineLayer == NoteLineLayer.Upper)) ||
			  (Config.bottomNotesToFeet && (noteData.noteLineLayer == NoteLineLayer.Base)))
			{
				noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Any);
				____noteTransform.localScale = new Vector3(1f, 0.5f, 1f);
			}
			else
			{
				____noteTransform.localScale = Vector3.one;
			}

			if (Config.topNotesToFeet && (noteData.noteLineLayer == NoteLineLayer.Top))
			{
				moveStartPos = new Vector3(moveStartPos.x, 0.1f, moveStartPos.z);
				moveEndPos = new Vector3(moveEndPos.x, 0.1f, moveEndPos.z);
				jumpEndPos = new Vector3(jumpEndPos.x, 0.1f, jumpEndPos.z);
			}
			if (Config.middleNotesToFeet && (noteData.noteLineLayer == NoteLineLayer.Upper))
			{
				moveStartPos = new Vector3(moveStartPos.x, 0.1f, moveStartPos.z);
				moveEndPos = new Vector3(moveEndPos.x, 0.1f, moveEndPos.z);
				jumpEndPos = new Vector3(jumpEndPos.x, 0.1f, jumpEndPos.z);
			}
			if (Config.bottomNotesToFeet && (noteData.noteLineLayer == NoteLineLayer.Base))
			{
				moveStartPos = new Vector3(moveStartPos.x, 0.1f, moveStartPos.z);
				moveEndPos = new Vector3(moveEndPos.x, 0.1f, moveEndPos.z);
				jumpEndPos = new Vector3(jumpEndPos.x, 0.1f, jumpEndPos.z);
			}
		}

		static void Postfix(Transform ____noteTransform)
		{
			if (Config.centering)
			{
				____noteTransform.localScale = Vector3.one * 0.75f;
			}

			if (Config.flatNotes)
			{
				____noteTransform.localScale = new Vector3(____noteTransform.localScale.x, 0.5f, ____noteTransform.localScale.z);
			}
		}
	}
}
