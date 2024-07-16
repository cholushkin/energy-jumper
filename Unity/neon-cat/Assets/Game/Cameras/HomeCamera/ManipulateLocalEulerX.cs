using UnityEngine;

public class ManipulateLocalEulerX : MonoBehaviour
{
    public float xRotateSpeed = 3.0f;

    void Update()
    {
        var deltaX = Input.GetAxis("Horizontal") * xRotateSpeed;
        Manipulate(deltaX);
    }

    public void Manipulate(float deltaX)
    {
        var rotation = Quaternion.Euler(0, -deltaX, 0);
        transform.localRotation *= rotation;
    }

    public void Relax()
    {
        const float rotSpeed = 1.0f / 5f;
        transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, rotSpeed * Time.deltaTime);
    }
}
