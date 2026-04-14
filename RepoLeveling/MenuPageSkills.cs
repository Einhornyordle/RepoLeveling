using System;
using System.Collections.Generic;
using System.Linq;
using MenuLib;
using MenuLib.MonoBehaviors;
using Sirenix.Utilities;
using UnityEngine;

namespace RepoLeveling;

internal static class MenuPageSkills
{
    private static Dictionary<REPOSlider, ManagerSaveData.SkillDefinition> _skillSliders = null!;
    private static REPOLabel _skillPointsLabel = null!;

    private static string SkillPointsLabelText(int available, int total) => $"Skill points: {available} / {total}";

    internal static void Open()
    {
        if (MenuManager.instance.addedPagesOnTop.Count > 0)
        {
            if (MenuManager.instance.addedPagesOnTop[0].name == "Menu Page Skills")
                return;
            MenuAPI.CloseAllPagesAddedOnTop();
        }

        REPOPopupPage page = MenuAPI.CreateREPOPopupPage("Skills", REPOPopupPage.PresetSide.Right, false, false);

        _skillSliders = new Dictionary<REPOSlider, ManagerSaveData.SkillDefinition>();
        int availableSkillPoints = ManagerSaveData.AvailableSkillPoints();

        foreach (ManagerSaveData.SkillDefinition skill in ManagerSaveData.SkillDefinitions)
        {
            int currentValue = ManagerSaveData.SkillEntries[skill.ConfigKey].Value;

            REPOSlider slider = MenuAPI.CreateREPOSlider(
                skill.ReadableName.ToTitleCase(),
                string.Empty,
                value => OnSkillPointAssignmentChanged(skill, value),
                page.transform,
                default, 0,
                Math.Clamp(currentValue + availableSkillPoints, 0, skill.MaxValue),
                currentValue);

            _skillSliders[slider] = skill;
            page.AddElementToScrollView(slider.rectTransform);
        }

        if (_skillSliders.Count > 0)
            _skillSliders.Keys.First().repoScrollViewElement.topPadding = 5;
        page.scrollView.spacing = 5;

        _skillPointsLabel = MenuAPI.CreateREPOLabel(
            SkillPointsLabelText(availableSkillPoints, ManagerSaveData.SkillPointsFromCumulativeHaul()), page.transform,
            new Vector2(420, 20));

        page.AddElement(_skillPointsLabel.rectTransform, _skillPointsLabel.rectTransform.localPosition);

        page.OpenPage(true);
    }

    private static void OnSkillPointAssignmentChanged(ManagerSaveData.SkillDefinition changedSkill, int value)
    {
        ManagerSaveData.SkillEntries[changedSkill.ConfigKey].Value = value;

        int availableSkillPoints = ManagerSaveData.AvailableSkillPoints();

        foreach ((REPOSlider slider, ManagerSaveData.SkillDefinition skill) in _skillSliders)
        {
            slider.max = Math.Min((skill == changedSkill ? value : slider.value) + availableSkillPoints,
                skill.MaxValue);
            slider.SetValue(slider.value, false);
        }

        _skillPointsLabel.labelTMP.text =
            SkillPointsLabelText(availableSkillPoints, ManagerSaveData.SkillPointsFromCumulativeHaul());
    }
}