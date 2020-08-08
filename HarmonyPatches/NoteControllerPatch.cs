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
		static void Prefix(ref NoteData noteData)
		{
			if (Config.noDirection)
			{
				if (noteData.cutDirection != NoteCutDirection.None)
				{
					noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Any);
				}
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
