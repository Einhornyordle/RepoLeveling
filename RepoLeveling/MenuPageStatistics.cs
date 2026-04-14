using MenuLib;
using MenuLib.MonoBehaviors;

namespace RepoLeveling;

public static class MenuPageStatistics
{
    public static void Open()
    {
        if (MenuManager.instance.addedPagesOnTop.Count > 0)
        {
            if (MenuManager.instance.addedPagesOnTop[0].name == "Menu Page Statistics")
                return;
            MenuAPI.CloseAllPagesAddedOnTop();
        }

        REPOPopupPage page = MenuAPI.CreateREPOPopupPage("Statistics", REPOPopupPage.PresetSide.Right, false, false);

        string[] statistics =
        [
            $"Total Hauled: {ManagerSaveData.SaveCumulativeHaul.Value}k",
            $"Current Haul: {StatsManager.instance.GetRunStatTotalHaul()}k",
            $"Next SP in: {ManagerSaveData.NeededCumulativeHaulForNextSkillPoint()}k",
            $"Total SP earned: {ManagerSaveData.SkillPointsFromCumulativeHaul()}",
            $"Available SP: {ManagerSaveData.AvailableSkillPoints()}"
        ];

        foreach (string stat in statistics)
        {
            page.AddElementToScrollView(MenuAPI.CreateREPOLabel(stat, page.transform).rectTransform);
        }

        page.OpenPage(true);
    }
}