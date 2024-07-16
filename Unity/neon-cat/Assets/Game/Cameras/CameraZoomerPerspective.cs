using DG.Tweening;
using UnityEngine;

public class CameraZoomerPerspective : MonoBehaviour
{
    public Camera Camera;

    public void Zoom(float duration, float fov, TweenCallback zoomCallback = null)
    {
        Camera.DOFieldOfView(fov, duration).OnComplete(zoomCallback).SetUpdate(true);
    }
}
