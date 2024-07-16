using GameLib.Alg;
using UnityEngine;
using UnityEngine.Assertions;

[ScriptExecutionOrder(-100)]
public class CameraControllerManager : Singleton<CameraControllerManager>
{
    public Transform CurrentCamera;

    public void SetCurrentCamera(string cameraName)
    {
        foreach (var child in transform.Children())
        {
            if (child.gameObject.name == cameraName)
            {
                Assert.IsTrue(transform.position == Vector3.zero, "");
                CurrentCamera = child;
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
