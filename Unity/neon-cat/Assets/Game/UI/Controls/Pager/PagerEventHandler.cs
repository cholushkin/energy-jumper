using Events;
using GameGUI;
using UnityEngine;

public class PagerEventHandler 
    : MonoBehaviour
    , SimpleGUI.IInitialize
    , IHandle<SwipePages.EventPageOpened>
{
    public PagerController Controller;


    public void Initialize()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public void Handle(SwipePages.EventPageOpened message)
    {
        Controller.SyncToPage(message.Page);
    }


}
