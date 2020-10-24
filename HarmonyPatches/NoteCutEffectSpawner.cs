using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteCutCoreEffectsSpawner), "SpawnNoteCutEffect")]
    static class NoteCutEffectSpawnerSpawnNoteCutEffect
    {
        [HarmonyPriority(Priority.High)]
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }

    [HarmonyPatch(typeof(NoteCutCoreEffectsSpawner), "SpawnBombCutEffect")]
    static class NoteCutEffectSpawnerSpawnSpawnBombCutEffect
    {
        [HarmonyPriority(Priority.High)]
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
