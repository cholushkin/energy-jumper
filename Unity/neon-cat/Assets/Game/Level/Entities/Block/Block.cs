using GameLib.Alg;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    // Represents Free block. WHY do you need to use block?:
    // * Trigger touch of block (to use as a button or trigger)
    // * Handles energy of impact (could be used for fractured blocks)
    // Free block doesn't  have animation
    public class Block : BlockStatic
    {
        public override void Reset()
        {
            base.Reset();
            NodeType = NodeType.BlockFree;
            NodeProps = NodeProps.NoProps;
        }

        public override void PhysicalImpact(GameObject otherGameObject, float prevImpactDeltaTime, float hitEnery, Vector3 relativeVelocityNormalized)
        {
            Assert.IsNotNull(_rigidbody, $"For Block there must be _rigidbody {transform.GetDebugName()}");
            OnBlockPressTrigger?.OnHit();
            Fractured?.Hit(otherGameObject, relativeVelocityNormalized, hitEnery);
        }
    }
}
