using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsFollower : MonoBehaviour
{
    public Transform Target;
    public float Radius;
    public float MaxForce;

    public AnimationCurve ForceDistributionToCenter;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (Target == null)
            return;
        var rb = GetComponent<Rigidbody>();
        var delta = Target.position -rb.position;
        var factor = Mathf.Clamp01(delta.magnitude / Radius); // 0 - center; 1 - on the radius
        var forceDistFactor = ForceDistributionToCenter.Evaluate(factor);
        rb.AddForce(delta.normalized * MaxForce * forceDistFactor);
    }
}
