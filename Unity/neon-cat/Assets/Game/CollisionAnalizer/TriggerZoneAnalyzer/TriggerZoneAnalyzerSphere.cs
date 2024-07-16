using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneAnalyzerSphere : TriggerZoneAnalyzer
{
    public SphereCollider SphereCollider;

    // returns normalized distance to the center of the sphere
    public override (float normalizedDistance, float distance, Vector3 normal) GetDistanceTo(Entry item)
    {
        var itemCenterLocal = transform.InverseTransformPoint(item.MainGameObject.transform.position);
        var normalLocal = itemCenterLocal.normalized;
        return (itemCenterLocal.magnitude / SphereCollider.radius, itemCenterLocal.magnitude, transform.TransformVector(normalLocal).normalized);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(SphereCollider.transform.position, SphereCollider.radius * CenterZone);
    }
}
