using Events;
using Game;
using GameGUI;
using UnityEngine;
using UnityEngine.Assertions;

public class CoinIndicatorGameplayEventHandler : MonoBehaviour
    , SimpleGUI.IInitialize
    , IHandle<ScreenGameHUD.EventEnterScreenGameHUD>
    , IHandle<StateGameplay.EventSpendCoinsSession>
    , IHandle<StateGameplay.EventReceiveCoinsSession>
{
    public CoinIndicatorController Controller;

    public void Initialize()
    {
        Assert.IsNotNull(Controller);
        GlobalEventAggregator.EventAggregator.Subscribe(this, notifyDisabled:false);
    }

    public void Handle(ScreenGameHUD.EventEnterScreenGameHUD message)
    {
        Controller.InitCoins(0);
        if (StateGameplay.Instance.EntryCoins > 0)
            Controller.AddCoins(StateGameplay.Instance.EntryCoins);
    }
            
    public void Handle(StateGameplay.EventSpendCoinsSession message)
    {
        Controller.UpdateCoins(message.SessionData.GetCoins());
    }

    public void Handle(StateGameplay.EventReceiveCoinsSession message)
    {
        Controller.UpdateCoins(message.SessionData.GetCoins());
    }
}
