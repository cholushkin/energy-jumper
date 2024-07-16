using UnityEngine;

public class ControlPanelController : MonoBehaviour
{
    public class EventBackPress // Spawned when ControlPanel Back button pressed
    { }

    public class EventHomePress // Spawned when ControlPanel Home button pressed
    { }

    public class EventOptionsPress // Spawned when ControlPanel Options button pressed
    { }

    public ControlPanelView View;


    public void SetupPanel(params string[] controlNames)
    {
        View.SyncTo(controlNames);
    }

    public void Clear()
    {
        View.Clear();
    }


    public void OnBackPress()
    {
        GlobalEventAggregator.EventAggregator.Publish(new EventBackPress());
    }

    public void OnHomePress()
    {
        GlobalEventAggregator.EventAggregator.Publish(new EventHomePress());
    }

    public void OnOptionsPress()
    {
        GlobalEventAggregator.EventAggregator.Publish(new EventOptionsPress());
    }

    public void OnCoinPress()
    {

    }
}
