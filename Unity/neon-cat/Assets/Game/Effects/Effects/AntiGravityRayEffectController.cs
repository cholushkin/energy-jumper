using UnityEngine;

namespace Game
{
    public class AntiGravityRayEffectController : MonoBehaviour
    {
        public float RayThickness;
        public float RayCurvature;
        public Transform Source;
        public float SourceRadius;
        public float DestinationRadius;

        
        public Renderer Renderer; // ray visual object (origin must be at the bottom)
        public Transform lookTarget;

        public const float AverageStableLengthAverage = 10f;  // length of the ray when object stays in balance with antigravity ray device
        private Vector3 _originalLocalScale;
        private float _distance;
        private float _length;


        void Reset()
        {
            RayThickness = 3f;
        }


        void Awake()
        {
            _originalLocalScale = Renderer.transform.localScale;
        }

        void Update()
        {
            var dir  = lookTarget.position - Source.position;
            _distance = dir.magnitude;
            _length = _distance - SourceRadius - DestinationRadius;
            _length = Mathf.Clamp(_length, 0f, _length);
            
            Renderer.enabled = _length != 0f;
            if (!Renderer.enabled)
                return;

            Renderer.transform.position = Source.transform.position + dir.normalized * SourceRadius;

            Renderer.transform.LookAt(lookTarget, Vector3.back);
            Renderer.transform.localScale = new Vector3(_originalLocalScale.x * RayThickness, _originalLocalScale.y, _originalLocalScale.z * _length);
            Renderer.material.SetVector("Tiling", new Vector4(1, _length / AverageStableLengthAverage * RayCurvature, 0, 0));
        }
    }
}