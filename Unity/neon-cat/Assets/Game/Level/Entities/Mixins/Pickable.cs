using System;
using DG.Tweening;
using Game;
using Game.Level.Entities;
using GameLib.Alg;
using UnityEngine;
using UnityEngine.Assertions;

[SelectionBase]
public class Pickable :
    Node,
    PortalSuckIn.ISuckingSupport,
    PortalOut.IOutingPortalSupport
{
    public enum ItemRank
    {
        ItemRankHighest = 0,
        ItemRankHigh = 1,
        ItemRankMedium = 2,
        ItemRankLow = 3,
        ItemRankLowest = 4,
    }

    public Rigidbody Rigidbody;
    public Transform PickupVisual;
    public int Quantity;
    public Collider Collider;
    public float SuckInDuration;
    public bool UseGravity;

    public Action<Pickable> OnPickup;

    private bool _isInSucking;
    private bool _isInOuting;

    void OnValidate()
    {
        Assert.IsNotNull(Rigidbody);
        Rigidbody.useGravity = UseGravity;
        Assert.IsTrue(SuckInDuration != 0f, $"{transform.GetDebugName()}");
        
    }

    public ItemRank GetItemRank()
    {
        return ItemRank.ItemRankLowest;
    }

    public void EnablePhysics(bool flag)
    {
        Rigidbody.detectCollisions = flag;
        if (UseGravity)
            Rigidbody.useGravity = flag;
        Collider.enabled = flag;
    }

    #region ISuckingSupport
    public void OnStartSuckIn(Transform suckingCenter, float portalRadius, TweenCallback onFinishSuckIn)
    {
        _isInSucking = true;
        EnablePhysics(false);

        // move
        var moveTo = gameObject.AddComponent<MoveTo>();
        moveTo.Duration = SuckInDuration;
        moveTo.Ease = Ease.InOutQuart;
        moveTo.Target = suckingCenter;
        moveTo.OffsetStartingTime(0.3f);

        // scale
        PickupVisual.DOScale(Vector3.zero, SuckInDuration * 0.7f).SetEase(Ease.InCirc)
            .OnComplete(()=>
            {
                OnPickup?.Invoke(this);
                onFinishSuckIn?.Invoke();
            });
    }

    public bool IsInSucking()
    {
        return _isInSucking;
    }

    #endregion

    #region IOutingPortalSupport

    private TweenCallback _finishOutCallback;
    public void OnStartOut(TweenCallback onFinishCallback)
    {
        _finishOutCallback = onFinishCallback;
        _isInOuting = true;
        PickupVisual.localScale = Vector3.zero;
        EnablePhysics(false);
        PickupVisual.DOScale(Vector3.one, SuckInDuration).OnComplete(OnFinishOut);
    }

    private void OnFinishOut()
    {
        EnablePhysics(true);
        _isInOuting = false;
        _finishOutCallback();
    }

    public bool IsInOuting()
    {
        return _isInOuting;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    #endregion
}
