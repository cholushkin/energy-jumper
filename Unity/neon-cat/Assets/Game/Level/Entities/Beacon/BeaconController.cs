using Game;
using UnityEngine;

[SelectionBase]
public class BeaconController : MonoBehaviour
{
    public class EventBeaconSpawned // Spawned when beacon entity spawned 
    {
        public BeaconController Beacon;
    }

    public class EventBeaconActivated // Spawned when beacon activated by player
    {
        public BeaconController Beacon;
    }

    public BeaconVisual Visual;

    void Awake()
    {
        GlobalEventAggregator.EventAggregator.Publish(new EventBeaconSpawned { Beacon = this });
    }

    private void OnTriggerEnter(Collider other)
    {
        var mainGameObject = other.gameObject.GetMainGameObject();
        if (mainGameObject.GetComponent<PlayerController>() == null)
            return;

        // activate beacon
        {
            GetComponent<Collider>().enabled = false;
            Visual.PlayActivation();
            GlobalEventAggregator.EventAggregator.Publish(new EventBeaconActivated { Beacon = this });
        }
    }
}
