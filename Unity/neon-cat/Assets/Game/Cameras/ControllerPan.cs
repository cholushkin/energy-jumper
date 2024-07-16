using System;
using System.Collections.Generic;
using DG.Tweening;
using GameLib;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Assertions;

public class ControllerPan : MonoBehaviour
{
    enum ValueChangeMode
    {
        RestoringFromRubberZone,
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

    public float RubberZoneSensitivity = 1.0f;

    public LeanScreenDepth ScreenDepth;

    public GameObject TargetObject;
    public Vector3 DefaultTargetValue;
    public float TargetMax;
    public float TargetMin;

    public float SmoothTime;

    public float RestoreSmoothTime;


    private float _currentSmoothTime;
    private Vector3 _target;
    private Vector3 _currentPos;
    private float _currentSensitivity;

    private bool _hasTouchPreviousFrame = false;
    private const float RubberDelta = 0.25f;
    private StateMachine<ValueChangeMode> _stateMachine;


    //private bool _isThisFrameUserTouched = false;
    //private bool _isThisFrameUserUntouched = false;
    private bool _isInRubberZone = false;
    private List<LeanFinger> _fingers;
    private float _animateToValue;

    //private Vector3 _currentWorldDelta;
    private Vector3 _velocityValueChange = Vector3.zero;
    private TweenCallback _tweenCallback;


    void Reset()
    {
        RestoreSmoothTime = 7f;
    }

    void Start()
    {
        Assert.IsNotNull(TargetObject);
        _currentPos = TargetObject.transform.position;
        _target = TargetObject.transform.position;
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
        _isInRubberZone = _target.y > TargetMax || _target.y < TargetMin;
        UpdateUserStopOrStart();
        _stateMachine.Update();
    }

    public void AnimateTo(float value, TweenCallback animationCallback)
    {
        _tweenCallback = animationCallback;
        _animateToValue = value;
        _stateMachine.GoTo(ValueChangeMode.AnimateToValue);
    }

    public void SwitchToRestoringState()
    {
        _stateMachine.GoTo(ValueChangeMode.RestoringToDefaultValue);
    }

    public bool IsUserProcessing()
    {
        return _stateMachine.CurrentState.State == ValueChangeMode.UserProcessing;
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
        if (_stateMachine.CurrentState.State == ValueChangeMode.RestoringToDefaultValue || _stateMachine.CurrentState.State == ValueChangeMode.RestoringFromRubberZone)
        {
            _stateMachine.GoTo(ValueChangeMode.UserProcessing);
        }
        //Zoom = _zoomCurrent;
        //_zoomSmoothTime = ZoomSmoothTime;
    }

    void OnUserStopTouch()
    {
        if (_stateMachine.CurrentState.State == ValueChangeMode.UserProcessing && _isInRubberZone)
        {
            _stateMachine.GoTo(ValueChangeMode.RestoringFromRubberZone);
            return;

        }

        if (_stateMachine.CurrentState.State == ValueChangeMode.UserProcessing && !_isInRubberZone)
        {

            _stateMachine.GoTo(ValueChangeMode.RestoringToDefaultValue);
            return;
        }
    }

    //states
    // RestoringToDefaultValue
    void OnUpdateRestoringToDefaultValue()
    {
        _currentPos = Vector3.SmoothDamp(_currentPos, _target, ref _velocityValueChange, _currentSmoothTime);
        TargetObject.transform.position = _currentPos;
    }

    void OnEnterRestoringToDefaultValue()
    {
        _target = DefaultTargetValue;
        _velocityValueChange = Vector3.zero;
        _currentSmoothTime = RestoreSmoothTime;
    }

    // UserProcessing
    void OnEnterUserProcessing()
    {
        _target = _currentPos;
        _currentSmoothTime = SmoothTime;
    }

    void OnUpdateUserProcessing()
    {
        var lastScreenPoint = LeanGesture.GetLastScreenCenter(_fingers);
        var screenPoint = LeanGesture.GetScreenCenter(_fingers);

        // Get the world delta of them after conversion
        var worldDelta = ScreenDepth.ConvertDelta(lastScreenPoint, screenPoint, gameObject);

        _currentSensitivity = _isInRubberZone ? RubberZoneSensitivity : Sensitivity;

        if (_target.y < TargetMin && worldDelta.y < 0f) // в верхней резиновой зоне но при это двигается вниз - снимаем сопротивление
            _currentSensitivity = Sensitivity;
        if (_target.y > TargetMax && worldDelta.y > 0f) // в нижней резиновой зоне но при это двигается вверх - снимаем сопротивление
            _currentSensitivity = Sensitivity;


        _target = TargetObject.transform.position + Vector3.up * worldDelta.y * _currentSensitivity;
        _target.y = Mathf.Clamp(_target.y, TargetMin - RubberDelta, TargetMax + RubberDelta);
        _currentPos = Vector3.SmoothDamp(_currentPos, _target, ref _velocityValueChange, _currentSmoothTime);
        TargetObject.transform.position = _currentPos;
    }

    // RestoringFromRubberZone
    void OnEnterRestoringFromRubberZone()
    {
        //_velocityValueChange = Vector3.zero;
        _currentSmoothTime = SmoothTime;
    }
    void OnUpdateRestoringFromRubberZone()
    {
        _target.y = Mathf.Clamp(_target.y, TargetMin, TargetMax);
        _currentPos = Vector3.SmoothDamp(_currentPos, _target, ref _velocityValueChange, _currentSmoothTime);
        TargetObject.transform.position = _currentPos;
        if (Math.Abs(_target.y - _currentPos.y) < 0.01f)
        {
            _stateMachine.GoTo(ValueChangeMode.RestoringToDefaultValue);
        }
    }

    // AnimateToValue
    void OnEnterAnimateToValue()
    {
        _velocityValueChange = Vector3.zero;
        _currentSmoothTime = SmoothTime * 100;
        _target.y = _animateToValue;
    }

    void OnUpdateAnimateToValue()
    {
        _currentPos = Vector3.SmoothDamp(_currentPos, _target, ref _velocityValueChange, _currentSmoothTime);
        TargetObject.transform.position = _currentPos;
        if (_tweenCallback != null)
        {
            if (Vector3.Distance(_currentPos, _target) < 0.1f)
            {
                _tweenCallback.Invoke();
                _tweenCallback = null;
            }
        }
    }
}
