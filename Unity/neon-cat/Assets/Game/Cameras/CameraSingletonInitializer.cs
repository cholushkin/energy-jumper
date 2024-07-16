using UnityEngine;

[ScriptExecutionOrder(-100)]
public class CameraSingletonInitializer : MonoBehaviour
{
    public HomeCameraController HomeCameraSingleton;
    public GameCamera GameCameraSingleton;
    void Awake()
    {
        HomeCameraSingleton.AssignInstance();
        GameCameraSingleton.AssignInstance();
    }
}
