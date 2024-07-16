using Events;
using GameGUI;

public class ScreenLevelSelection 
    : GUIScreenBase
    , IHandle<ControlPanelController.EventBackPress>
    , SimpleGUI.IInitialize
{
    public ControlPanelController ControlPanel;
    public LevelSelectionController LevelSelection;

    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this, notifyDisabled:false);
    }

    public override void StartAppearAnimation()
    {
        base.StartAppearAnimation();
        LevelSelection.SyncToData();

        var page = UserAccounts.Instance.GetCurrentAccount().AccountData.LastChapterPageOpened;
        LevelSelection.OpenPage(page);
        ControlPanel.SetupPanel("ButtonBack", "CoinIndicatorGlobal");
    }

    public override void OnRestore()
    {
        base.OnRestore();
        ControlPanel.SetupPanel("ButtonBack", "CoinIndicatorGlobal");
    }

    public override void OnPopped()
    {
        base.OnPopped();
        UserAccounts.Instance.GetCurrentAccount().AccountData.LastChapterPageOpened =
            LevelSelection.View.SwipePages.CurrentPage;
        UserAccounts.Instance.GetCurrentAccount().Save();
    }

    public void OnLevelSelected(int chapter, int level)
    {
        SimpleGui.PushScreen<PopupExpedition>("Popup.Expedition", expeditionPopup =>
        {
            expeditionPopup.LevelID = level;
        });
    }

    public void Handle(ControlPanelController.EventBackPress message)
    {
        if (!IsInputEnabled)
            return;
        ControlPanel.SetupPanel("CoinIndicatorGlobal");
        SimpleGui.PopScreen("Screen.LevelSelection");
    }
}
