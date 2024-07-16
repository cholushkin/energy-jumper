using Events;
using Game;
using GameGUI;
using UnityEngine;
using UnityEngine.Assertions;

public class ScreenHome 
    : GUIScreenBase
    , SimpleGUI.IInitialize
    , IHandle<ControlPanelController.EventOptionsPress>
{
    public class EventEnterHomeScreen // Spawned on enter HomeScreen
    {
    }
    
    public class EventExitHomeScreen // Spawned on exit HomeScreen
    {
    }

    public QuickStartController QuickStartButton;
    public ControlPanelController ControlPanel;
    public HomeCameraController HomeCameraController;
    public GameObject LevelSelectionButton;
    public SurvivalButton[] LevelSurvivalButtons;


    public bool HidePlayButtonOnUserInput;



    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }
    public void Update()
    {
        // hiding on user input
        if (HomeCameraController.Instance.CheckUserProcessing() && HidePlayButtonOnUserInput)
        {
            QuickStartButton.ProccessHiding();
        }

        HomeCameraController.SetEnabled(IsInputEnabled);
    }

    public override void StartAppearAnimation()
    {
        base.StartAppearAnimation();
        ControlPanel.SetupPanel("ButtonOptions", "CoinIndicatorGlobal");
        GlobalEventAggregator.EventAggregator.Publish(new EventEnterHomeScreen());
    }

    public override void StartDisappearAnimation()
    {
        base.StartDisappearAnimation();
        GlobalEventAggregator.EventAggregator.Publish(new EventExitHomeScreen());
    }

    public override void DisappearForced()
    {
        base.DisappearForced();
        GlobalEventAggregator.EventAggregator.Publish(new EventExitHomeScreen());
    }

    public override void OnRestore()
    {
        ControlPanel.SetupPanel("ButtonOptions", "CoinIndicatorGlobal");
        QuickStartButton.GetComponent<QuickStartEventHandler>().OnRestoreOwnerScreen();
    }

    public void OnStartButtonTap()
    {
        StateGameplay.Instance.CurrentGameMode = StateGameplay.GameMode.Regular;
        var levelID = QuickStartButton.GetLevel();
        SimpleGui.PushScreen<PopupExpedition>("Popup.Expedition", expeditionPopup =>
            {
                expeditionPopup.LevelID = levelID;
            });
    }

    public void OnSurvivalLevelTap(SurvivalButton sender)
    {
        var levelID = UserAccounts.Instance.GetCurrentAccount().AccountData.GetSurvivalLevelState(sender.Mode).LastLevelIndex;
        Assert.IsTrue(levelID > -1);
        if(levelID == -1)
            return;

        StateGameplay.Instance.CurrentGameMode = sender.Mode;
        StateGameplay.Instance.LevelID = levelID;

        SimpleGui.PushScreen<PopupExpedition>("Popup.Expedition", expeditionPopup =>
        {
            expeditionPopup.LevelID = levelID;
        });
    }

    public void OnHomeUpgradeTap()
    {
        QuickStartButton.ProccessHiding();
    }

    public void OnLevelSelectButtonTap()
    {
        StateGameplay.Instance.CurrentGameMode = StateGameplay.GameMode.Regular;
        SimpleGui.PushScreen("Screen.LevelSelection");
    }
                                              
    public void Handle(ControlPanelController.EventOptionsPress message)
    {        
        SimpleGui.PushScreen("Popup.Options");
    }

    public void SetLevelSelectionButtonEnabled(bool flag)
    {
        LevelSelectionButton.SetActive(flag);
    }

    public void SetQuickStartButtonEnabled(bool flag)
    {
        QuickStartButton.gameObject.SetActive(flag);
    }

    // survivalTypeIndex - mode of the game or button index in the menu
    public void SetSurvivalButton(int survivalTypeIndex, int lastLevelIndex)
    {
        var enableButton = lastLevelIndex > -1;
        LevelSurvivalButtons[survivalTypeIndex].gameObject.SetActive(enableButton);

        if (enableButton)
        {
            // update data on it
            LevelSurvivalButtons[survivalTypeIndex].SetLevelNumber(lastLevelIndex);
        }
    }
}
