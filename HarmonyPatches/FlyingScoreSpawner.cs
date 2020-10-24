using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(FlyingScoreSpawner), "SpawnFlyingScore")]
    static class FlyingScoreSpawnerSpawnFlyingScore
    {
        [HarmonyPriority(Priority.High)]
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
