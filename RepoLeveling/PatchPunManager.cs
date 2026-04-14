using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace RepoLeveling;

[HarmonyPatch(typeof(PunManager))]
internal static class PatchPunManager
{
    [HarmonyPrefix, HarmonyPatch(nameof(PunManager.SyncAllDictionaries))]
    private static void SyncAllDictionaries_Prefix()
    {
        if (!SemiFunc.IsMasterClientOrSingleplayer() || !SemiFunc.RunIsLevel()) return;
        RepoLeveling.Logger.LogDebug("Skill distribution started");
        foreach (PlayerAvatar player in GameDirector.instance.PlayerList.Where(player =>
                     StatsManager.instance.FetchPlayerUpgrades(player.steamID).Values.All(v => v == 0)))
        {
            RepoLeveling.Logger.LogDebug($"Applying skills for {player.playerName}...");
            if (RepoLeveling.Instance.PlayerSkills.TryGetValue(player.steamID, out Dictionary<string, int>? skills))
            {
                foreach (ManagerSaveData.SkillDefinition skill in ManagerSaveData.SkillDefinitions)
                {
                    if (!skills.TryGetValue(skill.ConfigKey, out int value)) continue;
                    StatsManager.instance.DictionaryUpdateValue(skill.DictionaryName, player.steamID, value);
                    RepoLeveling.Logger.LogDebug($"{skill.ReadableName} is now {value}.");
                }
            }

            RepoLeveling.Logger.LogDebug($"Skills applied for {player.playerName}.");
        }

        RepoLeveling.Logger.LogDebug("Skill distribution finished");
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PunManager.Awake))]
    // ReSharper disable once InconsistentNaming
    private static void Awake_Postfix(PunManager __instance)
    {
        __instance.gameObject.AddComponent<NetworkingReceiver>();
    }

    internal static void SyncSkills()
    {
        string[] keys = new string[ManagerSaveData.SkillDefinitions.Count];
        int[] values = new int[ManagerSaveData.SkillDefinitions.Count];

        for (int i = 0; i < ManagerSaveData.SkillDefinitions.Count; i++)
        {
            ManagerSaveData.SkillDefinition skill = ManagerSaveData.SkillDefinitions[i];
            keys[i] = skill.ConfigKey;
            values[i] = ManagerSaveData.SkillEntries[skill.ConfigKey].Value;
        }

        if (SemiFunc.IsMasterClientOrSingleplayer())
        {
            PunManager.instance.GetComponent<NetworkingReceiver>()
                .SyncSkillsRPC(PlayerAvatar.instance.steamID, keys, values);
        }
        else
        {
            PunManager.instance.photonView.RPC(nameof(NetworkingReceiver.SyncSkillsRPC), RpcTarget.MasterClient,
                PlayerAvatar.instance.steamID, keys, values);
            RepoLeveling.Logger.LogDebug("Sent skills to host.");
        }
    }
}

public class NetworkingReceiver : MonoBehaviour
{
    [PunRPC]
    public void SyncSkillsRPC(string steamID, string[] keys, int[] values)
    {
        Dictionary<string, int> skills = new();
        for (int i = 0; i < keys.Length; i++)
            skills[keys[i]] = values[i];

        RepoLeveling.Instance.PlayerSkills[steamID] = skills;
        RepoLeveling.Logger.LogDebug($"Updated skills for {steamID}.");
    }
}