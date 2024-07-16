using Events;
using Game;
using GameGUI;
using TMPro;
using UnityEngine;

public class PopupGameOver 
    : GUIScreenBase
    , IHandle<ControlPanelController.EventBackPress>
    , SimpleGUI.IInitialize
{
    public enum Mode
    {
        GameOver,
        Abandon
    }

    public GameObject CaptionGameOver;
    public GameObject CaptionAbandon;
    public LevelIconController LevelIcon;
    public SurvivalIcon SurvivalIcon;
    public GameObject CoinLoseSection;
    public TextMeshProUGUI CoinNumberText;
    private Mode _currentMode;

    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public override void StartAppearAnimation()
    {
        base.StartAppearAnimation();

        var acc = UserAccounts.Instance.GetCurrentAccount();
        
        var levelID = StateGameplay.Instance.Session.LevelID;
        if (levelID != -1) // dev start support
            LevelIcon.Init(acc.AccountData.LevelStates[levelID]);
        
        SetTextCoinLoose(StateGameplay.Instance.EntryCoins);
    }

    private void SetTextCoinLoose(int entryCoins)
    {
        if (entryCoins == 0)
        {
            CoinLoseSection.SetActive(false);
            return;
        }

        CoinNumberText.text = $"- {entryCoins}";
    }

    public void SetMode(Mode mode, StateGameplay.GameMode gameMode)
    {
        _currentMode = mode;
        CaptionGameOver.SetActive( mode == Mode.GameOver);
        CaptionAbandon.SetActive( mode == Mode.Abandon);

        if (gameMode == StateGameplay.GameMode.Regular)
        {
            LevelIcon.gameObject.SetActive(true);
            SurvivalIcon.gameObject.SetActive(false);
        }
        else // one of the survival modes
        {
            LevelIcon.gameObject.SetActive(false);
            SurvivalIcon.SetMode(gameMode);
            SurvivalIcon.SetLevelNumber(StateGameplay.Instance.LevelID);
            SurvivalIcon.gameObject.SetActive(true);
        }
    }

    public void Handle(ControlPanelController.EventBackPress message)
    {
        if(!IsInputEnabled)
            return;
        StateGameplay.Instance.SetPause(false);
        SimpleGui.PopScreen("Popup.GameOver");
    }


    // GameOver -> Home transition
    public void OnHomePress() 
    {
        SaveSessionStats();
        StateGameplay.Instance.ReturnToHomeState();
    }

    public void OnRestartPress()
    {
        SaveSessionStats();
        StateGameplay.Instance.RestartState();
    }

    private void SaveSessionStats()
    {
        if (StateGameplay.Instance.CurrentGameMode == StateGameplay.GameMode.Regular)
        {
            var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
            var sessionData = StateGameplay.Instance.Session;
            accountData.LevelStates[sessionData.LevelID].DieCounter += sessionData.DiesCounter;
        }
        else // survival
        {
            var accountData = UserAccounts.Instance.GetCurrentAccount().AccountData;
            var survLevState = accountData.GetSurvivalLevelState(StateGameplay.Instance.CurrentGameMode);
            survLevState.PlayCounter++;
        }

        UserAccounts.Instance.GetCurrentAccount().Save();
    }
}
