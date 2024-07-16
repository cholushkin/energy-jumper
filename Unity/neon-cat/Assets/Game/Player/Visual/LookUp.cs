using UnityEngine;

public class LookUp : MonoBehaviour
{
    public Transform Target;
    public float Speed;

    void Update()
    {
        Vector3 lTargetDir = Target.position - transform.position;
        //lTargetDir.y = 0.0f;
        var targetRot = Quaternion.LookRotation(-lTargetDir, Vector3.up);

        //var curAngle = Quaternion.Angle(targetRot, transform.rotation);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.unscaledDeltaTime * Speed);
    }
}
