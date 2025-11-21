using System;
using BepInEx.Configuration;
using MenuLib;
using MenuLib.MonoBehaviors;
using UnityEngine;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace RepoLeveling;

public static class SkillsMenu
{
    private static REPOPopupPage _statisticsPage;
    private static REPOPopupPage _skillsPage;
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
        {
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
        };

        foreach (REPOElement element in elements)
        {
            _statisticsPage.AddElement(element.rectTransform, element.rectTransform.localPosition);
        }

        _statisticsPage.OpenPage(false);
    }

    private static void CreateSkillsPage()
    {
        int availableSkillPoints = SaveDataManager.AvailableSkillPoints();

        _skillsPage = MenuAPI.CreateREPOPopupPage("Skills", REPOPopupPage.PresetSide.Right, false, false);

        _skillSliders = new[]
        {
            MenuAPI.CreateREPOSlider("Death Head Battery",
                string.Empty,
                value => OnSkillPointAssignmentChanged(0, SaveDataManager.SaveDeathHeadBattery, value),
                _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveDeathHeadBattery.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveDeathHeadBattery.Value),
            MenuAPI.CreateREPOSlider("Map Player Count",
                string.Empty,
                value => OnSkillPointAssignmentChanged(1, SaveDataManager.SaveMapPlayerCount, value),
                _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveMapPlayerCount.Value + availableSkillPoints, 0, 1),
                SaveDataManager.SaveMapPlayerCount.Value),
            MenuAPI.CreateREPOSlider("Crouch Rest",
                string.Empty,
                value => OnSkillPointAssignmentChanged(2, SaveDataManager.SaveCrouchRest, value), _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveCrouchRest.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveCrouchRest.Value),
            MenuAPI.CreateREPOSlider("Stamina",
                string.Empty,
                value => OnSkillPointAssignmentChanged(3, SaveDataManager.SaveEnergy, value), _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveEnergy.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveEnergy.Value),
            MenuAPI.CreateREPOSlider("Double Jump",
                string.Empty,
                value => OnSkillPointAssignmentChanged(4, SaveDataManager.SaveExtraJump, value), _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveExtraJump.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveExtraJump.Value),
            MenuAPI.CreateREPOSlider("Range",
                string.Empty,
                value => OnSkillPointAssignmentChanged(5, SaveDataManager.SaveGrabRange, value), _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveGrabRange.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveGrabRange.Value),
            MenuAPI.CreateREPOSlider("Strength",
                string.Empty,
                value => OnSkillPointAssignmentChanged(6, SaveDataManager.SaveGrabStrength, value),
                _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveGrabStrength.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveGrabStrength.Value),
            MenuAPI.CreateREPOSlider("Throw",
                string.Empty,
                value => OnSkillPointAssignmentChanged(7, SaveDataManager.SaveGrabThrow, value), _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveGrabThrow.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveGrabThrow.Value),
            MenuAPI.CreateREPOSlider("Health",
                string.Empty,
                value => OnSkillPointAssignmentChanged(8, SaveDataManager.SaveHealth, value), _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveHealth.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveHealth.Value),
            MenuAPI.CreateREPOSlider("Sprint Speed",
                string.Empty,
                value => OnSkillPointAssignmentChanged(9, SaveDataManager.SaveSprintSpeed, value), _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveSprintSpeed.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveSprintSpeed.Value),
            MenuAPI.CreateREPOSlider("Tumble Climb",
                string.Empty,
                value => OnSkillPointAssignmentChanged(10, SaveDataManager.SaveTumbleClimb, value),
                _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveTumbleClimb.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveTumbleClimb.Value),
            MenuAPI.CreateREPOSlider("Tumble Launch",
                string.Empty,
                value => OnSkillPointAssignmentChanged(11, SaveDataManager.SaveTumbleLaunch, value),
                _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveTumbleLaunch.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveTumbleLaunch.Value),
            MenuAPI.CreateREPOSlider("Tumble Wings",
                string.Empty,
                value => OnSkillPointAssignmentChanged(12, SaveDataManager.SaveTumbleWings, value),
                _skillsPage.transform,
                default, 0,
                Math.Clamp(SaveDataManager.SaveTumbleWings.Value + availableSkillPoints, 0, int.MaxValue),
                SaveDataManager.SaveTumbleWings.Value)
        };

        foreach (REPOSlider slider in _skillSliders)
        {
            _skillsPage.AddElementToScrollView(slider.rectTransform);
        }

        _skillSliders[0].repoScrollViewElement.topPadding = 5;

        _skillsPage.scrollView.spacing = 5;

        REPOButton resetSkillPointsButton = MenuAPI.CreateREPOButton("Reset spent skill points", () =>
            {
                SaveDataManager.ResetSkillPoints();
                _statisticsPage.ClosePage(true);
            }, _skillsPage.transform,
            new Vector2(460, 20));
        
        _skillsPage.AddElement(resetSkillPointsButton.rectTransform, resetSkillPointsButton.rectTransform.localPosition);

        _skillsPage.OpenPage(true);
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