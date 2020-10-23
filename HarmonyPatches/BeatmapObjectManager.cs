using HarmonyLib;
using System.Reflection;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapObjectManager), "SpawnBasicNote")]
    static class BeatmapObjectManagerSpawnBasicNote
    {
        static bool Prefix(ref NoteData noteData)
        {
            if ((noteData.colorType == ColorType.ColorA) && Config.redToBlue)
            {
                PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                property.SetValue(noteData, ColorType.ColorB, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }
            else if ((noteData.colorType == ColorType.ColorB) && Config.blueToRed)
            {
                PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                property.SetValue(noteData, ColorType.ColorA, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }

            if ((noteData.colorType == ColorType.ColorA) && Config.noRed)
            {
                return false;
            }
            else if ((noteData.colorType == ColorType.ColorB) && Config.noBlue)
            {
                return false;
            }

            return true;
        }
    }
}
