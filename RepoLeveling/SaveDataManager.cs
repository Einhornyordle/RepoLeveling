using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Configuration;
using UnityEngine;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace RepoLeveling;

public static class SaveDataManager
{
    public static ConfigEntry<int> SaveCumulativeHaul;

    public static ConfigEntry<int> SaveDeathHeadBattery;
    public static ConfigEntry<int> SaveMapPlayerCount;
    public static ConfigEntry<int> SaveCrouchRest;
    public static ConfigEntry<int> SaveEnergy;
    public static ConfigEntry<int> SaveExtraJump;
    public static ConfigEntry<int> SaveGrabRange;
    public static ConfigEntry<int> SaveGrabStrength;
    public static ConfigEntry<int> SaveGrabThrow;
    public static ConfigEntry<int> SaveHealth;
    public static ConfigEntry<int> SaveSprintSpeed;
    public static ConfigEntry<int> SaveTumbleClimb;
    public static ConfigEntry<int> SaveTumbleLaunch;
    public static ConfigEntry<int> SaveTumbleWings;

    internal static void Initialize()
    {
        ConfigFile save = new(Path.Combine(Application.persistentDataPath, "REPOModData/RepoLeveling/save.cfg"), false);

        SaveCumulativeHaul = save.Bind("General", "CumulativeHaul", 0,
            new ConfigDescription(
                "The total value of all hauls you've ever completed. This value is used to calculate your available skill points. Increase this to cheat skill points. Set to 0 to reset your progress.",
                new AcceptableValueRange<int>(0, int.MaxValue)));

        SaveDeathHeadBattery = save.Bind("Skills", "DeathHeadBattery", 0,
            new ConfigDescription(
                "The amount of death head battery upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveMapPlayerCount = save.Bind("Skills", "MapPlayerCount", 0,
            new ConfigDescription(
                "The amount of map player count upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveCrouchRest = save.Bind("Skills", "CrouchRest", 0,
            new ConfigDescription(
                "The amount of crouch rest upgrades you've skilled. Set to 0 to regain spent skill points.",
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
        SaveTumbleClimb = save.Bind("Skills", "TumbleClimb", 0,
            new ConfigDescription(
                "The amount of tumble climb upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveTumbleLaunch = save.Bind("Skills", "TumbleLaunch", 0,
            new ConfigDescription(
                "The amount of tumble launch upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        SaveTumbleWings = save.Bind("Skills", "TumbleWings", 0,
            new ConfigDescription(
                "The amount of tumble wings upgrades you've skilled. Set to 0 to regain spent skill points.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
    }

    public static void ResetProgress()
    {
        SaveCumulativeHaul.Value = 0;
        ResetSkillPoints();
        RepoLeveling.Logger.LogDebug("Progress reset.");
    }

    public static void ResetSkillPoints()
    {
        SaveDeathHeadBattery.Value = 0;
        SaveMapPlayerCount.Value = 0;
        SaveCrouchRest.Value = 0;
        SaveEnergy.Value = 0;
        SaveExtraJump.Value = 0;
        SaveGrabRange.Value = 0;
        SaveGrabStrength.Value = 0;
        SaveGrabThrow.Value = 0;
        SaveHealth.Value = 0;
        SaveSprintSpeed.Value = 0;
        SaveTumbleClimb.Value = 0;
        SaveTumbleLaunch.Value = 0;
        SaveTumbleWings.Value = 0;
        RepoLeveling.Logger.LogDebug("Skill points reset.");
    }

    private static void ApplyUpgrade(
        Dictionary<string, int> upgradeDict,
        ConfigEntry<int> savedValue,
        Func<string, int, int> upgradeFunc)
    {
        upgradeDict.TryAdd(PlayerController.instance.playerSteamID, 0);

        upgradeFunc(PlayerController.instance.playerSteamID, savedValue.Value);
    }

    public static void ApplySkills()
    {
        RepoLeveling.Logger.LogDebug("Applying skill points...");

        ApplyUpgrade(StatsManager.instance.playerUpgradeDeathHeadBattery, SaveDeathHeadBattery,
            PunManager.instance.UpgradeDeathHeadBattery);

        ApplyUpgrade(StatsManager.instance.playerUpgradeMapPlayerCount, SaveMapPlayerCount,
            PunManager.instance.UpgradeMapPlayerCount);

        ApplyUpgrade(StatsManager.instance.playerUpgradeCrouchRest, SaveCrouchRest,
            PunManager.instance.UpgradePlayerCrouchRest);

        ApplyUpgrade(StatsManager.instance.playerUpgradeStamina, SaveEnergy,
            PunManager.instance.UpgradePlayerEnergy);

        ApplyUpgrade(StatsManager.instance.playerUpgradeExtraJump, SaveExtraJump,
            PunManager.instance.UpgradePlayerExtraJump);

        ApplyUpgrade(StatsManager.instance.playerUpgradeRange, SaveGrabRange,
            PunManager.instance.UpgradePlayerGrabRange);

        ApplyUpgrade(StatsManager.instance.playerUpgradeStrength, SaveGrabStrength,
            PunManager.instance.UpgradePlayerGrabStrength);

        ApplyUpgrade(StatsManager.instance.playerUpgradeThrow, SaveGrabThrow,
            PunManager.instance.UpgradePlayerThrowStrength);

        ApplyUpgrade(StatsManager.instance.playerUpgradeHealth, SaveHealth,
            PunManager.instance.UpgradePlayerHealth);

        ApplyUpgrade(StatsManager.instance.playerUpgradeSpeed, SaveSprintSpeed,
            PunManager.instance.UpgradePlayerSprintSpeed);

        ApplyUpgrade(StatsManager.instance.playerUpgradeTumbleClimb, SaveTumbleClimb,
            PunManager.instance.UpgradePlayerTumbleClimb);

        ApplyUpgrade(StatsManager.instance.playerUpgradeLaunch, SaveTumbleLaunch,
            PunManager.instance.UpgradePlayerTumbleLaunch);

        ApplyUpgrade(StatsManager.instance.playerUpgradeTumbleWings, SaveTumbleWings,
            PunManager.instance.UpgradePlayerTumbleWings);

        RepoLeveling.Logger.LogDebug("Skill points applied.");
    }

    /// <summary>
    /// Returns the total number of skill points available for the current CumulativeHaul.
    /// </summary>
    /// <returns>The total available skill points.</returns>
    public static int SkillPointsFromCumulativeHaul()
    {
        int skillPoints = (int)((-1 + Math.Sqrt(1 + 4 * SaveCumulativeHaul.Value / 75.0)) / 2);
        RepoLeveling.Logger.LogDebug($"Total skill points: {skillPoints}");
        return skillPoints;
    }

    /// <summary>
    /// Returns the number of skill points spent across all available skills.
    /// </summary>
    /// <returns>The number of skill points spent across all available skills.</returns>
    public static int TotalSpentSkillPoints()
    {
        int spentSkillPoints = SaveDeathHeadBattery.Value + SaveMapPlayerCount.Value + SaveCrouchRest.Value +
                               SaveEnergy.Value +
                               SaveExtraJump.Value + SaveGrabRange.Value +
                               SaveGrabStrength.Value + SaveGrabThrow.Value + SaveHealth.Value + SaveSprintSpeed.Value +
                               SaveTumbleClimb.Value + SaveTumbleLaunch.Value + SaveTumbleWings.Value;
        RepoLeveling.Logger.LogDebug($"Spent skill points: {spentSkillPoints}");
        return spentSkillPoints;
    }

    /// <summary>
    /// Returns the number of skill points available to be spent on new skills.
    /// </summary>
    /// <returns>The number of skill points available to be spent on new skills.</returns>
    public static int AvailableSkillPoints()
    {
        int availableSkillPoints = SkillPointsFromCumulativeHaul() - TotalSpentSkillPoints();
        RepoLeveling.Logger.LogDebug($"Available skill points: {availableSkillPoints}");
        return availableSkillPoints;
    }

    /// <summary>
    /// Returns the total haul required for the next skill point.
    /// </summary>
    /// <returns>The total haul required for the next skill point.</returns>
    public static int TotalCumulativeHaulNeededForNextSkillPoint()
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
    public static int NeededCumulativeHaulForNextSkillPoint()
    {
        int leftoverHaul = TotalCumulativeHaulNeededForNextSkillPoint() - SaveCumulativeHaul.Value;
        RepoLeveling.Logger.LogDebug($"Haul still needed for next skill point: {leftoverHaul}");
        return leftoverHaul;
    }
}