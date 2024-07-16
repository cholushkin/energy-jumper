using System.Collections;
using Events;
using Game;
using GameGUI;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;

public class PopupExpedition 
    : GUIScreenBase
    , IHandle<ControlPanelController.EventBackPress>
    , IHandle<ControlPanelController.EventHomePress>
    , SimpleGUI.IInitialize
{
    public NumberInputBlockController InputBlock;
    public ControlPanelController ControlPanelHome;
    public ControlPanelController ControlPanelGame;
    public ThumbnailView ThumbnailView;
    public LevelManager LevelManager;
    public LevelIconController LevelIcon;
    public SurvivalIcon SurvivalIcon;
    public Animator WarningAnimator;
    public int LevelID { get; set; }
    
    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public void SetMode(StateGameplay.GameMode mode)
    {
        if (mode == StateGameplay.GameMode.Regular)
        {
            // show icon, hide survival level index
            LevelIcon.gameObject.SetActive(true);
            SurvivalIcon.gameObject.SetActive(false);
        }
        else // one of survival mode
        {
            // hide icon, show survival level index
            LevelIcon.gameObject.SetActive(false);
            SurvivalIcon.SetLevelNumber(LevelID);
            SurvivalIcon.gameObject.SetActive(true);
        }
    }

    public override void StartAppearAnimation()
    {
        base.StartAppearAnimation();

        SetMode(StateGameplay.Instance.CurrentGameMode);

        if(AppStateManager.Instance.IsCurrentState(StateHome.Instance))
            ControlPanelHome.SetupPanel("ButtonBack", "CoinIndicatorGlobal");
        else if (AppStateManager.Instance.IsCurrentState(StateGameplay.Instance))
            ControlPanelGame.SetupPanel("ButtonHome", "CoinIndicatorGlobal");

        // assign thumbnail
        var levelPrefab = StateGameplay.Instance.CurrentGameMode == StateGameplay.GameMode.Regular ? LevelManager.GetLevel(LevelID) : LevelManager.GetSurvivalLevel(StateGameplay.Instance.CurrentGameMode);
        var thumbnail = levelPrefab.GetComponent<LevelEnvironmentSettings>().Thumbnail;
        ThumbnailView.SetSprite(thumbnail);

        if(StateGameplay.Instance.CurrentGameMode == StateGameplay.GameMode.Regular)
            LevelIcon.Init(UserAccounts.Instance.GetCurrentAccount().AccountData.LevelStates[LevelID]);
        InputBlock.Setup(maxValue: Mathf.Min(UserAccounts.Instance.GetCurrentAccount().AccountData.IslandLevel, UserAccounts.Instance.GetCurrentAccount().AccountData.Coins));
        InputBlock.LimitReached += OnLimitReached;
    }

    private void OnLimitReached()
    {
        var trigger = UserAccounts.Instance.GetCurrentAccount().AccountData.Coins >=
                      UserAccounts.Instance.GetCurrentAccount().AccountData.IslandLevel
            ? "LevelWarning"
            : "CoinWarning";
        WarningAnimator.SetTrigger(trigger);
    }

    public void Handle(ControlPanelController.EventBackPress message)
    {
        if (IsInputEnabled)
            SimpleGui.PopScreen("Popup.Expedition");
    }

    public void Handle(ControlPanelController.EventHomePress message)
    {
        if (!IsInputEnabled)
            return;
        Assert.IsTrue(AppStateManager.Instance.IsCurrentState(StateGameplay.Instance));
        StateGameplay.Instance.ReturnToHomeState();
    }

    public int GetExpeditionCoins()
    {
        return InputBlock.Value;
    }

    public void OnGoPress()
    {
        StartCoroutine(StartGame());
    }


    IEnumerator StartGame()
    {
        // if we spend some money for expedition wait for one second to emphasize deduction
        var expeditionSpend = InputBlock.Value;
        if (expeditionSpend > 0)
        {
            StateHome.Instance.SpendCoins(expeditionSpend);
            yield return new WaitForSeconds(1f);
        }

        StateHome.Instance.StartGame(GetExpeditionCoins(), LevelID);
    }


}
