using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;


namespace Game
{
    public class IntentDoRotation : IntentBase
    {
        public Vector3 RelativeRotation;
        public float Duration;
        public Ease Ease;
        public bool PhysicsRotation;
        public TweenCallback OnComplete;
        private Tweener _currentTweener;
   
        public override void DoIntention(GameObject destObject)
        {
            if(PhysicsRotation)
                DoRotationPhysics(destObject);
            else
                DoRotation(destObject);
        }

        private void DoRotationPhysics(GameObject destObject)
        {
            Assert.IsTrue(DestinationObjects.Count == 1, "only one obj supported for now");
            var rb = destObject.GetComponent<Rigidbody>();
            Assert.IsNotNull(rb, "If you performing DoRotationPhysics you must have RigidBody attached");

            if (DOTween.IsTweening(rb))
            {
                Debug.Log("[!]IntentDoRotation.DoRotation call while there is still rotation performing on rigid body since previous call");
                return;
            }

            _currentTweener = rb.DORotate(RelativeRotation, Duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease);
            _currentTweener.SetUpdate(UpdateType.Fixed);
        }

        private void DoRotation(GameObject destObject)
        {
            Assert.IsTrue(DestinationObjects.Count == 1, "only one obj supported for now");

            if (DOTween.IsTweening(destObject.transform))
            {
                Debug.Log("[!]IntentDoRotation.DoRotation call while there is still rotation performing since previous call");
                return;
            }
            _currentTweener = destObject.transform.DOLocalRotate(RelativeRotation, Duration, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease);
        }
    }
}

