using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CollisionPropagator : MonoBehaviour
{
    public interface ICollisionListener
    {
        void OnCollisionEnter(Collision collision);
        void OnCollisionExit(Collision collisionInfo);
    }

    [SerializeField]
    private List<MonoBehaviour> _targets;

    void OnCollisionEnter(Collision collision)
    {
        foreach (var monoBehaviour in _targets)
        {
            var collisionListener = monoBehaviour as ICollisionListener;
            Assert.IsNotNull(collisionListener);
            collisionListener?.OnCollisionEnter(collision);
        }
    }

    public void AddTarget(MonoBehaviour target)
    {
        Assert.IsNotNull(target);
        if(_targets == null)
            _targets = new List<MonoBehaviour>();
        _targets.Add(target);
    }
}
