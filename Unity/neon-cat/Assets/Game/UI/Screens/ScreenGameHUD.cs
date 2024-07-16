using Events;
using Game;
using GameGUI;
using UnityEngine.Assertions;

public class ScreenGameHUD 
    : GUIScreenBase
    , IHandle<ControlPanelController.EventBackPress>
    , SimpleGUI.IInitialize
{
    public class EventEnterScreenGameHUD// Spawned on enter ScreenGameHUD
    {
    }

    public ControlPanelController ControlPanel;
    public LevelDistanceProgressionBarController LevelProgressionController;

    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this, notifyDisabled:false);
    }

    public override void StartAppearAnimation()
    {
        base.StartAppearAnimation();
        ControlPanel.SetupPanel("ButtonBack", "CoinIndicatorLocal");
        GlobalEventAggregator.EventAggregator.Publish(new EventEnterScreenGameHUD());
    }

    public override void OnBecomeUnderModal(bool isUnder)
    {
        base.OnBecomeUnderModal(isUnder);
        InputHandler.Instance.EnablePlayerInputs(!isUnder);
    }

    public void Handle(ControlPanelController.EventBackPress message)
    {
        var needHandleBackButton = StateGameplay.Instance.MainWindowSystem.GetCurrentScreen() is ScreenGameHUD
                                  || StateGameplay.Instance.MainWindowSystem.GetCurrentScreen() is ScreenBubbleDialog;
        if (!needHandleBackButton)
            return;
        StateGameplay.Instance.SetPause(true);
        SimpleGui.PushScreen("Popup.GameOver");
        var gameOverScreen = SimpleGui.GetCurrentScreen() as PopupGameOver;
        Assert.IsNotNull(gameOverScreen);
        gameOverScreen.SetMode(PopupGameOver.Mode.Abandon, StateGameplay.Instance.CurrentGameMode);
    }
}
