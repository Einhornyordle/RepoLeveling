using MenuLib;
using MenuLib.MonoBehaviors;
using UnityEngine;

namespace RepoLeveling;

internal static class MenuPageMain
{
    internal static void Initialize()
    {
        MenuAPI.AddElementToMainMenu(parent =>
            MenuAPI.CreateREPOButton("Repo Leveling", Open, parent, new Vector2(50, 360)));
        MenuAPI.AddElementToEscapeMenu(parent =>
            MenuAPI.CreateREPOButton("Repo Leveling", Open, parent, new Vector2(50, 360)));
        MenuAPI.AddElementToLobbyMenu(parent =>
            MenuAPI.CreateREPOButton("Repo Leveling", Open, parent, new Vector2(50, 360)));
    }

    internal static void Open()
    {
        ManagerConfig.Reload();
        REPOPopupPage page = MenuAPI.CreateREPOPopupPage("Repo Leveling", REPOPopupPage.PresetSide.Left, false, true);

        REPOElement[] elements =
        [
            MenuAPI.CreateREPOButton("Skills", MenuPageSkills.Open, page.transform),
            MenuAPI.CreateREPOButton("Statistics", MenuPageStatistics.Open, page.transform),
            MenuAPI.CreateREPOButton("Settings", MenuPageSettings.Open, page.transform)
        ];

        foreach (REPOElement element in elements)
        {
            page.AddElementToScrollView(element.rectTransform);
        }

        REPOButton close = MenuAPI.CreateREPOButton("Close", () =>
            {
                page.ClosePage(true);
                PatchPunManager.SyncSkills();
            }, page.transform,
            new Vector2(66, 20));

        page.AddElement(close.rectTransform,
            close.rectTransform.localPosition);

        page.onEscapePressed += () =>
        {
            PatchPunManager.SyncSkills();
            return true;
        };

        page.OpenPage(false);
    }
}