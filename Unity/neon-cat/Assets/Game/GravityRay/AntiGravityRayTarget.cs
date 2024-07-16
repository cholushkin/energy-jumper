using System;
using UnityEngine;

namespace Game
{
    // Enables gravity, emits events of anti gravity rays on this object
    public class AntiGravityRayTarget : MonoBehaviour
    {
        public bool IsActive; // ready to interact with gravity ray
        public Rigidbody Rigidbody; // self rigid body
        public Node Node; // self node

        public Action<AntiGravityRayTarget> OnActivation;
        public Action<AntiGravityRayTarget> OnEnterSphere;
        public Action<AntiGravityRayTarget> OnExitSphere;

        private int _gravityZonesCounter;
        private bool IsInsideAnyGravityRayDevice => _gravityZonesCounter > 0; // there could be multiple antigravity zones
        private TriggerActivatedByAntiGravityRay _triggerActivatedByAntigravityRay;


        void Awake()
        {
            _triggerActivatedByAntigravityRay = GetComponent<TriggerActivatedByAntiGravityRay>();
        }


        public void Activate(bool flag)
        {
            IsActive = flag;
            if (flag)
            {
                OnActivation?.Invoke(this);
                _triggerActivatedByAntigravityRay?.OnHit();

                // todo: 
                //GlobalEventAggregator.EventAggregator.Publish( new VFXManager.EventRequestVFX("VfxHostageActivation"));
            }
        }

        public void OnEnterAntiGravity()
        {
            _gravityZonesCounter++;
            Rigidbody.useGravity = false;
            OnEnterSphere?.Invoke(this);
        }

        public void OnExitAntiGravity()
        {
            _gravityZonesCounter--;
            if (!IsInsideAnyGravityRayDevice) // still could be in another antigrav zone
            {
                if (IsGravityAffectedObject())
                    Rigidbody.useGravity = true;
            }
            OnExitSphere?.Invoke(this);
        }

        public bool IsGravityAffectedObject()
        {
            return !Node.NodeProps.HasFlag(NodeProps.NoGravity);
        }
    }
}
