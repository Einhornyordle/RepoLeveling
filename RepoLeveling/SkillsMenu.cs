using System;
using BepInEx.Configuration;
using MenuLib;
using MenuLib.MonoBehaviors;
using Sirenix.Utilities;
using UnityEngine;

namespace RepoLeveling;

public static class SkillsMenu
{
    private static REPOPopupPage _statisticsPage = null!;
    private static REPOPopupPage _skillsPage = null!;
    private static REPOLabel _availableSkillPoints = null!;
    private static REPOSlider[] _skillSliders = null!;

    internal static void Initialize()
    {
        MenuAPI.AddElementToMainMenu(parent =>
            MenuAPI.CreateREPOButton("Skills", CreateSkillsMenu, parent, new Vector2(0, 360)));
        MenuAPI.AddElementToEscapeMenu(parent =>
            MenuAPI.CreateREPOButton("Skills", CreateSkillsMenu, parent, new Vector2(0, 360)));
        MenuAPI.AddElementToLobbyMenu(parent =>
            MenuAPI.CreateREPOButton("Skills", CreateSkillsMenu, parent, new Vector2(0, 360)));
    }

    private static void CreateSkillsMenu()
    {
        RepoLeveling.Instance.Config.Reload();
        ConfigManager.TotalHaulRequirementMultiplier.Value =
            (float)Math.Round(ConfigManager.TotalHaulRequirementMultiplier.Value, 2);
        CreateStatisticsPage();
        CreateSkillsPage();
    }

    private static void CreateStatisticsPage()
    {
        _statisticsPage = MenuAPI.CreateREPOPopupPage("Statistics", REPOPopupPage.PresetSide.Left, false, true);
        REPOElement[] elements =
        [
            MenuAPI.CreateREPOLabel("Total Hauled:", _statisticsPage.transform, new Vector2(70, 270)),
            MenuAPI.CreateREPOLabel("Next SP in:", _statisticsPage.transform, new Vector2(70, 240)),
            MenuAPI.CreateREPOLabel("Total SP earned:", _statisticsPage.transform, new Vector2(70, 210)),
            MenuAPI.CreateREPOLabel("Available SP:", _statisticsPage.transform, new Vector2(70, 180)),
            MenuAPI.CreateREPOLabel("Current Haul:", _statisticsPage.transform, new Vector2(70, 150)),

            MenuAPI.CreateREPOLabel($"{SaveDataManager.SaveCumulativeHaul.Value}k", _statisticsPage.transform,
                new Vector2(260, 270)),
            MenuAPI.CreateREPOLabel($"{SaveDataManager.NeededCumulativeHaulForNextSkillPoint()}k",
                _statisticsPage.transform,
                new Vector2(260, 240)),
            MenuAPI.CreateREPOLabel($"{SaveDataManager.SkillPointsFromCumulativeHaul()}", _statisticsPage.transform,
                new Vector2(260, 210)),
            _availableSkillPoints = MenuAPI.CreateREPOLabel($"{SaveDataManager.AvailableSkillPoints()}",
                _statisticsPage.transform, new Vector2(260, 180)),
            MenuAPI.CreateREPOLabel($"{StatsManager.instance.GetRunStatTotalHaul()}k", _statisticsPage.transform,
                new Vector2(260, 150)),

            MenuAPI.CreateREPOLabel("Note: SP = Skill Points", _statisticsPage.transform, new Vector2(70, 60)),
            MenuAPI.CreateREPOButton("Reset progress", () => MenuAPI.OpenPopup("Reset Progress", Color.red,
                "Are you sure that you want to reset your progress? This cannot be undone!",
                () =>
                {
                    SaveDataManager.ResetProgress();
                    _statisticsPage.ClosePage(true);
                }), _statisticsPage.transform, new Vector2(60, 20)),
            MenuAPI.CreateREPOButton("Close", () => _statisticsPage.ClosePage(true), _statisticsPage.transform,
                new Vector2(280, 20))
        ];

        foreach (REPOElement element in elements)
        {
            _statisticsPage.AddElement(element.rectTransform, element.rectTransform.localPosition);
        }

        _statisticsPage.OpenPage(false);
    }

    private static void CreateSkillsPage()
    {
        _skillsPage = MenuAPI.CreateREPOPopupPage("Skills", REPOPopupPage.PresetSide.Right, false, false);

        _skillSliders = new REPOSlider[SaveDataManager.SkillDefinitions.Length];
        int availableSkillPoints = SaveDataManager.AvailableSkillPoints();

        for (int i = 0; i < SaveDataManager.SkillDefinitions.Length; i++)
        {
            SaveDataManager.SkillDefinition skill = SaveDataManager.SkillDefinitions[i];
            ConfigEntry<int> entry = SaveDataManager.SkillEntries[skill.ConfigKey];
            int index = i;

            _skillSliders[i] = MenuAPI.CreateREPOSlider(
                skill.ReadableName.ToTitleCase(),
                string.Empty,
                value => OnSkillPointAssignmentChanged(index, value),
                _skillsPage.transform,
                default, 0,
                Math.Clamp(entry.Value + availableSkillPoints, 0, skill.MaxValue),
                entry.Value);
        }

        foreach (REPOSlider slider in _skillSliders)
        {
            _skillsPage.AddElementToScrollView(slider.rectTransform);
        }

        _skillSliders[0].repoScrollViewElement.topPadding = 5;

        _skillsPage.scrollView.spacing = 5;

        REPOButton resetSkillPointsButton = MenuAPI.CreateREPOButton("Reset spent skill points", () =>
            {
                SaveDataManager.ResetSpentSkillPoints();
                _statisticsPage.ClosePage(true);
            }, _skillsPage.transform,
            new Vector2(460, 20));

        _skillsPage.AddElement(resetSkillPointsButton.rectTransform,
            resetSkillPointsButton.rectTransform.localPosition);

        _skillsPage.OpenPage(true);
    }

    private static void OnSkillPointAssignmentChanged(int index, int value)
    {
        SaveDataManager.SkillEntries[SaveDataManager.SkillDefinitions[index].ConfigKey].Value = value;

        int availableSkillPoints = SaveDataManager.AvailableSkillPoints();
        _availableSkillPoints.labelTMP.text = availableSkillPoints.ToString();

        for (int i = 0; i < _skillSliders.Length; i++)
        {
            REPOSlider slider = _skillSliders[i];

            slider.max = Mathf.Min((i == index ? value : slider.value) + availableSkillPoints,
                SaveDataManager.SkillDefinitions[i].MaxValue);

            slider.SetValue(slider.value, false);
        }
    }
}