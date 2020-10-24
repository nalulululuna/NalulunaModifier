using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(FlyingSpriteSpawner), "SpawnFlyingSprite")]
    static class FlyingSpriteSpawnerSpawnFlyingSprite
    {
        [HarmonyPriority(Priority.High)]
        static bool Prefix()
        {
            return !Config.vacuum;
        }
    }
}
