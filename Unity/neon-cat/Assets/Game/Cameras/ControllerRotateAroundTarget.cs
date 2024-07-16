using System.Collections.Generic;
using DG.Tweening;
using GameLib;
using GameLib.Alg;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Assertions;

public class ControllerRotateAroundTarget : Singleton<ControllerRotateAroundTarget>
{
    enum ValueChangeMode
    {
        RestoringToDefaultValue,
        AnimateToValue,
        UserProcessing
    }


    [Tooltip("Ignore fingers with StartedOverGui?")]
    public bool IgnoreStartedOverGui = true;

    [Tooltip("Ignore fingers with IsOverGui?")]
    public bool IgnoreIsOverGui;

    [Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
    public int RequiredFingerCount;

    [Tooltip("The sensitivity of the movement, use -1 to invert")]
    public float Sensitivity = 1.0f;

    public LeanScreenDepth ScreenDepth;

    public GameObject TargetObject;
    public Quaternion DefaultTargetValue = Quaternion.identity;

    public float SmoothTime;
    public float RestoreSmoothTime;
    private float _currentSmoothTime;
    private Quaternion _target;
    private Quaternion _currentRot;
    private float _currentSensitivity;

    private bool _hasTouchPreviousFrame = false;
    private StateMachine<ValueChangeMode> _stateMachine;


    //private bool _isThisFrameUserTouched = false;
    //private bool _isThisFrameUserUntouched = false;
    private List<LeanFinger> _fingers;
    private Vector3 _velocityValueChange = Vector3.zero;
    private float _animateToValue;
    private TweenCallback _tweenCallback;


    void Reset()
    {
        RestoreSmoothTime = 8;
    }

    void Start()
    {
        Assert.IsNotNull(TargetObject);
        _currentRot = Quaternion.identity;
        _target = Quaternion.identity;
        _currentSmoothTime = SmoothTime;
        _currentSensitivity = Sensitivity;
        _stateMachine = new StateMachine<ValueChangeMode>(this, ValueChangeMode.RestoringToDefaultValue);
        _stateMachine.GoTo(ValueChangeMode.RestoringToDefaultValue);
    }

    protected virtual void LateUpdate()
    {
        // Get the fingers we want to use
        _fingers = LeanTouch.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount);
        var isInGesture = (_fingers.Count > 0);
        UpdateUserStopOrStart();
        _stateMachine.Update();
    }

    void UpdateUserStopOrStart()
    {
        if (_fingers.Count == 0)
        {
            if (_hasTouchPreviousFrame)
            {
                //_isThisFrameUserUntouched = true;
                _hasTouchPreviousFrame = false;
                OnUserStopTouch();
            }
        }
        else
        {
            if (!_hasTouchPreviousFrame)
            {
                //_isThisFrameUserTouched = true;
                _hasTouchPreviousFrame = true;
                OnStartUserTouch();
            }
        }
    }

    void OnStartUserTouch()
    {
        if (_stateMachine.CurrentState.State == ValueChangeMode.RestoringToDefaultValue)
        {
            _stateMachine.GoTo(ValueChangeMode.UserProcessing);
        }
    }

    void OnUserStopTouch()
    {
        if (_stateMachine.CurrentState.State == ValueChangeMode.UserProcessing)
        {
            _stateMachine.GoTo(ValueChangeMode.RestoringToDefaultValue);
        }
    }

    public bool IsUserProcessing()
    {
        return _stateMachine.CurrentState.State == ValueChangeMode.UserProcessing;
    }

    public void SwitchToRestoringState()
    {
        _stateMachine.GoTo(ValueChangeMode.RestoringToDefaultValue);
    }

    public void AnimateTo(float value, TweenCallback animationCallback)
    {
        _tweenCallback = animationCallback;
        _animateToValue = value;
        _stateMachine.GoTo(ValueChangeMode.AnimateToValue);
    }


    //states
    // RestoringToDefaultValue
    void OnUpdateRestoringToDefaultValue()
    {
        _currentRot = SmoothDampQuaternion(_currentRot, _target, ref _velocityValueChange, _currentSmoothTime);
        TargetObject.transform.localRotation = _currentRot;
    }
    void OnEnterRestoringToDefaultValue()
    {
        _target = DefaultTargetValue;
        _velocityValueChange = Vector3.zero;
        _currentSmoothTime = RestoreSmoothTime;
    }

    void OnEnterUserProcessing()
    {
        _target = _currentRot;
        _currentSmoothTime = SmoothTime;
    }

    void OnUpdateUserProcessing()
    {
        var fingersDelta = LeanGesture.GetScaledDelta(_fingers);
        _target = _target * Quaternion.Euler(0, fingersDelta.x * _currentSensitivity, 0);
        _currentRot = SmoothDampQuaternion(_currentRot, _target, ref _velocityValueChange, _currentSmoothTime);
        TargetObject.transform.localRotation = _currentRot;
    }

    void OnEnterAnimateToValue()
    {
        _velocityValueChange = Vector3.zero;
        _currentSmoothTime = SmoothTime * 100;
        _target = Quaternion.Euler(0f, _animateToValue, 0f);
    }

    void OnUpdateAnimateToValue()
    {
        _currentRot = SmoothDampQuaternion(_currentRot, _target, ref _velocityValueChange, _currentSmoothTime);
        TargetObject.transform.localRotation = _currentRot;
        if (_tweenCallback != null)
        {
            if (Mathf.Abs(Quaternion.Angle(_currentRot, _target)) < 1f)
            {
                _tweenCallback.Invoke();
                _tweenCallback = null;
            }
        }
    }


    public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
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
