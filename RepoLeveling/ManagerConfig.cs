using System;
using BepInEx.Configuration;

namespace RepoLeveling;

internal static class ManagerConfig
{
    internal static ConfigEntry<float> TotalHaulRequirementMultiplier = null!;
    internal static ConfigEntry<bool> EnableThrowSkill = null!;

    internal static void Initialize()
    {
        TotalHaulRequirementMultiplier = RepoLeveling.Instance.Config.Bind("General", "TotalHaulRequirementMultiplier",
            1.0f,
            new ConfigDescription(
                "The multiplier applied to the total haul requirement for skill point gains. Increase it to slow skill point gain, decrease it to gain skill points faster. Note that this value is limited to 2 decimal places to prevent floating point precision errors and the upper and lower range is limited for better compatibility with the REPO Config mod.",
                new AcceptableValueRange<float>(0.01f, 10.00f)));
        EnableThrowSkill =
            RepoLeveling.Instance.Config.Bind("General", "EnableThrowSkill", true, new ConfigDescription(
                "The game contains references to a currently unused throw upgrade. With this option, you can choose if you want the related skill to be available or not. This setting will NOT make the upgrade available in-game!."));
    }

    internal static void Reload()
    {
        RepoLeveling.Instance.Config.Reload();
        TotalHaulRequirementMultiplier.Value =
            (float)Math.Round(TotalHaulRequirementMultiplier.Value, 2);
    }
}