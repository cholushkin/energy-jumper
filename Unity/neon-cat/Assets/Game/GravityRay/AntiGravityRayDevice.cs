using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class AntiGravityRayDevice : MonoBehaviour
    {
        public TriggerZoneAnalyzerSphere GravityRayTargetsAnalizer; // gravity ray targets
        public float ActivationRadius; // RayTarget objects need to enter this radius for the first activation
        public AntiGravityRayEffectController PrefabEffectController;

        public float PushStrength = 0.3f;
        public float PullStrength = 0.3f;

        private Dictionary<AntiGravityRayTarget, AntiGravityRayEffectController> _effects;
        private int _activeEffectsCount;

        #region Behaviour methods
        void Awake()
        {
            Assert.IsTrue(ActivationRadius < GravityRayTargetsAnalizer.SphereCollider.radius);
            GravityRayTargetsAnalizer.OnEnter += TargetEnter;
            GravityRayTargetsAnalizer.OnExit += TargetExit;
            _effects = new Dictionary<AntiGravityRayTarget, AntiGravityRayEffectController>(8);
        }

        public void FixedUpdate()
        {
            var items = GravityRayTargetsAnalizer.GetEntries();

            foreach (var target in items)
            {
                var rayTarget = target.Key.GetComponent<AntiGravityRayTarget>();
                Assert.IsNotNull(rayTarget);
                var (normalizedDistance, distance, normal) = GravityRayTargetsAnalizer.GetDistanceTo(target.Value);

                if (!rayTarget.IsActive)
                {
                    if (distance < ActivationRadius)
                    {
                        print("activate");
                        rayTarget.Activate(true);
                        TargetEnterImpl(rayTarget);
                    }
                }

                if (!rayTarget.IsActive)
                    continue;

                var distanceFactor = Mathf.Clamp01(normalizedDistance); // 0 - center; 1 - on the radius
                var reverseDistanceFactor = 1f - distanceFactor; // 1 - center; 0 - on the radius

                // update intentional vector
                rayTarget.Rigidbody.AddForce(InputHandler.Instance.IntentionVector * reverseDistanceFactor * PushStrength * 1.2f, ForceMode.Impulse);


                // update pushing
                if (InputHandler.Instance.IntentionVector.magnitude < 0.99f)
                {
                    if (distanceFactor < 0.4f)
                    {
                        // push out
                        rayTarget.Rigidbody.AddForce(normal * reverseDistanceFactor * PushStrength, ForceMode.Impulse);
                    }
                    else
                    {
                        // pull in
                        rayTarget.Rigidbody.AddForce(normal * distanceFactor * -PullStrength, ForceMode.Impulse);
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow * 0.5f;
            Gizmos.DrawWireSphere(transform.position, ActivationRadius);
            if (Application.isPlaying)
            {
                Gizmos.color = Color.magenta;
                var items = GravityRayTargetsAnalizer.GetEntries();
                foreach (var zoneItem in items)
                    Gizmos.DrawLine(transform.position, zoneItem.Key.transform.position);
            }
        }
        #endregion


        #region GravityRayDevice API and implementation
        public void SetEnabled(bool flag)
        {
            gameObject.SetActive(flag);
        }

        public void SetIgnoreCollision(SphereCollider otherCollider)
        {
            var myCollider = GetComponent<SphereCollider>();
            Assert.IsNotNull(myCollider);
            Physics.IgnoreCollision(myCollider, otherCollider);
        }

        private void TargetEnter(CollisionAnalyzerBase.Entry entry)
        {
            var rayTarget = entry.MainGameObject.GetComponent<AntiGravityRayTarget>();
            Assert.IsNotNull(rayTarget);

            if (rayTarget.IsActive)
                TargetEnterImpl(rayTarget);
        }

        private void TargetEnterImpl(AntiGravityRayTarget rayTarget)
        {
            rayTarget.OnEnterAntiGravity();
            SetAntigravityRayEffect(rayTarget, true);
        }

        private void TargetExit(CollisionAnalyzerBase.Entry entry)
        {
            var rayTarget = entry.MainGameObject.GetComponent<AntiGravityRayTarget>();
            Assert.IsNotNull(rayTarget);
            rayTarget.OnExitAntiGravity();
            SetAntigravityRayEffect(rayTarget, false);
        }

        private void SetAntigravityRayEffect(AntiGravityRayTarget rayTarget, bool flag)
        {
            _activeEffectsCount += flag ? +1 : -1;
            // get cached effect or create new
            AntiGravityRayEffectController effect;
            if (!_effects.TryGetValue(rayTarget, out effect))
            {
                effect = Instantiate(PrefabEffectController, transform);
                effect.transform.localPosition = Vector3.zero;
                effect.lookTarget = rayTarget.transform;
                effect.Source = transform;
                _effects.Add(rayTarget, effect);
            }

            if (flag) // enable
            {
                effect.gameObject.SetActive(true);
            }
            else // disable
            {
                effect.gameObject.SetActive(false);
            }
        }

        public bool HasAnyRay()
        {
            return _activeEffectsCount > 0;
        }
        #endregion
    }
}
