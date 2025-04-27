using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace RepoLeveling;

[BepInPlugin("Einhornyordle.RepoLeveling", "RepoLeveling", "0.1.1")]
public class RepoLeveling : BaseUnityPlugin
{
    private static RepoLeveling Instance { get; set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;

    // ReSharper disable once InconsistentNaming
    private ManualLogSource _logger => base.Logger;
    private Harmony? Harmony { get; set; }

    private void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();

        SaveDataManager.Initialize();
        SkillsMenu.Initialize();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    private void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }
}