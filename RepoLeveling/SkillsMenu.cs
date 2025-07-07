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
            MenuAPI.CreateREPOLabel("Next SP in:", statisticsPage.transform, new Vector2(70, 240)),
            MenuAPI.CreateREPOLabel("Total SP earned:", statisticsPage.transform, new Vector2(70, 210)),
            MenuAPI.CreateREPOLabel("Available SP:", statisticsPage.transform, new Vector2(70, 180)),
            MenuAPI.CreateREPOLabel("Current Haul:", statisticsPage.transform, new Vector2(70, 150)),

            MenuAPI.CreateREPOLabel($"{SaveDataManager.SaveCumulativeHaul.Value}k", statisticsPage.transform,
                new Vector2(260, 270)),
            MenuAPI.CreateREPOLabel($"{SaveDataManager.NeededCumulativeHaulForNextSkillPoint()}k",
                statisticsPage.transform,
                new Vector2(260, 240)),
            MenuAPI.CreateREPOLabel($"{SaveDataManager.SkillPointsFromCumulativeHaul()}", statisticsPage.transform,
                new Vector2(260, 210)),
            _availableSkillPoints = MenuAPI.CreateREPOLabel($"{SaveDataManager.AvailableSkillPoints()}",
                statisticsPage.transform, new Vector2(260, 180)),
            MenuAPI.CreateREPOLabel($"{StatsManager.instance.GetRunStatTotalHaul()}k", statisticsPage.transform,
                new Vector2(260, 150)),

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
                new Vector2(395, 280), 0,
                Math.Min(SaveDataManager.SaveMapPlayerCount.Value + availableSkillPoints, 1),
                SaveDataManager.SaveMapPlayerCount.Value),
            MenuAPI.CreateREPOSlider("Crouch Rest",
                string.Empty,
                value => OnSkillPointAssignmentChanged(1, SaveDataManager.SaveCrouchRest, value), skillsPage.transform,
                new Vector2(395, 255), 0,
                Math.Min(SaveDataManager.SaveCrouchRest.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveCrouchRest.Value),
            MenuAPI.CreateREPOSlider("Stamina",
                string.Empty,
                value => OnSkillPointAssignmentChanged(2, SaveDataManager.SaveEnergy, value), skillsPage.transform,
                new Vector2(395, 230), 0,
                Math.Min(SaveDataManager.SaveEnergy.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveEnergy.Value),
            MenuAPI.CreateREPOSlider("Double Jump",
                string.Empty,
                value => OnSkillPointAssignmentChanged(3, SaveDataManager.SaveExtraJump, value), skillsPage.transform,
                new Vector2(395, 205), 0,
                Math.Min(SaveDataManager.SaveExtraJump.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveExtraJump.Value),
            MenuAPI.CreateREPOSlider("Range",
                string.Empty,
                value => OnSkillPointAssignmentChanged(4, SaveDataManager.SaveGrabRange, value), skillsPage.transform,
                new Vector2(395, 180), 0,
                Math.Min(SaveDataManager.SaveGrabRange.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveGrabRange.Value),
            MenuAPI.CreateREPOSlider("Strength",
                string.Empty,
                value => OnSkillPointAssignmentChanged(5, SaveDataManager.SaveGrabStrength, value),
                skillsPage.transform,
                new Vector2(395, 155), 0,
                Math.Min(SaveDataManager.SaveGrabStrength.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveGrabStrength.Value),
            MenuAPI.CreateREPOSlider("Throw",
                string.Empty,
                value => OnSkillPointAssignmentChanged(6, SaveDataManager.SaveGrabThrow, value), skillsPage.transform,
                new Vector2(395, 130), 0,
                Math.Min(SaveDataManager.SaveGrabThrow.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveGrabThrow.Value),
            MenuAPI.CreateREPOSlider("Health",
                string.Empty,
                value => OnSkillPointAssignmentChanged(7, SaveDataManager.SaveHealth, value), skillsPage.transform,
                new Vector2(395, 105), 0,
                Math.Min(SaveDataManager.SaveHealth.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveHealth.Value),
            MenuAPI.CreateREPOSlider("Sprint Speed",
                string.Empty,
                value => OnSkillPointAssignmentChanged(8, SaveDataManager.SaveSprintSpeed, value), skillsPage.transform,
                new Vector2(395, 80), 0,
                Math.Min(SaveDataManager.SaveSprintSpeed.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveSprintSpeed.Value),
            MenuAPI.CreateREPOSlider("Tumble Launch",
                string.Empty,
                value => OnSkillPointAssignmentChanged(9, SaveDataManager.SaveTumbleLaunch, value),
                skillsPage.transform,
                new Vector2(395, 55), 0,
                Math.Min(SaveDataManager.SaveTumbleLaunch.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveTumbleLaunch.Value),
            MenuAPI.CreateREPOSlider("Tumble Wings",
                string.Empty,
                value => OnSkillPointAssignmentChanged(10, SaveDataManager.SaveTumbleWings, value), skillsPage.transform,
                new Vector2(395, 30), 0,
                Math.Min(SaveDataManager.SaveTumbleWings.Value + availableSkillPoints, int.MaxValue),
                SaveDataManager.SaveTumbleWings.Value)
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
            int limit = slider.labelTMP.text == "Map Player Count" ? 1 : int.MaxValue;

            if (slider == _skillSliders[index])
            {
                slider.max = Mathf.Min(value + availableSkillPoints, limit);
            }
            else
            {
                slider.max = Mathf.Min(slider.value + availableSkillPoints, limit);
            }

            slider.SetValue(slider.value, false);
        }
    }
}