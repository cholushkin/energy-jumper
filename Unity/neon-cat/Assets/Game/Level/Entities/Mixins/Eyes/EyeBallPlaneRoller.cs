using UnityEngine;
using UnityEngine.Assertions;

public class EyeBallPlaneRoller : MonoBehaviour
{
    public float Radius;
    public Transform EyeBallMesh;
    public float SmoothTime;
    private EyeRoller _eyeRollerController;
    private Vector3 _ballTargetPosition;
    private Vector3 _velocity;

    void Awake()
    {
        _eyeRollerController = GetComponentInParent<EyeRoller>();
        Assert.IsNotNull(_eyeRollerController);
    }


    void Update()
    {
        if (_eyeRollerController.IsInFocus())
        {
            _ballTargetPosition =
                transform.position + _eyeRollerController.DirectionToTarget * _eyeRollerController.Factor;
        }
        else
        {
            _ballTargetPosition = transform.position; // centered
        }

        EyeBallMesh.transform.position = Vector3.SmoothDamp(EyeBallMesh.transform.position, _ballTargetPosition,
            ref _velocity, SmoothTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
