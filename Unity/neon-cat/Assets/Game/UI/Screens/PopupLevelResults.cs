using System;
using DG.Tweening;
using Events;
using Game;
using GameGUI;
using GameLib.Log;
using IngameDebugConsole;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLevelResults
    : GUIScreenBase
    , SimpleGUI.IInitialize
    , IHandle<ControlPanelController.EventHomePress>
{
    public ControlPanelController ControlPanel;
    public GameObject ButtonRestart;
    public GameObject ButtonGo;
    public GameObject LevelModeUnlocked;
    public TextMeshProUGUI TextCaption;
    public TextMeshProUGUI TextCaptionWin;
    public LevelIconController LevelIcon;
    public SurvivalIcon SurvivalIcon;
    public LevelManager LevManager;
    public RectTransform CenterPivot;
    public TextMeshProUGUI TextCoinsCollected;
    public IconIndicatorView CoinsTotal;
    public StarsBlockView StarsBlock;
    public Image NextLevelSpiral;
    public TextMeshProUGUI TextNextLevelNumber;

    public LogChecker Log;

    private bool IsShowNextLevelNumber = false;
    private StateGameplay.GameMode _gameMode;

    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public void SetMode(StateGameplay.GameMode mode)
    {
        _gameMode = mode;
        if (Log.Verbose())
            Debug.Log($"PopupLevelResult.SetMode {mode}");
        if (mode == StateGameplay.GameMode.Regular)
        {
            LevelIcon.gameObject.SetActive(true);
            SurvivalIcon.gameObject.SetActive(false);
            ButtonRestart.SetActive(true);
            var r = ButtonGo.GetComponent<RectTransform>();
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
        }
        else // one of survival
        {
            LevelIcon.gameObject.SetActive(false);
            var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
            SurvivalIcon.SetLevelNumber(accountData.GetSurvivalLevelState(mode).LastLevelIndex);
            SurvivalIcon.gameObject.SetActive(true);
            ButtonRestart.SetActive(false);
            ButtonGo.GetComponent<RectTransform>().position = CenterPivot.position;
        }
    }


    public override void StartAppearAnimation()
    {
        base.StartAppearAnimation();

        SetMode(StateGameplay.Instance.CurrentGameMode);
        StateGameplay.Instance.DestroyLevel();
        StateGameplay.Instance.SetPause(false, true);

        ControlPanel.SetupPanel("ButtonHome");
        SetupWindow();

        if (StateGameplay.Instance.CurrentGameMode == StateGameplay.GameMode.Regular)
            ApplyResultsRegular();
        else
            ApplyResultsSurvival();
    }

    private void ApplyResultsRegular()
    {
        if (Log.Verbose())
            Debug.Log($"> PopupLevelResult.ApplyResultsRegular");
        var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
        var sessionData = StateGameplay.Instance.Session;
        var completeFactor = sessionData.CoinsCollected / (float)sessionData.CoinsLocated;
        var oldStars = accountData.LevelStates[sessionData.LevelID].Stars;
        var newStars = ResultHelpers.GetStarsCount(completeFactor);
        var medal = sessionData.HostagesRemained == 0;
        var oldMedal = accountData.LevelStates[sessionData.LevelID].Medal0;

        accountData.Coins += sessionData.GetCoins();
        accountData.LevelStates[sessionData.LevelID].IsCompleted = true;
        accountData.LevelStates[sessionData.LevelID].DieCounter += sessionData.DiesCounter;
        accountData.LevelStates[sessionData.LevelID].WinCounter += 1;
        accountData.LevelStates[sessionData.LevelID].Stars = Mathf.Max(oldStars, newStars);
        accountData.LevelStates[sessionData.LevelID].Medal0 = medal || oldMedal;
        accountData.CheatedCounter += sessionData.HasCheats ? 1 : 0;
        if (sessionData.LevelID == accountData.LastLevelIndex)
        {
            if (accountData.LastLevelIndex == StateHome.Instance.LevelManager.GetMaxLevelIndex())
            {
                // last level competed
            }
            else
            {
                // unlock next level
                accountData.LastLevelIndex++;
                accountData.LevelStates[accountData.LastLevelIndex].IsOpened = true;
            }
        }

        // survival mode unlocked
        if (GetSurvivalUnlocked(out var modeIndex))
            accountData.SurvivalLevelStates[modeIndex].LastLevelIndex = 0;
        UserAccounts.Instance.GetCurrentAccount().Save();
        if (Log.Verbose())
            Debug.Log($"Session state saved: {JsonUtility.ToJson(StateGameplay.Instance.Session)}");
    }

    private void ApplyResultsSurvival()
    {
        if (Log.Verbose())
            Debug.Log($"> PopupLevelResult.ApplyResultsSurvival");
        var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
        var sessionData = StateGameplay.Instance.Session;
        var survivalLevelState = accountData.GetSurvivalLevelState(StateGameplay.Instance.CurrentGameMode);

        accountData.Coins += sessionData.GetCoins();
        accountData.CheatedCounter += sessionData.HasCheats ? 1 : 0;
        survivalLevelState.LastLevelIndex++;
        survivalLevelState.PlayCounter++;

        UserAccounts.Instance.GetCurrentAccount().Save();
        if (Log.Verbose())
            Debug.Log($"Session state saved: {JsonUtility.ToJson(StateGameplay.Instance.Session)}");
    }

    // note: some values are not restored such as complete level status, unlock of survival mode 
    private void RevertResults()
    {
        var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
        var sessionData = StateGameplay.Instance.Session;

        // restore total coins to original value
        accountData.Coins -= sessionData.GetCoins();

        UserAccounts.Instance.GetCurrentAccount().Save();
    }

    private void SetupWindow()
    {
        var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
        var sessionData = StateGameplay.Instance.Session;

        var coinsTotalLocal = StateGameplay.Instance.Session.GetCoins();
        var coinsTotalGlobal = UserAccounts.Instance.GetCurrentAccount().AccountData.Coins;
        var levelID = StateGameplay.Instance.Session.LevelID;
        var isDevMode = levelID == -1;
        var completeFactor = sessionData.CoinsCollected / (float)sessionData.CoinsLocated;
        var oldStars = isDevMode ? 0 : UserAccounts.Instance.GetCurrentAccount().AccountData.LevelStates[levelID].Stars;
        var newStars = ResultHelpers.GetStarsCount(completeFactor);
        var isFinalLevelCompleted = (sessionData.LevelID == StateHome.Instance.LevelManager.GetMaxLevelIndex());
        var medalPrev = isDevMode ? false : accountData.LevelStates[levelID].Medal0;
        var medal = sessionData.HostagesRemained == 0;


        if (Log.Verbose())
        {
            Debug.Log("PopupLevelResult.SetupWindow >>>>>>>>>>>>>>>");
            Debug.Log($"session state: {JsonUtility.ToJson(StateGameplay.Instance.Session)}");
            if (isDevMode)
                Debug.Log("Dev mode performed");
            else
                Debug.Log($"level state: {JsonUtility.ToJson(UserAccounts.Instance.GetCurrentAccount().AccountData.LevelStates[levelID])}");
            Debug.Log($"coins total local {coinsTotalLocal}");
            Debug.Log($"coins total global {coinsTotalGlobal}");
            Debug.Log($"complete factor {completeFactor}");
            Debug.Log($"stars gained {newStars}, prev was {oldStars}");
            Debug.Log($"medal gained {medal}, prev was {medalPrev}");
        }

        if (isFinalLevelCompleted)
        {
            ButtonGo.SetActive(false);
            // todo: show home button and center
        }

        // coins collected
        TextCoinsCollected.text = $"{coinsTotalLocal}";
        TextCoinsCollected.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-100, 0), 1.0f)
            .From()
            .SetUpdate(true)
            .SetEase(Ease.OutQuint);

        // text coins total
        CoinsTotal.InitValue(coinsTotalGlobal);
        if (coinsTotalLocal != 0)
            CoinsTotal.ChangeValue(coinsTotalLocal);

        // level icon
        if(!isDevMode)
            LevelIcon.Init(UserAccounts.Instance.GetCurrentAccount().AccountData.LevelStates[levelID]);
        LevelIcon.View.ShowMedal0(medalPrev || medal);
        StarsBlock.Set(newStars, oldStars);
        LevelIcon.View.PlayAnimation();

        // level mode unlock
        LevelModeUnlocked.SetActive(GetSurvivalUnlocked(out _));

        // next level
        if (IsShowNextLevelNumber)
        {
            TextNextLevelNumber.text = isFinalLevelCompleted ? "" : (accountData.LastLevelIndex + 1 + 1).ToString();
            NextLevelSpiral.gameObject.SetActive(!isFinalLevelCompleted);
        }
    }

    public void Handle(ControlPanelController.EventHomePress message)
    {
        if (!IsInputEnabled)
            return;
        StateGameplay.Instance.SetPause(false, true);
        StateGameplay.Instance.ReturnToHomeState();
    }

    public void OnNextPress()
    {
        var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;

        var nextLevelIndex = _gameMode == StateGameplay.GameMode.Regular
            ? accountData.LastLevelIndex
            : accountData.GetSurvivalLevelState(StateGameplay.Instance.CurrentGameMode).LastLevelIndex;

        SimpleGui.PushScreen<PopupExpedition>("Popup.Expedition", expeditionPopup =>
        {
            expeditionPopup.LevelID = nextLevelIndex;
        });
    }

    public void OnRestartPress()
    {
        StateGameplay.Instance.SetPause(false, true);
        RevertResults();
        StateGameplay.Instance.RestartState();
    }

    private bool GetSurvivalUnlocked(out int modeIndex)
    {
        var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
        var sessionData = StateGameplay.Instance.Session;
        modeIndex = -1;

        if (StateGameplay.Instance.CurrentGameMode == StateGameplay.GameMode.Regular)
        {
            modeIndex = Array.IndexOf(LevManager.SurvivalUnlockIndexes, sessionData.LevelID);
            return modeIndex > -1 && accountData.SurvivalLevelStates[modeIndex].LastLevelIndex == -1;
        }
        return false;
    }

    //[ConsoleMethod("levels.unlock", "Unlocks all levels")]
    public static void DbgUnlockAllLevels()
    {
        var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
        accountData.LastLevelIndex = StateHome.Instance.LevelManager.GetMaxLevelIndex();
        foreach (var levState in accountData.LevelStates)
            levState.IsOpened = true;

        foreach (var survivalLevelState in accountData.SurvivalLevelStates)
            if (survivalLevelState.LastLevelIndex < 0)
                survivalLevelState.LastLevelIndex = 0;

        accountData.CheatedCounter++;

        UserAccounts.Instance.GetCurrentAccount().Save();

        Debug.Log("All levels unlocked.");
    }
}

public static class ResultHelpers
{
    public static int GetStarsCount(float result)
    {
        var stars = 0;
        if (result > 0.3f)
            stars = 1;
        if (result > 0.6)
            stars = 2;
        if (result >= 1f)
            stars = 3;
        return stars;
    }
}
