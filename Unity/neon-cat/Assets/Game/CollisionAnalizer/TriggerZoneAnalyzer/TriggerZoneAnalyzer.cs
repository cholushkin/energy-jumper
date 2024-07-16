using Game;
using UnityEngine;
using UnityEngine.Assertions;

[ScriptExecutionOrder(-1)] // todo: ScriptExecutionOrderDependsOn(typeof(ForceField))
public abstract class TriggerZoneAnalyzer : CollisionAnalyzerBase
{
    void OnTriggerEnter(Collider otherCollider)
    {
        var mainGameObject = otherCollider.gameObject.GetMainGameObject();
        var rigidBody = mainGameObject.GetComponent<Rigidbody>();
        if (rigidBody == null)
            return; // "zone analyzer works only with rigibodies"
        var node = mainGameObject.GetComponent<Node>(); // could be null
        CollisionEnter(mainGameObject, node, rigidBody, otherCollider);
    }

    void OnTriggerStay(Collider otherCollider)
    {
        var mainGameObject = otherCollider.gameObject.GetMainGameObject();
        var rigidBody = mainGameObject.GetComponent<Rigidbody>();
        if (rigidBody == null)
            return; // "zone analyzer works only with rigibodies"
        var node = mainGameObject.GetComponent<Node>(); // could be null
        CollisionStay(mainGameObject, node, rigidBody, otherCollider);
    }

    void OnTriggerExit(Collider otherCollider)
    {
        var mainGameObject = otherCollider.gameObject.GetMainGameObject();
        Assert.IsNotNull(mainGameObject);
        var node = mainGameObject.GetComponent<Node>(); // optional
        CollisionExit(mainGameObject, otherCollider);
    }
}
