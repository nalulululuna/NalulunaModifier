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
    [HarmonyPatch(typeof(NoteController))]
	[HarmonyPatch("Init")]
	internal static class NoteControllerInit
	{
		static void Postfix(Transform ____noteTransform)
		{
			if (Config.centering)
			{
				____noteTransform.localScale = Vector3.one * 0.75f;
			}
		}
	}
}
