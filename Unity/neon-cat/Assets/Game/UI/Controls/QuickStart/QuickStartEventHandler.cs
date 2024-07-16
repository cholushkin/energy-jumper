using Events;
using UnityEngine;

public class QuickStartEventHandler : MonoBehaviour, IHandle<ScreenHome.EventEnterHomeScreen>
{
    public QuickStartController Controller;
    public void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
        
        // load current value from data
        Controller.SetLevel(UserAccounts.Instance.GetCurrentAccount().AccountData.LastLevelIndex);
    }

    public void Handle(ScreenHome.EventEnterHomeScreen message)
    {
        // load current value from data
        Controller.SetLevel(UserAccounts.Instance.GetCurrentAccount().AccountData.LastLevelIndex);
    }

    public void OnRestoreOwnerScreen()
    {
        Controller.RechargeMultiClickPreventer();
    }
}
