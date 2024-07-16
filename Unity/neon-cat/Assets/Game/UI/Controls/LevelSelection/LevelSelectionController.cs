using UnityEngine;

public class LevelSelectionController : MonoBehaviour
{
    public LevelSelectionView View;
    public ScreenLevelSelection Screen;

    public void SyncToData()
    {
        var account = UserAccounts.Instance.GetCurrentAccount();
        foreach (var levState in account.AccountData.LevelStates)
        {
            View.SyncLevelState(levState);
        }
    }

    public void OpenPage(int page)
    {
        View.SetPage(page);
    }

    public void OnLevelSelected(int chapter, int level)
    {
        Screen.OnLevelSelected(chapter,level);
    }
}
