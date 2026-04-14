using MenuLib;
using MenuLib.MonoBehaviors;
using UnityEngine;

namespace RepoLeveling;

internal static class MenuPageSettings
{
    internal static void Open()
    {
        if (MenuManager.instance.addedPagesOnTop.Count > 0)
        {
            if (MenuManager.instance.addedPagesOnTop[0].name == "Menu Page Settings")
                return;
            MenuAPI.CloseAllPagesAddedOnTop();
        }

        REPOPopupPage page = MenuAPI.CreateREPOPopupPage("Settings", REPOPopupPage.PresetSide.Right, false, false);

        page.AddElementToScrollView(MenuAPI.CreateREPOButton("Reset spent skill points", () =>
        {
            ManagerSaveData.ResetSpentSkillPoints();
            MenuAPI.CloseAllPagesAddedOnTop();
            PatchPunManager.SyncSkills();
        }, page.transform).rectTransform);

        page.AddElementToScrollView(MenuAPI.CreateREPOButton("Reset progress", () => MenuAPI.OpenPopup("Reset Progress",
            Color.red,
            "Are you sure that you want to reset your progress? This cannot be undone!",
            () =>
            {
                ManagerSaveData.ResetProgress();
                page.ClosePage(true);
                PatchPunManager.SyncSkills();
            }), page.transform).rectTransform);

        page.OpenPage(true);
    }
}