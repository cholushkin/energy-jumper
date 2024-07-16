using Events;
using GameGUI;
using UnityEngine;

public class HomeUpgradeEventHandler 
    : MonoBehaviour
    , SimpleGUI.IInitialize
    , IHandle<ScreenHome.EventEnterHomeScreen>
{
    public HomeUpgradeController Controller;

    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }
    void Start()
    {
        Controller.SetPrice(true);
    }

    public void Handle(ScreenHome.EventEnterHomeScreen message)
    {
        
    }
}
