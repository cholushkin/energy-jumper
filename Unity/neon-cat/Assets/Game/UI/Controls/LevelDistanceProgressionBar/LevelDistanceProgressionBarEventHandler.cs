using Events;
using Game.Level.Entities;
using GameGUI;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelDistanceProgressionBarEventHandler
    : MonoBehaviour
    , SimpleGUI.IInitialize
    , IHandle<PortalControllerBase.EventPortalSpawned>
    , IHandle<PortalOut.EventSpawnPlayer>
    , IHandle<PortalSuckIn.EventPlayerExitLevel>
{

    private LevelDistanceProgressionBarController _controller;

    public void Initialize()
    {
        _controller = GetComponent<LevelDistanceProgressionBarController>();
        Assert.IsNotNull(_controller);
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    public void Handle(PortalControllerBase.EventPortalSpawned message)
    {
        if (!message.Portal.gameObject.CompareTag("Finish"))
            return;
        _controller.SetTarget(message.Portal.transform);
        _controller.SetLevelsToCurrent();
    }

    public void Handle(PortalOut.EventSpawnPlayer message)
    {
        _controller.SetAgent(message.SpawnedPlayer.transform);
    }

    public void Handle(PortalSuckIn.EventPlayerExitLevel message)
    {
        _controller.SetComplete();
    }
}
