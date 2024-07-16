using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;


namespace Game.Level.Entities
{
    [SelectionBase]
    public abstract class PortalControllerBase : Node
    {
        public class EventPortalSpawned // Spawned when portal entity spawned
        {
            public PortalControllerBase Portal;
        }

        public enum State
        {
            Opening,
            Closing,
            Working, // spawning item or sucking in item
            Closed // disabled and inactive state
        }

        public ForceField ForceField;
        public PortalVisualController VisualController;
        public int WorksMax; // -1 is infinite 
        public int WorksCounter;

        public State CurrentState { get; protected set; }


        public virtual void Awake()
        {
            // opening state
            CurrentState = State.Opening;
            ForceField.SetEnabled(false);
            StartCoroutine(OpeningCoroutine());
            GlobalEventAggregator.EventAggregator.Publish(new EventPortalSpawned { Portal = this });
            //transform.DOMove(transform.position + Vector3.forward * 10, 2.0f).From();
        }

        protected void ProcessWorkingCounter()
        {
            if (WorksMax == -1) // infinite
                return;

            ++WorksCounter;
            if (WorksCounter >= WorksMax)
            {
                Assert.IsTrue(CurrentState == State.Working);
                CurrentState = State.Closing;
                StartCoroutine(ClosingCoroutine());
            }
        }

        protected IEnumerator OpeningCoroutine()
        {
            // play portal spawning animation
            Assert.IsTrue(CurrentState == State.Opening);
            var duration = VisualController.PlayAppearingAnimation();
            yield return new WaitForSeconds(duration);

            // working state
            CurrentState = State.Working;
            ForceField.SetEnabled(true);
            
            yield return null;
        }

        protected IEnumerator ClosingCoroutine()
        {
            ForceField.SetEnabled(false);
            var duration = VisualController.PlayDisappearingAnimation();
            yield return new WaitForSeconds(duration);

            CurrentState = State.Closed;
            KillPortal();
            yield return null;
        }

        private void KillPortal()
        {
            Destroy(gameObject);
        }

        protected float GetPortalRadius()
        {
            var sphereZoneAnalizer = ForceField.ZoneAnalyzer as TriggerZoneAnalyzerSphere;
            Assert.IsNotNull(sphereZoneAnalizer, "portal must be spherical");
            return sphereZoneAnalizer.SphereCollider.radius;
        }
    }
}
