using UnityEngine;
using UnityEngine.Assertions;

public class TriggerZoneAnalyzerBox : TriggerZoneAnalyzer
{
    public enum Surface
    {
        Left,
        Top,
        Right,
        Bottom,
        Front,
        Back
    }

    public BoxCollider BoxCollider;
    public Surface OriginSurface;

    
    // return normalized distance from center of the object to ZeroSurface
    public override (float normalizedDistance, float distance, Vector3 normal) GetDistanceTo(Entry item)
    {
        Assert.IsTrue(OriginSurface == Surface.Left, "Only left zero surface is supported for now");
        var itemCenterLocal = transform.InverseTransformPoint(item.MainGameObject.transform.position);
        var leftSurfaceCenterLocal = BoxCollider.center + Vector3.left * BoxCollider.size.x * 0.5f; // local
        var normalLocal = Vector3.right;
        return (itemCenterLocal.x - leftSurfaceCenterLocal.x / BoxCollider.size.x, itemCenterLocal.magnitude, transform.TransformVector(normalLocal).normalized);
    }

    void OnDrawGizmos()
    {
        if (OriginSurface == Surface.Left)
        {
            var leftSurfaceCenterLocal = BoxCollider.center + Vector3.left * BoxCollider.size.x * 0.5f; // local
            var leftSurfaceCenter = transform.TransformPoint(leftSurfaceCenterLocal);
            Gizmos.color = Color.red * 0.5f;
            Gizmos.DrawSphere(leftSurfaceCenter, 0.33f);
        }
    }
}
