using DG.Tweening;
using Events;
using Game;
using UnityEngine;
using UnityEngine.Assertions;

public class DeadlyWaterCube : MonoBehaviour, IHandle<PlayerController.EventDeath>
{
    public Vector3 MoveCameraAfterDeathOffset;

    private void Awake()
    {
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        var mainGameObject = other.gameObject.GetMainGameObject();
        
        // for player
        if (mainGameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log($"Death by deadly water");
            var playerController = other.gameObject.GetComponent<PlayerController>();
            Assert.IsNotNull(playerController);

            var curPos = GameCamera.Instance.Follower.transform.position;
            
            GlobalEventAggregator.EventAggregator.Publish(new PlayerController.EventDeath { Player = playerController, Reason = PlayerController.EventDeath.DeathReason.DeadlyWater, ReasonObject = gameObject });
        }

        // all objects
        var rb = mainGameObject.GetComponent<Rigidbody>();
        if(rb == null)
            return;

        ArchimedeanForce.Apply(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        ArchimedeanForce.Remove(other.attachedRigidbody);
    }
 
    public void Handle(PlayerController.EventDeath message)
    {
        const float moveCameraAfterDeathDuration = 2f;
        //GameCamera.Instance.Focus(transform.position + MoveCameraAfterDeathOffset, moveCameraAfterDeathDuration, Ease.OutQuart);
    }
}