using Events;
using GameGUI;

public class PopupOptions 
    : GUIScreenBase
    , IHandle<ControlPanelController.EventBackPress>
    , SimpleGUI.IInitialize
{
    public ControlPanelController ControlPanel;
    public StateButtonView SoundButton;
    private UserAccount.Data.OptionsState _state;


    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }


    public override void Awake()
    {
        base.Awake();
        InitializeState();
        SyncToState();
    }

    public override void StartAppearAnimation()
    {
        base.StartAppearAnimation();
        ControlPanel.SetupPanel("ButtonBack");
    }

    public void OnButtonNoAdsTap()
    {
        // do logic
        if (NoAdsPaymentCheck())
        {
            // update _state

            // sync to state
            SyncToState();

            // save state
            UserAccounts.Instance.GetCurrentAccount().Save();
        }
    }

    public void OnButtonNoSoundTap()
    {
        // do logic
        var newSoundState = SwitchSound(_state.Sound);

        // update state
        _state.Sound = newSoundState;

        // sync to state
        SyncToState();

        // save state
        UserAccounts.Instance.GetCurrentAccount().Save();
    }

    public void OnButtonNoVibrationTap()
    {
        print("OnButtonNoVibrationTap");
    }

    public void OnButtonHelpTap()
    {
        print("OnButtonHelpTap");
    }

    public void OnButtonGraphicsTap()
    {
        print("OnButtonGraphicsTap");
    }

    public void OnButtonCancelTap()
    {
        SimpleGui.PopScreen("Popup.Options");
    }

    public void OnButtonApplyTap()
    {
        print("OnButtonApplyTap");
    }

    public void InitializeState()
    {
        var account = UserAccounts.Instance.GetCurrentAccount();
        _state = account.AccountData.Options;
    }

    public void SyncToState()
    {
        // sound
        SoundButton.SwitchState(_state.Sound ? "SoundOn" : "SoundOff");


    }
    #region actions

    private bool NoAdsPaymentCheck()
    {
        return false;
    }

    private bool SwitchSound(bool stateSound)
    {
        return !stateSound;
    }
    #endregion

    public void Handle(ControlPanelController.EventBackPress message)
    {
        if (IsInputEnabled)
            SimpleGui.PopScreen("Popup.Options");
    }


}
