using System;
using BepInEx.Configuration;
using MenuLib;
using MenuLib.MonoBehaviors;
using UnityEngine;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace RepoLeveling;

public static class SkillsMenu
{
    private static REPOLabel _availableSkillPoints;
    private static REPOSlider[] _skillSliders;

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
        CreateStatisticsPage();
        CreateSkillsPage();
    }

    private static void CreateStatisticsPage()
    {
        var statisticsPage = MenuAPI.CreateREPOPopupPage("Statistics", REPOPopupPage.PresetSide.Left, false, true);
        REPOElement[] elements =
        {
            MenuAPI.CreateREPOLabel("Total Hauled:", statisticsPage.transform, new Vector2(70, 270)),
            MenuAPI.CreateREPOLabel("Total SP earned:", statisticsPage.transform, new Vector2(70, 240)),
            MenuAPI.CreateREPOLabel("Available SP:", statisticsPage.transform, new Vector2(70, 210)),
            MenuAPI.CreateREPOLabel("Next SP in:", statisticsPage.transform, new Vector2(70, 180)),

            MenuAPI.CreateREPOLabel($"{SaveDataManager.SaveCumulativeHaul.Value}k", statisticsPage.transform,
                new Vector2(260, 270)),
            MenuAPI.CreateREPOLabel($"{SaveDataManager.SkillPointsFromCumulativeHaul()}", statisticsPage.transform,
                new Vector2(260, 240)),
            _availableSkillPoints = MenuAPI.CreateREPOLabel($"{SaveDataManager.AvailableSkillPoints()}",
                statisticsPage.transform, new Vector2(260, 210)),
            MenuAPI.CreateREPOLabel($"{SaveDataManager.NeededCumulativeHaulForNextSkillPoint()}k", statisticsPage.transform,
                new Vector2(260, 180)),

            MenuAPI.CreateREPOLabel("Note: SP = Skill Points", statisticsPage.transform, new Vector2(70, 60)),
            MenuAPI.CreateREPOButton("Reset progress", () => MenuAPI.OpenPopup("Reset Progress", Color.red,
                "Are you sure that you want to reset your progress? This cannot be undone!",
                () =>
                {
                    SaveDataManager.ResetProgress();
                    statisticsPage.ClosePage(true);
                }), statisticsPage.transform, new Vector2(190, 30)),
            MenuAPI.CreateREPOButton("Close", () => statisticsPage.ClosePage(true), statisticsPage.transform,
                new Vector2(60, 30))
        };

        foreach (REPOElement element in elements)
        {
            statisticsPage.AddElement(element.rectTransform, element.rectTransform.localPosition);
        }

        statisticsPage.OpenPage(false);
    }

    private static void CreateSkillsPage()
    {
        int availableSkillPoints = SaveDataManager.AvailableSkillPoints();

        var skillsPage = MenuAPI.CreateREPOPopupPage("Skills", REPOPopupPage.PresetSide.Right, false, false);

        _skillSliders = new[]
        {
            MenuAPI.CreateREPOSlider("Map Player Count",
                string.Empty,
                value => OnSkillPointAssignmentChanged(0, SaveDataManager.SaveMapPlayerCount, value),
                skillsPage.transform,
                new Vector2(395, 270), 0,
                Math.Min(SaveDataManager.SaveMapPlayerCount.Value + availableSkillPoints, 1),
                SaveDataManager.SaveMapPlayerCount.Value),
            MenuAPI.CreateREPOSlider("Stamina",
                string.Empty,
                value => OnSkillPointAssignmentChanged(1, SaveDataManager.SaveEnergy, value), skillsPage.transform,
                new Vector2(395, 240), 0,
                Math.Min(SaveDataManager.SaveEnergy.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveEnergy.Value),
            MenuAPI.CreateREPOSlider("Double Jump",
                string.Empty,
                value => OnSkillPointAssignmentChanged(2, SaveDataManager.SaveExtraJump, value), skillsPage.transform,
                new Vector2(395, 210), 0,
                Math.Min(SaveDataManager.SaveExtraJump.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveExtraJump.Value),
            MenuAPI.CreateREPOSlider("Range",
                string.Empty,
                value => OnSkillPointAssignmentChanged(3, SaveDataManager.SaveGrabRange, value), skillsPage.transform,
                new Vector2(395, 180), 0,
                Math.Min(SaveDataManager.SaveGrabRange.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveGrabRange.Value),
            MenuAPI.CreateREPOSlider("Strength",
                string.Empty,
                value => OnSkillPointAssignmentChanged(4, SaveDataManager.SaveGrabStrength, value),
                skillsPage.transform,
                new Vector2(395, 150), 0,
                Math.Min(SaveDataManager.SaveGrabStrength.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveGrabStrength.Value),
            MenuAPI.CreateREPOSlider("Throw",
                string.Empty,
                value => OnSkillPointAssignmentChanged(5, SaveDataManager.SaveGrabThrow, value), skillsPage.transform,
                new Vector2(395, 120), 0,
                Math.Min(SaveDataManager.SaveGrabThrow.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveGrabThrow.Value),
            MenuAPI.CreateREPOSlider("Health",
                string.Empty,
                value => OnSkillPointAssignmentChanged(6, SaveDataManager.SaveHealth, value), skillsPage.transform,
                new Vector2(395, 90), 0,
                Math.Min(SaveDataManager.SaveHealth.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveHealth.Value),
            MenuAPI.CreateREPOSlider("Sprint Speed",
                string.Empty,
                value => OnSkillPointAssignmentChanged(7, SaveDataManager.SaveSprintSpeed, value), skillsPage.transform,
                new Vector2(395, 60), 0,
                Math.Min(SaveDataManager.SaveSprintSpeed.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveSprintSpeed.Value),
            MenuAPI.CreateREPOSlider("Tumble Launch",
                string.Empty,
                value => OnSkillPointAssignmentChanged(8, SaveDataManager.SaveTumbleLaunch, value),
                skillsPage.transform,
                new Vector2(395, 30), 0,
                Math.Min(SaveDataManager.SaveTumbleLaunch.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveTumbleLaunch.Value)
        };

        foreach (REPOSlider slider in _skillSliders)
        {
            skillsPage.AddElement(slider.rectTransform, slider.rectTransform.localPosition);
        }

        skillsPage.OpenPage(true);
    }

    private static void OnSkillPointAssignmentChanged(int index, ConfigEntry<int> entry, int value)
    {
        entry.Value = value;

        int availableSkillPoints = SaveDataManager.AvailableSkillPoints();

        _availableSkillPoints.labelTMP.text = availableSkillPoints.ToString();

        foreach (REPOSlider slider in _skillSliders)
        {
            if (slider.labelTMP.text == "Map Player Count")
            {
                slider.max = Math.Min(slider.value + availableSkillPoints, 1);
            }
            else if (slider == _skillSliders[index])
            {
                slider.max = Mathf.Min(value + availableSkillPoints, int.MaxValue);
            }
            else
            {
                slider.max = Mathf.Min(slider.value + availableSkillPoints, int.MaxValue);
            }

            slider.SetValue(slider.value, false);
        }
    }
}