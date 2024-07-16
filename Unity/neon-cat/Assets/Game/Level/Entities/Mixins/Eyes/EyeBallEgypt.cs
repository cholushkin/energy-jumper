using UnityEngine;
using UnityEngine.Assertions;

public class EyeBallEgypt : MonoBehaviour
{
    public Transform EyeBallMesh;
    public float SmoothTime;
    private EyeRoller _eyeRollerController;
    private Quaternion _targetRotation;
    private Quaternion _relaxRotation;

    private Vector3 _velocity;

    void Awake()
    {
        _eyeRollerController = GetComponentInParent<EyeRoller>();
        _relaxRotation = transform.rotation;
        Assert.IsNotNull(_eyeRollerController);
    }

    void Update()
    {
        if (_eyeRollerController.IsInFocus())
        {
            _targetRotation = Quaternion.LookRotation(_eyeRollerController.DirectionToTarget);
        }
        else
        {
            _targetRotation = _relaxRotation;
        }

        //transform.rotation = _targetRotation;

        transform.rotation = SmoothDampQuaternion(transform.rotation, _targetRotation, ref _velocity, SmoothTime);

    }

    private static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
            Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
            Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
            Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }

}
