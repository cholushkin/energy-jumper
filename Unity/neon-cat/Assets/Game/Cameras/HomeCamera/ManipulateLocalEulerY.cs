using UnityEngine;

public class ManipulateLocalEulerY : MonoBehaviour
{
    public float yRotateSpeed = 3.0f;
    private float deltaY;
    public Transform Normal;
    public Transform CameraObj;
    public bool UseRestrictions;
    private float _currentSign;
    private const float _dumpingFactor = 0.1f;

    void Start()
    {
        var crossProduct = Vector3.Cross(Normal.up, CameraObj.position - transform.position);
        _currentSign = Vector3.Dot(crossProduct, Normal.right) < 0f ? -1f : 1f;
    }

    void Update()
    {
        deltaY = Input.GetAxis("Vertical") * yRotateSpeed;
        Manipulate(deltaY);

        // damping back to correct values 
        if (UseRestrictions && Vector3.Dot(Normal.up, CameraObj.position - transform.position) < 0f)
        {
            var rotation = Quaternion.Euler(_dumpingFactor * _currentSign, 0, 0);
            transform.localRotation *= rotation;
        }
    }

    public float GetSwapSign()
    {
        var toCamV3 = CameraObj.position - transform.position;
        var crossNormalToCam = Vector3.Cross(Normal.up, toCamV3);
        var dot = Vector3.Dot(transform.right, crossNormalToCam);
        return dot < 0f ? -1f : 1f;
    }

    public void Manipulate(float deltaY)
    {
        var rotation = Quaternion.Euler(deltaY * _currentSign, 0, 0);
        var prev = transform.localRotation;
        transform.localRotation *= rotation;

        if (UseRestrictions && Vector3.Dot(Normal.up, CameraObj.position - transform.position) < 0f)
        {
            transform.localRotation = prev;
            return;
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        var toCamV3 = CameraObj.position - transform.position;
        var crossNormalToCam = Vector3.Cross(Normal.up, toCamV3);
        Gizmos.color = GetSwapSign() < 0f ? Color.yellow : Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + crossNormalToCam);
    }
}