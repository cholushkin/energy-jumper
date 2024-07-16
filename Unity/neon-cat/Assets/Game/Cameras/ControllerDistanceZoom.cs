using System;
using System.Collections.Generic;
using DG.Tweening;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lean.Touch
{
    public class ControllerDistanceZoom : MonoBehaviour
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

        [Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
        public int RequiredFingerCount;

        [Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
        public float Sensitivity = 0f;
        public float RubberZoneSensitivity = 1.0f;

        [Tooltip("Restore smooth time")]
        public float RestoreSmoothTime;

        public LeanScreenDepth ScreenDepth;


        public GameObject TargetObject; // zoom distance

        public Vector3 DefaultTargetValue;
        public float TargetMax;
        public float TargetMin;

        public float SmoothTime;
        private float _currentSmoothTime;
        private Vector3 _target;
        private Vector3 _currentPos;
        private float _currentSensitivity;

        private bool _hasTouchPreviousFrame = false;
        private const float RubberDelta = 0.3f;
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
            RestoreSmoothTime = 7.5f;
        }

        void Start()
        {
            Assert.IsNotNull(TargetObject);
            _currentPos = TargetObject.transform.localPosition;
            _target = TargetObject.transform.localPosition;
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
            _isInRubberZone = _target.z > TargetMax || _target.z < TargetMin;
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
            TargetObject.transform.localPosition = _currentPos;
        }

        void OnEnterRestoringToDefaultValue()
        {
            _target = DefaultTargetValue;
            _velocityValueChange = Vector3.zero;
            _currentSmoothTime = SmoothTime * 1000 * 1.5f / 2f;
        }

        // UserProcessing
        void OnEnterUserProcessing()
        {
            _target = _currentPos;
            _currentSmoothTime = SmoothTime;
        }

        void OnUpdateUserProcessing()
        {
            // Get the world delta of them after conversion
            _currentSensitivity = _isInRubberZone ? RubberZoneSensitivity : Sensitivity;
            var pinchRatio = LeanGesture.GetPinchRatio(_fingers, _currentSensitivity);

            //if (_target.z < TargetMin && worldDelta.y < 0f) // в верхней резиновой зоне но при это двигается вниз - снимаем сопротивление
            //    _currentSensitivity = Sensitivity;
            //if (_target.z > TargetMax && worldDelta.y > 0f) // в нижней резиновой зоне но при это двигается вверх - снимаем сопротивление
            //    _currentSensitivity = Sensitivity;

            _target.z = _target.z * pinchRatio;
            _target.z = Mathf.Clamp(_target.z, TargetMin - RubberDelta, TargetMax + RubberDelta);
            _currentPos = Vector3.SmoothDamp(_currentPos, _target, ref _velocityValueChange, _currentSmoothTime);
            TargetObject.transform.localPosition = _currentPos;
        }

        // RestoringFromRubberZone
        void OnEnterRestoringFromRubberZone()
        {
            //_velocityValueChange = Vector3.zero;
            _currentSmoothTime = SmoothTime;
        }
        void OnUpdateRestoringFromRubberZone()
        {
            _target.z = Mathf.Clamp(_target.z, TargetMin, TargetMax);
            _currentPos = Vector3.SmoothDamp(_currentPos, _target, ref _velocityValueChange, _currentSmoothTime);
            TargetObject.transform.localPosition = _currentPos;
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
            _target.z = _animateToValue;
        }

        void OnUpdateAnimateToValue()
        {
            _currentPos = Vector3.SmoothDamp(_currentPos, _target, ref _velocityValueChange, _currentSmoothTime);
            TargetObject.transform.localPosition = _currentPos;
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
}