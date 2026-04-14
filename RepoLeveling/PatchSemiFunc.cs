using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace RepoLeveling;

[HarmonyPatch(typeof(SemiFunc))]
internal static class PatchSemiFunc
{
    [HarmonyPostfix, HarmonyPatch(nameof(SemiFunc.OnSceneSwitch))]
    private static void OnSceneSwitch_Postfix(bool _gameOver, bool _leaveGame)
    {
        if (_leaveGame)
        {
            RepoLeveling.Logger.LogDebug("Clearing skill cache.");
            RepoLeveling.Instance.PlayerSkills.Clear();
        }

        if (!SemiFunc.RunIsArena()) return;
        RepoLeveling.Logger.LogDebug($"Level failed, saving haul of {StatsManager.instance.GetRunStatTotalHaul()}");
        ManagerSaveData.SaveCumulativeHaul.Value += StatsManager.instance.GetRunStatTotalHaul();
        RepoLeveling.Logger.LogDebug($"New total: {ManagerSaveData.SaveCumulativeHaul.Value}");
    }
}