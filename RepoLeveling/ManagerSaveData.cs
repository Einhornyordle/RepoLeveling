using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace RepoLeveling;

internal static class ManagerSaveData
{
    internal record SkillDefinition(
        string ConfigKey,
        string DictionaryName,
        string ReadableName,
        int MaxValue = int.MaxValue);

    internal static List<SkillDefinition> SkillDefinitions =
    [
        new("DeathHeadBattery", "playerUpgradeDeathHeadBattery", "death head battery"),
        new("MapPlayerCount", "playerUpgradeMapPlayerCount", "map player count", 1),
        new("CrouchRest", "playerUpgradeCrouchRest", "crouch rest"),
        new("Energy", "playerUpgradeStamina", "stamina"),
        new("ExtraJump", "playerUpgradeExtraJump", "double jump"),
        new("GrabRange", "playerUpgradeRange", "range"),
        new("GrabStrength", "playerUpgradeStrength", "strength"),
        new("Health", "playerUpgradeHealth", "health"),
        new("SprintSpeed", "playerUpgradeSpeed", "sprint speed"),
        new("TumbleClimb", "playerUpgradeTumbleClimb", "tumble climb"),
        new("TumbleLaunch", "playerUpgradeLaunch", "tumble launch"),
        new("TumbleWings", "playerUpgradeTumbleWings", "tumble wings"),
    ];

    internal static ConfigEntry<int> SaveCumulativeHaul = null!;

    internal static Dictionary<string, ConfigEntry<int>> SkillEntries = new();

    internal static void Initialize()
    {
        ConfigFile save = new(Path.Combine(Application.persistentDataPath, "REPOModData/RepoLeveling/save.cfg"), false);

        SaveCumulativeHaul = save.Bind("General", "CumulativeHaul", 0,
            new ConfigDescription(
                "The total value of all hauls you've ever completed. Increase this to cheat skill points. Set to 0 to reset progress.",
                new AcceptableValueRange<int>(0, int.MaxValue)));

        if (ManagerConfig.EnableThrowSkill.Value)
            SkillDefinitions.Add(new SkillDefinition("GrabThrow", "playerUpgradeThrow", "throw"));

        foreach (SkillDefinition skill in SkillDefinitions)
        {
            SkillEntries[skill.ConfigKey] = save.Bind("Skills", skill.ConfigKey, 0,
                new ConfigDescription(
                    $"The amount of {skill.ReadableName} upgrades you've skilled. Set to 0 to regain spent skill points.",
                    new AcceptableValueRange<int>(0, int.MaxValue)));
        }
    }

    /// <summary>
    /// Performs a factory reset of the save data.
    /// </summary>
    internal static void ResetProgress()
    {
        SaveCumulativeHaul.Value = 0;
        ResetSpentSkillPoints();
        RepoLeveling.Logger.LogDebug("Progress reset.");
    }

    /// <summary>
    /// Resets spent skill points for redistribution.
    /// </summary>
    internal static void ResetSpentSkillPoints()
    {
        foreach (ConfigEntry<int> entry in SkillEntries.Values)
            entry.Value = 0;
        RepoLeveling.Logger.LogDebug("Skill points reset.");
    }

    /// <summary>
    /// Returns the total number of skill points available for the current CumulativeHaul.
    /// </summary>
    /// <returns>The total available skill points.</returns>
    internal static int SkillPointsFromCumulativeHaul()
    {
        int skillPoints = (int)Math.Round((-1 + Math.Sqrt(1 + 4 * SaveCumulativeHaul.Value /
            (75.0f * ManagerConfig.TotalHaulRequirementMultiplier.Value))) / 2);
        RepoLeveling.Logger.LogDebug($"Total skill points: {skillPoints}");
        return skillPoints;
    }

    /// <summary>
    /// Returns the number of skill points spent across all available skills.
    /// </summary>
    /// <returns>The number of skill points spent across all available skills.</returns>
    internal static int TotalSpentSkillPoints()
    {
        int spent = SkillEntries.Values.Sum(e => e.Value);
        RepoLeveling.Logger.LogDebug($"Spent skill points: {spent}");
        return spent;
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
    internal static int TotalCumulativeHaulNeededForNextSkillPoint()
    {
        int nextSkillPoint = SkillPointsFromCumulativeHaul() + 1;
        int neededHaul = (int)Math.Round(75 * nextSkillPoint * (nextSkillPoint + 1) *
                                         ManagerConfig.TotalHaulRequirementMultiplier.Value);
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