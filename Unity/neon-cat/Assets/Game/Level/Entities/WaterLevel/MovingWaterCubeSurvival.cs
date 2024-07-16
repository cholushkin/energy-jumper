using DG.Tweening;
using Events;
using Game;
using Game.Level.Entities;
using UnityEngine;

public class MovingWaterCubeSurvival : MonoBehaviour, IHandle<PortalOut.EventSpawnPlayer>
{
    public Vector3 MovingDistance;
    public Transform WaterCube;
    public float StartingMovingDelay;
    public Follower Follower;

    private PlayerController PlayerController;
    private Vector3 _waterTargetPoint;

    void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    void Update()
    {
        if (PlayerController == null)
            return;
        if (PlayerController.transform.position.y > _waterTargetPoint.y)
            _waterTargetPoint = PlayerController.transform.position;
    }

    public void EnableWaterMovement(bool flag)
    {
        if (flag)
            Follower.Follow(_waterTargetPoint);
        else
            Follower.Follow(null);
    }

    public void Handle(PortalOut.EventSpawnPlayer message)
    {
        PlayerController = message.SpawnedPlayer.GetComponent<PlayerController>();
        DOVirtual.DelayedCall(StartingMovingDelay, () => EnableWaterMovement(true), false);
    }
}
