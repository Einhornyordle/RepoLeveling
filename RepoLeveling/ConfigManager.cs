using BepInEx.Configuration;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace RepoLeveling;

internal static class ConfigManager
{
    internal static ConfigEntry<float> TotalHaulRequirementMultiplier;

    internal static void Initialize()
    {
        TotalHaulRequirementMultiplier = RepoLeveling.Instance.Config.Bind("General", "TotalHaulRequirementMultiplier",
            1.0f,
            new ConfigDescription(
                "The multiplier applied to the total haul requirement for skill point gains. Increase it to slow skill point gain, decrease it to gain skill points faster. Note that this value is limited to 2 decimal places to prevent floating point precision errors and the upper and lower range is limited for better compatibility with the REPO Config mod.",
                new AcceptableValueRange<float>(0.01f, 10.0f)));
    }
}