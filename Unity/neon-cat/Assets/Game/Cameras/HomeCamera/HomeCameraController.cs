using GameLib.Alg;
using Lean.Touch;
using UnityEngine;

public class HomeCameraController : Singleton<HomeCameraController>
{
    public ControllerRotateAroundTarget ControllerRotate;
    public ControllerPan ControllerPan;
    public ControllerDistanceZoom ControllerZoom;
    public Camera Camera;

    private int _animatingCounters;



    public void SetFocusTarget(float rotation, float zoom, float pan)
    {
        _animatingCounters = 3;
        ControllerRotate.AnimateTo(rotation, () => _animatingCounters--);
        ControllerPan.AnimateTo(pan, () => _animatingCounters--);
        ControllerZoom.AnimateTo(zoom, () => _animatingCounters--);
    }

    public void SetRelaxing()
    {
        _animatingCounters = 0;
        ControllerRotate.SwitchToRestoringState();
        ControllerPan.SwitchToRestoringState();
        ControllerZoom.SwitchToRestoringState();
    }

    public bool IsAnimatingToValue()
    {
        return _animatingCounters > 0;
    }

    public bool CheckUserProcessing()
    {
        var result = ControllerRotate.IsUserProcessing()
                     && ControllerPan.IsUserProcessing()
                     && ControllerZoom.IsUserProcessing();
        return result;
    }

    public void SetEnabled(bool isInputEnabled)
    {
        ControllerRotate.enabled = isInputEnabled;
        ControllerZoom.enabled = isInputEnabled;
        ControllerPan.enabled = isInputEnabled;
    }

    public Camera GetCamera()
    {
        return Camera;
    }
}