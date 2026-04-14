using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace RepoLeveling;

[BepInPlugin("Einhornyordle.RepoLeveling", "RepoLeveling", "0.3.0")]
internal class RepoLeveling : BaseUnityPlugin
{
    internal static RepoLeveling Instance { get; set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;

    // ReSharper disable once InconsistentNaming
    private ManualLogSource _logger => base.Logger;
    private Harmony? Harmony { get; set; }

    internal Dictionary<string, Dictionary<string, int>> PlayerSkills = null!;

    private void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();

        PlayerSkills = new Dictionary<string, Dictionary<string, int>>();

        ManagerConfig.Initialize();
        ManagerSaveData.Initialize();
        MenuPageMain.Initialize();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    private void OnDestroy()
    {
        Harmony?.UnpatchSelf();
    }

    private void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }
}