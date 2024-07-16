using Events;
using Game;
using GameGUI;
using UnityEngine;

public class HomeLevelIndicatorEventHandler
    : MonoBehaviour
    , SimpleGUI.IInitialize
    , IHandle<ScreenHome.EventEnterHomeScreen>
    , IHandle<ScreenHome.EventExitHomeScreen>
    , IHandle<HomeIslandController.EventIslandUpgrade>
{
    public HomeLevelIndicatorController Controller;
    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public void Handle(ScreenHome.EventEnterHomeScreen message)
    {
        Controller.SetVisibility(true);

        // load current value from data
        var account = UserAccounts.Instance.GetCurrentAccount();
        var level = account.AccountData.IslandLevel;
        Controller.Setup(level);
    }

    public void Handle(HomeIslandController.EventIslandUpgrade message)
    {
        Controller.Upgrade();
    }

    public void Handle(ScreenHome.EventExitHomeScreen message)
    {
        Controller.SetVisibility(false);
    }
}
