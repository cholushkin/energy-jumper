using UnityEngine;

[ScriptExecutionOrder(-100)]
public class ForceImpact : MonoBehaviour
{
    public Rigidbody Rigidbody;
    private Vector3 _forceImpact;

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        if (forceMode == ForceMode.Force)
        {
            _forceImpact += force;
        }
        else if (forceMode == ForceMode.Acceleration)
        {
            _forceImpact += Rigidbody.mass * force;
        }
    }

    public Vector3 GetForceImpact()
    {
        return _forceImpact;
    }

    void FixedUpdate()
    {
        _forceImpact = Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(Rigidbody.worldCenterOfMass, _forceImpact );
    }
}
