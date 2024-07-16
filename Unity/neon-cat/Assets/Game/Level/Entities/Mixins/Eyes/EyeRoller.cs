using Game;
using UnityEngine;

// Eye roll controller which works on a plane (2d projection)
// todo: multiple blind sectors support
public class EyeRoller : MonoBehaviour
{
    [Tooltip("Point of attention of the creature")]
    public Transform Target;

    [Tooltip("Target is player")]
    public bool TargetingPlayer; // trying to 

    [Tooltip("Radius in which creature pays attention to target")]
    public float DetectRadius;

    [Tooltip("How far is target to the sensor. [0..1] inside DetectRadius. >1 out of DetectRadius")]
    public float Factor { get; set; } // how far target to the sensor (0 - center; 1 - on DetectRadius; >1 further than radius)

    [Tooltip("Normalized direction to target")]
    public Vector3 DirectionToTarget { get; set; }

    public bool IsInBlindSector { get; private set; }

    [Header("Blind sector")]
    public float BlindSectorAngle;
    public float BlindSectorRotation;

    public bool IsInFocus()
    {
        if (Target == null)
            return false;
        if (IsInBlindSector)
            return false;
        return Factor <= 1f;
    }

    void Update()
    {
        // if we are targeting player 
        if (TargetingPlayer) // get player game object
        {
            var player = StateGameplay.Instance.Player;
            Target = player == null ? null : player.transform;
        }

        // no target? stop update DirectionToTarget Factor (values stay from previously focused target)
        if (Target == null)
            return;

        DirectionToTarget = Target.transform.position - transform.position;
        DirectionToTarget = Vector3.ProjectOnPlane(DirectionToTarget, -transform.forward); // project on plane
        Factor = DirectionToTarget.magnitude / DetectRadius; // 0 - center; 1 - on the radius
        DirectionToTarget = DirectionToTarget.normalized;

        var blindSectorVector = Quaternion.AngleAxis(BlindSectorRotation, transform.forward) * transform.right;

        IsInBlindSector = Vector3.Angle(blindSectorVector, DirectionToTarget) < BlindSectorAngle * 0.5f;
    }

    void OnDrawGizmos()
    {
        // draw DetectRadius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRadius);

        // draw DirectionToTarget (focused or not)
        Gizmos.color = IsInFocus() ? Color.white : Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + DirectionToTarget * DetectRadius);

        // draw blind sector
        Gizmos.color = Color.blue;

        var blindSectorVector = Quaternion.AngleAxis(BlindSectorRotation, transform.forward) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + blindSectorVector * DetectRadius);
    }
}
