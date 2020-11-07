using HarmonyLib;

namespace NalulunaModifier.HarmonyPatches
{
    [HarmonyPatch(typeof(PlayerAllOverallStatsData.PlayerOverallStatsData), "UpdateWithLevelCompletionResults")]
    internal static class PlayerAllOverallStatsDataUpdateWithLevelCompletionResults
    {
        static bool Prefix()
        {
            return !(Config.disableStatistics && Config.feet);
        }
    }
}
