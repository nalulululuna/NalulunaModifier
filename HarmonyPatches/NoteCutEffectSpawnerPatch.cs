using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(NoteCutEffectSpawner), "SpawnNoteCutEffect")]
    static class NoteCutEffectSpawnerSpawnNoteCutEffect
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }

    [HarmonyPatch(typeof(NoteCutEffectSpawner), "SpawnBombCutEffect")]
    static class NoteCutEffectSpawnerSpawnSpawnBombCutEffect
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
