using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class IntentDoMove : IntentBase
    {
        public Vector3 MoveOffset;
        public float Duration;
        public Ease Ease;
        public AnimationCurve Curve;
        public bool PhysicsMove;
        public LoopType LoopType = LoopType.Incremental;
        public int Loops = 1;

        public override void DoIntention(GameObject destObject)
        {
            Tweener tweener;
            if (PhysicsMove)
            {
                tweener = destObject.GetComponent<Rigidbody>()
                    .DOMove(destObject.transform.position + MoveOffset, Duration)
                    .SetUpdate(UpdateType.Fixed);
            }
            else
                tweener = destObject.transform.DOMove(destObject.transform.position + MoveOffset, Duration);

            if (Ease == Ease.Unset)
                tweener.SetEase(Curve);
            else
                tweener.SetEase(Ease);

            tweener.SetLoops(Loops, LoopType);
        }

        void OnDrawGizmos()
        {
            foreach (var destinationObject in DestinationObjects)
            {
                Gizmos.DrawCube(destinationObject.transform.position+MoveOffset, Vector3.one * 0.5f);
            }
        }
    }
}
