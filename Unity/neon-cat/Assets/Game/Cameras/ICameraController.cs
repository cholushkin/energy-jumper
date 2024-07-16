using DG.Tweening;
using UnityEngine;

public interface ICameraController
{
    Camera GetCamera();
    //void FadeToColor(Color color, TweenCallback fadeinCallback, bool isInstant);
    //void FadeToTransparency(TweenCallback fadeoutCallback, bool isInstant);
    void Zoom(float zoom, float duration, TweenCallback zoomCallback);
    void Follow(Transform target, TweenCallback followCallback);
    void Focus(Vector3 position, bool instant, TweenCallback focusCallback);
    void Rotate(float eulerY, float duration, TweenCallback rotationCallback);
    void Shake(float duration, int shakeMode, TweenCallback shakeCallback);
    void Shake(TweenCallback shakeCallback);
    void LookAt(Transform target);
    void Height(float height);
    void Distance(float distanceFactor);
}
