using Game;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LevelIconController : MonoBehaviour
{
    public LevelIconView View;
    private Color BackgroundActiveLevelColor = Color.white;
    private UserAccount.Data.LevelState LevelState;

    public void Init(int chapter, int levelIndex)
    {
        View.SetMainText((levelIndex + 1).ToString(), false);
        View.ShowSpiral(true);
        View.ShowLastNotComleted(false);
        View.SetStarsVisible(false);
        View.ShowMedal0(false);
        View.Animator.enabled = false;
        GetComponent<Button>().interactable = false;

        LevelState = new UserAccount.Data.LevelState(chapter, levelIndex % LevelManager.LevelsPerChapter);
    }

    public void Init(UserAccount.Data.LevelState levState)
    {
        LevelState = levState;
        View.SetMainText((levState.LevelId + 1).ToString(), true);
        View.ShowSpiral(true);
        View.ShowLastNotComleted(false);
        View.SetStarsVisible(true);
        View.ShowMedal0(levState.Medal0);
        View.StarsBlockView.Set(LevelState.Stars, LevelState.Stars);
        GetComponent<Button>().interactable = levState.IsOpened;
    }

    public void SyncToState(UserAccount.Data.LevelState levState)
    {
        // ChapterId
        Assert.IsTrue(LevelState.ChapterId == levState.ChapterId, "can't change chapter");

        // LevelId
        Assert.IsTrue(LevelState.LevelId == levState.LevelId, $"can't change level id: {LevelState.LevelId} -> {levState.LevelId}; chapter {LevelState.ChapterId}");

        // IsCompleted
        if (LevelState.IsCompleted != levState.IsCompleted)
        {
            Assert.IsTrue(LevelState.IsCompleted == false);
            LevelState.IsCompleted = levState.IsCompleted;
            View.SetStarsVisible(true);
        }

        // IsOpened
        if (LevelState.IsOpened != levState.IsOpened)
        {
            LevelState.IsOpened = levState.IsOpened;
            GetComponent<Button>().interactable = levState.IsOpened;
            if (LevelState.IsOpened)
            {
                var lastOpenedIndex = UserAccounts.Instance.GetCurrentAccount().AccountData.LastLevelIndex;
                if (LevelState.LevelId == lastOpenedIndex && !LevelState.IsCompleted)
                    View.ShowLastNotComleted(true);
            }

            View.SetMainTextVisible(true);
        }

        // Stars
        View.StarsBlockView.Set(levState.Stars, levState.Stars);

        // Medal0
        View.ShowMedal0(levState.Medal0);

        // DieCounter;
        // PlayCounter;






    }
}
