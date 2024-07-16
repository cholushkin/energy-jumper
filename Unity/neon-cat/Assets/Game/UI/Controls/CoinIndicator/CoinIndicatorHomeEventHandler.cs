using Events;
using Game;
using GameGUI;
using UnityEngine;

public class CoinIndicatorHomeEventHandler 
    : MonoBehaviour
    , SimpleGUI.IInitialize
    , IHandle<ScreenHome.EventEnterHomeScreen>
    , IHandle<StateHome.EventSpendCoinsAtHome>
    , IHandle<StateHome.EventReceiveCoinsAtHome>
{
    public CoinIndicatorController Controller;
    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this, notifyDisabled:false);
    }

    void OnEnable()
    {
        InitCoinsValue();
    }

    #region Event handling ####################################################
    public void Handle(ScreenHome.EventEnterHomeScreen message)
    {
        UpdateCoinsValue();
    }

    public void Handle(StateHome.EventSpendCoinsAtHome message)
    {
        var account = UserAccounts.Instance.GetCurrentAccount();
        var coins = account.AccountData.Coins;
        Controller.UpdateCoins(coins);
    }

    public void Handle(StateHome.EventReceiveCoinsAtHome message)
    {
        var account = UserAccounts.Instance.GetCurrentAccount();
        var coins = account.AccountData.Coins;
        Controller.UpdateCoins(coins);
    }
    #endregion
    
    private void InitCoinsValue()
    {
        var account = UserAccounts.Instance.GetCurrentAccount();
        var coins = account.AccountData.Coins;
        Controller.InitCoins(coins);
    }

    private void UpdateCoinsValue()
    {
        // load current value from data and update current indicated value with animation
        var account = UserAccounts.Instance.GetCurrentAccount();
        var coins = account.AccountData.Coins;
        Controller.UpdateCoins(coins);
    }
}
