using HarmonyLib;

namespace RepoLeveling;

[HarmonyPatch(typeof(PlayerAvatar))]
internal static class PatchPlayerAvatar
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerAvatar.Start))]
    private static void Start_Postfix() => PatchPunManager.SyncSkills();
}