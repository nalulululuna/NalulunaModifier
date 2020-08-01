using HarmonyLib;
using System.Reflection;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(BeatmapObjectManager), "SpawnBasicNote")]
    static class BeatmapObjectManagerSpawnBasicNote
    {
        static bool Prefix(ref NoteData noteData)
        {
            if ((noteData.noteType == NoteType.NoteA) && Config.redToBlue)
            {
                PropertyInfo property = typeof(NoteData).GetProperty("noteType");
                property.SetValue(noteData, NoteType.NoteB, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }
            else if ((noteData.noteType == NoteType.NoteB) && Config.blueToRed)
            {
                PropertyInfo property = typeof(NoteData).GetProperty("noteType");
                property.SetValue(noteData, NoteType.NoteA, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }

            if ((noteData.noteType == NoteType.NoteA) && Config.noRed)
            {
                return false;
            }
            else if ((noteData.noteType == NoteType.NoteB) && Config.noBlue)
            {
                return false;
            }

            return true;
        }
    }
}
