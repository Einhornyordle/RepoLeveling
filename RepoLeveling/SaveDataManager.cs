using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Configuration;
using Photon.Pun;
using UnityEngine;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace RepoLeveling;

internal static class SaveDataManager
{
    internal static ConfigEntry<int> SaveCumulativeHaul;

    internal static ConfigEntry<int> SaveMapPlayerCount;
    internal static ConfigEntry<int> SaveEnergy;
    internal static ConfigEntry<int> SaveExtraJump;
    internal static ConfigEntry<int> SaveGrabRange;
    internal static ConfigEntry<int> SaveGrabStrength;
    internal static ConfigEntry<int> SaveGrabThrow;
    internal static ConfigEntry<int> SaveHealth;
    internal static ConfigEntry<int> SaveSprintSpeed;
    internal static ConfigEntry<int> SaveTumbleLaunch;

    internal static void Initialize()
    {
        ConfigFile save =
            new ConfigFile(Path.Combine(Application.persistentDataPath, "REPOModData/RepoLeveling/save.cfg"), false);

        SaveCumulativeHaul = save.Bind("General", "CumulativeHaul", 0,
            new ConfigDescription(
                "The total value of all hauls you've ever completed. This value is used to calculate your available skill points. Increase this to cheat skill points. Set to 0 to reset your progress.",
                new AcceptableValueRange<int>(0, int.MaxValue)));

        SaveMapPlayerCount = save.Bind("Skills", "MapPlayerCount", 0,
            new ConfigDescription(
                "The amount of map player count upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveEnergy = save.Bind("Skills", "Energy", 0,
            new ConfigDescription(
                "The amount of stamina upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveExtraJump = save.Bind("Skills", "ExtraJump", 0,
            new ConfigDescription(
                "The amount of double jump upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveGrabRange = save.Bind("Skills", "GrabRange", 0,
            new ConfigDescription("The amount of range upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveGrabStrength = save.Bind("Skills", "GrabStrength", 0,
            new ConfigDescription(
                "The amount of strength upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveGrabThrow = save.Bind("Skills", "GrabThrow", 0,
            new ConfigDescription("The amount of throw upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveHealth = save.Bind("Skills", "Health", 0,
            new ConfigDescription(
                "The amount of health upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveSprintSpeed = save.Bind("Skills", "SprintSpeed", 0,
            new ConfigDescription(
                "The amount of sprint speed upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveTumbleLaunch = save.Bind("Skills", "TumbleLaunch", 0,
            new ConfigDescription(
                "The amount of tumble launch upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
    }

    internal static void ResetProgress()
    {
        SaveCumulativeHaul.Value = 0;
        SaveMapPlayerCount.Value = 0;
        SaveEnergy.Value = 0;
        SaveExtraJump.Value = 0;
        SaveGrabRange.Value = 0;
        SaveGrabStrength.Value = 0;
        SaveGrabThrow.Value = 0;
        SaveHealth.Value = 0;
        SaveSprintSpeed.Value = 0;
        SaveTumbleLaunch.Value = 0;
        RepoLeveling.Logger.LogDebug("Progress reset.");
    }

    private static void ApplyUpgrade(
        Dictionary<string, int> upgradeDict,
        ConfigEntry<int> savedValue,
        Action<string> upgradeAction,
        string rpcName)
    {
        upgradeDict.TryAdd(PlayerController.instance.playerSteamID, 0);

        while (upgradeDict[PlayerController.instance.playerSteamID] < savedValue.Value)
        {
            ++upgradeDict[PlayerController.instance.playerSteamID];
            upgradeAction(PlayerController.instance.playerSteamID);
            if (SemiFunc.IsMultiplayer())
                PunManager.instance.photonView.RPC(rpcName, RpcTarget.Others, PlayerController.instance.playerSteamID,
                    upgradeDict[PlayerController.instance.playerSteamID]);
        }
    }

    internal static void ApplySkills()
    {
        RepoLeveling.Logger.LogDebug("Applying skill points...");

        ApplyUpgrade(StatsManager.instance.playerUpgradeMapPlayerCount, SaveMapPlayerCount,
            PunManager.instance.UpdateMapPlayerCountRightAway, "UpgradeMapPlayerCountRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeStamina, SaveEnergy,
            PunManager.instance.UpdateEnergyRightAway, "UpgradePlayerEnergyRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeExtraJump, SaveExtraJump,
            PunManager.instance.UpdateExtraJumpRightAway, "UpgradePlayerExtraJumpRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeRange, SaveGrabRange,
            PunManager.instance.UpdateGrabRangeRightAway, "UpgradePlayerGrabRangeRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeStrength, SaveGrabStrength,
            PunManager.instance.UpdateGrabStrengthRightAway, "UpgradePlayerGrabStrengthRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeThrow, SaveGrabThrow,
            PunManager.instance.UpdateThrowStrengthRightAway, "UpgradePlayerThrowStrengthRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeHealth, SaveHealth,
            PunManager.instance.UpdateHealthRightAway, "UpgradePlayerHealthRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeSpeed, SaveSprintSpeed,
            PunManager.instance.UpdateSprintSpeedRightAway, "UpgradePlayerSprintSpeedRPC");

        ApplyUpgrade(StatsManager.instance.playerUpgradeLaunch, SaveTumbleLaunch,
            PunManager.instance.UpdateTumbleLaunchRightAway, "UpgradePlayerTumbleLaunchRPC");

        RepoLeveling.Logger.LogDebug("Skill points applied.");
    }

    /// <summary>
    /// Returns the total number of skill points available for the current CumulativeHaul.
    /// </summary>
    /// <returns>The total available skill points.</returns>
    internal static int SkillPointsFromCumulativeHaul()
    {
        int skillPoints = (int)((-1 + Math.Sqrt(1 + 4 * SaveCumulativeHaul.Value / 75.0)) / 2);
        RepoLeveling.Logger.LogDebug($"Total skill points: {skillPoints}");
        return skillPoints;
    }

    /// <summary>
    /// Returns the number of skill points spent across all available skills.
    /// </summary>
    /// <returns>The number of skill points spent across all available skills.</returns>
    private static int TotalSpentSkillPoints()
    {
        int spentSkillPoints = SaveMapPlayerCount.Value + SaveEnergy.Value + SaveExtraJump.Value + SaveGrabRange.Value +
                               SaveGrabStrength.Value + SaveGrabThrow.Value + SaveHealth.Value + SaveSprintSpeed.Value +
                               SaveTumbleLaunch.Value;
        RepoLeveling.Logger.LogDebug($"Spent skill points: {spentSkillPoints}");
        return spentSkillPoints;
    }

    /// <summary>
    /// Returns the number of skill points available to be spent on new skills.
    /// </summary>
    /// <returns>The number of skill points available to be spent on new skills.</returns>
    internal static int AvailableSkillPoints()
    {
        int availableSkillPoints = SkillPointsFromCumulativeHaul() - TotalSpentSkillPoints();
        RepoLeveling.Logger.LogDebug($"Available skill points: {availableSkillPoints}");
        return availableSkillPoints;
    }

    /// <summary>
    /// Returns the total haul required for the next skill point.
    /// </summary>
    /// <returns>The total haul required for the next skill point.</returns>
    private static int TotalCumulativeHaulNeededForNextSkillPoint()
    {
        int nextSkillPoint = SkillPointsFromCumulativeHaul() + 1;
        int neededHaul = 75 * nextSkillPoint * (nextSkillPoint + 1);
        RepoLeveling.Logger.LogDebug($"Total haul needed for next skill point: {neededHaul}");
        return neededHaul;
    }

    /// <summary>
    /// Returns the haul amount still needed to gain the next skill point.
    /// </summary>
    /// <returns>The haul amount still needed to gain the next skill point</returns>
    internal static int NeededCumulativeHaulForNextSkillPoint()
    {
        int leftoverHaul = TotalCumulativeHaulNeededForNextSkillPoint() - SaveCumulativeHaul.Value;
        RepoLeveling.Logger.LogDebug($"Haul still needed for next skill point: {leftoverHaul}");
        return leftoverHaul;
    }
}