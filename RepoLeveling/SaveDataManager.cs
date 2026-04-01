using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace RepoLeveling;

public static class SaveDataManager
{
    public record SkillDefinition(string ConfigKey, string DictionaryName, string ReadableName, int MaxValue = int.MaxValue);

    public static SkillDefinition[] SkillDefinitions =
    [
        new("DeathHeadBattery", "playerUpgradeDeathHeadBattery", "death head battery"),
        new("MapPlayerCount", "playerUpgradeMapPlayerCount", "map player count", 1),
        new("CrouchRest", "playerUpgradeCrouchRest", "crouch rest"),
        new("Energy", "playerUpgradeStamina", "stamina"),
        new("ExtraJump", "playerUpgradeExtraJump", "double jump"),
        new("GrabRange", "playerUpgradeRange", "range"),
        new("GrabStrength", "playerUpgradeStrength", "strength"),
        new("GrabThrow", "playerUpgradeThrow", "throw"),
        new("Health", "playerUpgradeHealth", "health"),
        new("SprintSpeed", "playerUpgradeSpeed", "sprint speed"),
        new("TumbleClimb", "playerUpgradeTumbleClimb", "tumble climb"),
        new("TumbleLaunch", "playerUpgradeLaunch", "tumble launch"),
        new("TumbleWings", "playerUpgradeTumbleWings", "tumble wings"),
    ];

    public static ConfigEntry<int> SaveCumulativeHaul = null!;

    public static Dictionary<string, ConfigEntry<int>> SkillEntries = new();

    internal static void Initialize()
    {
        ConfigFile save = new(Path.Combine(Application.persistentDataPath, "REPOModData/RepoLeveling/save.cfg"), false);

        SaveCumulativeHaul = save.Bind("General", "CumulativeHaul", 0,
            new ConfigDescription(
                "The total value of all hauls you've ever completed. Increase this to cheat skill points. Set to 0 to reset progress.",
                new AcceptableValueRange<int>(0, int.MaxValue)));

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
    public static void ResetProgress()
    {
        SaveCumulativeHaul.Value = 0;
        ResetSpentSkillPoints();
        RepoLeveling.Logger.LogDebug("Progress reset.");
    }

    /// <summary>
    /// Resets spent skill points for redistribution.
    /// </summary>
    public static void ResetSpentSkillPoints()
    {
        foreach (ConfigEntry<int> entry in SkillEntries.Values)
            entry.Value = 0;
        RepoLeveling.Logger.LogDebug("Skill points reset.");
    }

    /// <summary>
    /// Applies distributed skill points to the player.
    /// </summary>
    /// <param name="force">Skip checking if the player already has any upgrades applied.</param>
    public static void ApplySkills(bool force = false)
    {
        if (!force && StatsManager.instance.FetchPlayerUpgrades(PlayerAvatar.instance.steamID).Values
                .Any(v => v != 0)) return;

        RepoLeveling.Logger.LogDebug("Applying skill points...");

        foreach (SkillDefinition skill in SkillDefinitions)
        {
            //TODO Clients should also receive skills
            PunManager.instance.UpdateStat(skill.DictionaryName, PlayerController.instance.playerSteamID,
                SkillEntries[skill.ConfigKey].Value);
        }

        RepoLeveling.Logger.LogDebug("Skill points applied.");
    }

    /// <summary>
    /// Returns the total number of skill points available for the current CumulativeHaul.
    /// </summary>
    /// <returns>The total available skill points.</returns>
    public static int SkillPointsFromCumulativeHaul()
    {
        int skillPoints = (int)Math.Round((-1 + Math.Sqrt(1 + 4 * SaveCumulativeHaul.Value /
            (75.0f * ConfigManager.TotalHaulRequirementMultiplier.Value))) / 2);
        RepoLeveling.Logger.LogDebug($"Total skill points: {skillPoints}");
        return skillPoints;
    }

    /// <summary>
    /// Returns the number of skill points spent across all available skills.
    /// </summary>
    /// <returns>The number of skill points spent across all available skills.</returns>
    public static int TotalSpentSkillPoints()
    {
        int spent = SkillEntries.Values.Sum(e => e.Value);
        RepoLeveling.Logger.LogDebug($"Spent skill points: {spent}");
        return spent;
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
        int neededHaul = (int)Math.Round(75 * nextSkillPoint * (nextSkillPoint + 1) *
                                         ConfigManager.TotalHaulRequirementMultiplier.Value);
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