using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace NalulunaModifier
{
    [HarmonyPatch(typeof(NoteCutEffectSpawner), "SpawnNoteCutEffect")]
    class NoteCutEffectSpawnerSpawnNoteCutEffect
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }

    [HarmonyPatch(typeof(NoteCutEffectSpawner), "SpawnBombCutEffect")]
    class NoteCutEffectSpawnerSpawnSpawnBombCutEffect
    {
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
