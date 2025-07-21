using System.Linq;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace RepoLeveling;

[HarmonyPatch(typeof(SemiFunc))]
internal static class SemiFuncPatch
{
    [HarmonyPrefix, HarmonyPatch(nameof(SemiFunc.OnSceneSwitch))]
    private static void OnSceneSwitch_Prefix()
    {
        if (!SemiFunc.RunIsArena()) return;
        RepoLeveling.Logger.LogDebug($"Level failed, saving haul of {StatsManager.instance.GetRunStatTotalHaul()}");
        SaveDataManager.SaveCumulativeHaul.Value += StatsManager.instance.GetRunStatTotalHaul();
        RepoLeveling.Logger.LogDebug($"New total: {SaveDataManager.SaveCumulativeHaul.Value}");
    }

    [HarmonyPostfix, HarmonyPatch(nameof(SemiFunc.OnSceneSwitch))]
    private static void OnSceneSwitch_Postfix()
    {
        if (!SemiFunc.RunIsLevel() || StatsManager.instance.FetchPlayerUpgrades(PlayerController.instance.playerSteamID)
                .Values.Any(val => val > 0))
            return;
        SaveDataManager.ApplySkills();
    }
}