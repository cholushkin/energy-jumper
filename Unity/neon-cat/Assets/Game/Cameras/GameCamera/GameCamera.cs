using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameLib;
using GameLib.Alg;
using UnityEngine;
using UnityEngine.Assertions;


[ScriptExecutionOrder(-100)]
public class GameCamera : Singleton<GameCamera>, ICameraController
{
    public ScreenTransitionProcessor CameraFader;
    public Follower Follower;
    public Shaker Shaker;
    public Rotator Rotator;
    public CameraZoomerPerspective Zoomer;
    public LookAt Looker;
    public DistanceMover DistanceMover;
    public Camera Camera;
    public Canvas Canvas;
    private Tweener _moveTween;


    public Camera GetCamera()
    {
        return Camera;
    }

    public void Zoom(float zoom, float duration, TweenCallback zoomCallback = null)
    {
        Zoomer.Zoom(duration, zoom, zoomCallback);
    }

    public void Follow(Transform target, TweenCallback followCallback = null)
    {
        if (_moveTween != null && _moveTween.IsPlaying())
        {
            _moveTween.Kill();
            _moveTween = null;
        }
        Follower.Follow(target);
        if (followCallback != null)
            Follower.SetCallback(followCallback);
    }

    public void Focus(Vector3 position, bool instant = false, TweenCallback focusCallback = null)
    {
        if (_moveTween != null && _moveTween.IsPlaying())
        {
            _moveTween.Kill();
            _moveTween = null;
        }

        Follower.Follow(position, instant);
        if (focusCallback != null)
            Follower.SetCallback(focusCallback);
    }

    public void Focus(Vector3 position, float duration, Ease ease = Ease.Linear, TweenCallback focusCallback = null, bool unscaledDeltaTime = false)
    {
        Follower.Follow(null);
        _moveTween = transform.DOMove(position, duration)
            .SetUpdate(unscaledDeltaTime)
            .OnComplete(focusCallback)
            .SetEase(ease);
    }

    public void Rotate(float eulerY, float duration, TweenCallback rotationCallback = null)
    {
        Rotator.RotateTo(eulerY, duration, rotationCallback);
    }

    public void Shake(float duration, int shakeMode, TweenCallback shakeCallback)
    {
        Shaker.Shake(duration, shakeMode, Vector3.one, shakeCallback);
    }

    public void Shake(float duration, float shakeStrength, Vector3 shakeDir, TweenCallback shakeCallback)
    {
        var mode = 0;
        if (shakeStrength > 0.3f)
            mode = 1;
        if (shakeStrength > 0.6f)
            mode = 2;
        Shaker.Shake(duration, mode, shakeDir, shakeCallback);
    }

    public void Shake(TweenCallback shakeCallback = null)
    {
        Shaker.Shake(shakeCallback);
    }

    public void LookAt(Transform target)
    {
        Looker.Target = target;
    }

    public void Height(float height)
    {
        DistanceMover.DistancePosition.y = height;
    }

    public void Distance(float distanceFactor)
    {
        DistanceMover.CloserFactor = distanceFactor;
    }
}
