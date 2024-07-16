using GameLib.Alg;
using UnityEngine;
using UnityEngine.Assertions;

public class BehaviourDistanceActivator : MonoBehaviour
{
    public enum Mode
    {
        OneShoot,
        Switcher,
    }

    public Mode WorkingMode;
    public float ActivationDistance;

    public Behaviour Host; // object to disable if the distance between Transform and Target exceeds ActivationDistance
    public Transform Target; // target to analize distance from

    void Reset()
    {
        ActivationDistance = 20f;
    }

    void Start()
    {
        Assert.IsNotNull(Target, "target is null on " + transform.GetDebugName());
    }

    private void Update()
    {
        if (Target == null)
            return;

        var isInActivationZone = (Target.gameObject.transform.position - transform.position).magnitude < ActivationDistance;

        // activate
        if (isInActivationZone && Host.enabled == false)
        {
            Host.enabled = true;
            return;
        }

        // deactivate
        if (!isInActivationZone && Host.enabled && WorkingMode == Mode.Switcher)
            Host.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, ActivationDistance);
    }
}
