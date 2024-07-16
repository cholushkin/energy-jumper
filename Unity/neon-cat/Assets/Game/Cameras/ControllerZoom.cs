using Lean.Touch;
using UnityEngine;

namespace Game
{
    public class ControllerZoom : MonoBehaviour
    {
        [Tooltip("The camera that will be zoomed (None = MainCamera)")]
        public Camera Camera;

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreStartedOverGui = true;

        [Tooltip("Ignore fingers with IsOverGui?")]
        public bool IgnoreIsOverGui;

        [Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
        public int RequiredFingerCount;

        [Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
        [Range(-1.0f, 1.0f)]
        public float WheelSensitivity;


        [Tooltip("The current FOV/Size")]
        public float Zoom = 50.0f;

        [Tooltip("Limit the FOV/Size?")]
        public bool ZoomClamp;

        [Tooltip("The minimum FOV/Size we want to zoom to")]
        public float ZoomMin = 10.0f;

        [Tooltip("The maximum FOV/Size we want to zoom to")]
        public float ZoomMax = 60.0f;


        public float ZoomSmoothTime;
        public float RubberThreshold;
        private float _zoomCurrent;
        private float _currentZoomVelocity = 0f;

        private float _zoomSmoothTime;
        private bool _hasTouchPreviousFrame = false;
        private float _defaultZoom;


        void Start()
        {
            _zoomCurrent = Zoom;
            _defaultZoom = Zoom;
        }

        protected virtual void LateUpdate()
        {
            // Get the fingers we want to use
            var fingers = LeanSelectable.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount);
            var isRelaxing = (fingers.Count == 0);
            if (fingers.Count == 0)
            {
                if (_hasTouchPreviousFrame)
                {
                    OnUserStopTouch();
                    _hasTouchPreviousFrame = false;
                }
            }
            else
            {
                if (!_hasTouchPreviousFrame)
                {
                    OnStartUserTouch();
                    _hasTouchPreviousFrame = true;
                }
            }

            // Get the pinch ratio of these fingers
            var pinchRatio = LeanGesture.GetPinchRatio(fingers, WheelSensitivity);


            // Modify zoom value
            Zoom *= pinchRatio;

            if (fingers.Count == 0 && Zoom > ZoomMax - RubberThreshold)
            {
                Zoom = ZoomMax - RubberThreshold;
            }

            if (fingers.Count == 0 && Zoom < ZoomMin + RubberThreshold)
            {
                Zoom = ZoomMin + RubberThreshold;
            }

            if (ZoomClamp)
            {
                Zoom = Mathf.Clamp(Zoom, ZoomMin, ZoomMax);
            }

            _zoomCurrent = Mathf.SmoothDamp(_zoomCurrent, Zoom, ref _currentZoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);

            SetZoomToCamera(_zoomCurrent);
        }

        void OnStartUserTouch()
        {
            Zoom = _zoomCurrent;
            _zoomSmoothTime = ZoomSmoothTime;
        }

        void OnUserStopTouch()
        {
            Zoom = _defaultZoom; 
            _zoomSmoothTime = ZoomSmoothTime * 100;
            _currentZoomVelocity = 0f;
        }


        protected void SetZoomToCamera(float current)
        {
            // Make sure the camera exists
            var camera = LeanTouch.GetCamera(Camera, gameObject);

            if (camera != null)
            {
                if (camera.orthographic == true)
                {
                    camera.orthographicSize = current;
                }
                else
                {
                    camera.fieldOfView = current;
                }
            }
            else
            {
                Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
            }
        }
    }
}