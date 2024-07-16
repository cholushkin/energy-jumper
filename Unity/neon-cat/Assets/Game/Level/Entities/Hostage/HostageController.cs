
using DG.Tweening;
using Game.Level.Entities;
using UnityEngine;

namespace Game
{
    [SelectionBase]
    public class HostageController : Node, PortalSuckIn.ISuckingSupport
    {
        public class EventHostageRescued
        {
        }
        public enum State
        {
            InTheWorld,
            SuckingInToPortal
        }

        public AntiGravityRayTarget GravityRayTarget;
        public Rigidbody Rigidbody;
        public HostageVisualController VisualController;
        public float BubbleWaitTime;
        public float RemoveBubbleRadius;
        public string LocalizationTextKey;

        private State _state;
        private HostageBubble _hostageBubble;
        private Vector3 _activatedPosition;
        private bool _isActivated;


        #region Behaviour methods
        void Awake()
        {
            _state = State.InTheWorld;
            GravityRayTarget.OnActivation += Activation;
            GravityRayTarget.OnEnterSphere += EnterSphere;
            GravityRayTarget.OnExitSphere += ExitSphere;

            // create hostage bubble
            _hostageBubble = HostageBubble.Create(transform, LocalizationTextKey);
        }

        void Update()
        {
            if (_isActivated && _hostageBubble.GetWaitTime() == -1f)
            {
                var distance = Vector3.Distance(_activatedPosition, transform.position);
                if (distance > RemoveBubbleRadius)
                {
                    _hostageBubble.SetWaitTime(BubbleWaitTime);
                }
            }
        }

        #endregion


        #region Hostage interface

        public void EnablePhysics(bool flag)
        {
            Rigidbody.detectCollisions = flag;
            Rigidbody.useGravity = flag;
            Rigidbody.linearVelocity = Vector3.zero;
        }
        #endregion


        #region PortalSuckIn.ISuckingSupport
        public void OnStartSuckIn(Transform destination, float portalRadius, TweenCallback onFinishSuckIn)
        {
            _state = State.SuckingInToPortal;
            EnablePhysics(false);

            VisualController.transform.DOMove(destination.position, PortalSuckIn.SuckingDuration).SetEase(Ease.OutElastic);
            VisualController.PlayAnimation( PortalSuckIn.SuckingDuration * 0.5f, () => 
                {
                    onFinishSuckIn.Invoke(); // notify portal that we've done
                    OnEnterPortal(); // do our stuff on enter portal
                });
        }

        public bool IsInSucking()
        {
            return _state == State.SuckingInToPortal;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        private void OnEnterPortal()
        {
            GlobalEventAggregator.EventAggregator.Publish(new EventHostageRescued());
        }

        private void Activation(AntiGravityRayTarget obj)
        {
            _activatedPosition = transform.position;
            _isActivated = true;
            _hostageBubble.Show();
            
            StateGameplay.Instance.SetPause(true);
            StateGameplay.Instance.MainWindowSystem.PushScreen("Screen.BubbleDialog");
            GameCamera.Instance.Focus(transform.position, duration:2f, ease:Ease.OutQuint, unscaledDeltaTime:true); // focus on hostage
        }

        private void ExitSphere(AntiGravityRayTarget obj)
        {
            _hostageBubble.Hide();
        }

        private void EnterSphere(AntiGravityRayTarget obj)
        {
            _hostageBubble.Show();
        }
        #endregion
    }
}
