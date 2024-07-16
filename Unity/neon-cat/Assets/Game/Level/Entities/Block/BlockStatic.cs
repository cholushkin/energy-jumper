using GameLib.Log;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    // Block manages:
    // * trigger touch of block (to use as a button)
    // * plays sway animation
    // * handles energy of impact

    public class BlockStatic
        : Node
        , CollisionPropagator.ICollisionListener
        , Affects.IPhysicalImpactHandler
    {
        public Range EnergyFilterRange = new Range(80f, 600f);

        public BlockVisual Visual;

        public Fractured Fractured;
        public TriggerOnBlockPress OnBlockPressTrigger;
        public LogChecker LogChecker;


        [Tooltip("Snap block to move aligned to axis. And accept only impacts of the same direction")]
        public SnapAxis SnapAxis;

        [Tooltip("Snap block to move only in positive direction of an axis")]
        public bool OnlyPositiveAxisSnap;

        [Tooltip("For buttons on the floor which we press by rolling to them. Good for a very flat on floor buttons or buttons which player can activate from the side")]
        public bool TreatNegativeImpactAsPositive;

        [Range(0f,180f)]
        [Tooltip("Max angle degree block is accepting when snapping is enabled")]
        public float DiagonalTolerance;

        private float dtPrevHit;
        private float hitTime = float.NegativeInfinity;
        protected Rigidbody _rigidbody;


        public virtual void Reset()
        {
            NodeType = NodeType.BlockStatic;
            NodeProps = NodeProps.NoProps;
        }

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected void ProcessCollision(Collision collision)
        {
            var theirsRigidBody = collision.rigidbody;

            var relativeSpeed = collision.relativeVelocity;
            var relativeAngularVelocity = theirsRigidBody == null ? Vector3.zero : theirsRigidBody.angularVelocity;
            if (_rigidbody != null)
                relativeAngularVelocity += _rigidbody.angularVelocity;

            var hitEnery = HitEnergy(relativeSpeed, relativeAngularVelocity);

            if (hitEnery < EnergyFilterRange.From)
            {
                if (LogChecker.Normal())
                    Debug.LogFormat($"Filtered impact {name}. Rel speed:{relativeSpeed.magnitude}; Rel angular vel:{relativeAngularVelocity.magnitude}; Hit Energy:{hitEnery}. No impact!");
                return; // no impact
            }

            hitEnery = Mathf.Clamp(hitEnery, EnergyFilterRange.From, EnergyFilterRange.To);

            // hit time
            dtPrevHit = Time.time - hitTime;
            hitTime = Time.time;

            if (LogChecker.Normal())
                Debug.LogFormat($"Impact: {name} <=> {collision.gameObject.name}. Rel speed:{relativeSpeed.magnitude}; Rel angular vel:{relativeAngularVelocity.magnitude}; Hit Energy:{hitEnery}; PrevHit dt:{dtPrevHit}");
            if (dtPrevHit < 0.1f)
            {
                if (LogChecker.Normal())
                    Debug.Log("Ignoring impact due to low prev hit dt. No impact!");
                return; // no impact
            }

            var relativeVelocityNormalized = collision.relativeVelocity.normalized;
            var v3 = relativeVelocityNormalized;
            if (SnapAxis != SnapAxis.None)
            {
                v3 = AxisToV3(SnapAxis, Visual.transform);
                var dot = Vector3.Dot(v3, relativeVelocityNormalized);
                var sign = dot > 0f ? 1f : -1f;
                if (LogChecker.Verbose())
                    Debug.Log($"snap: relVelNorm:{relativeVelocityNormalized} dot:{dot}");
                v3 *= sign;

                var hitAngle = Mathf.Rad2Deg * Mathf.Acos(dot);
                if (LogChecker.Verbose())
                    Debug.Log($"Hit angle: {hitAngle} degrees.");
                if (hitAngle > DiagonalTolerance) // diagonal hit on snapping block
                {
                    if (LogChecker.Verbose())
                        Debug.Log($"Diagonal intolerant hit. Max angle allowed:{DiagonalTolerance}. No impact!");
                    return; // no impact
                }

                if (OnlyPositiveAxisSnap && sign < 0f) // only impact on positive direction of snapped axis
                {
                    if (LogChecker.Verbose())
                        Debug.Log("Negative axis impact (pull from opposite side). No impact!");
                    if (TreatNegativeImpactAsPositive)
                        v3 *= sign;
                    else
                        return; // no impact
                }
            }

            Affects.PhysicalImpact(gameObject, collision.gameObject, dtPrevHit, hitEnery, v3);
        }

        public virtual void PhysicalImpact(GameObject otherGameObject, float prevImpactDeltaTime, float hitEnery, Vector3 relativeVelocityNormalized)
        {
            Assert.IsNull(_rigidbody, "For BlockStatic _rigidbody must be always null. It is used only in inherited classes");
            OnBlockPressTrigger?.OnHit();
            Fractured?.Hit(otherGameObject, relativeVelocityNormalized, hitEnery);
            if (Fractured != null && Fractured.IsExploded)
                return;

            Visual.StartBlockPushAnimation(Visual.transform, relativeVelocityNormalized);
        }


        #region ICollisionListener
        public void OnCollisionEnter(Collision collision)
        {
            ProcessCollision(collision);
        }

        public void OnCollisionExit(Collision collisionInfo)
        {
        }
        #endregion

        void CollisionPropagator.ICollisionListener.OnCollisionEnter(Collision collision)
        {
            OnCollisionEnter(collision);
        }

        protected static float HitEnergy(Vector3 relativeVel, Vector3 relAngularVel)
        {
            // mass in kg, velocity in meters per second, result is joules
            // no mass for easy balancing (rb.mass)
            float E = relativeVel.sqrMagnitude * 0.5f;
            E += relAngularVel.sqrMagnitude * 0.5f;
            return E;
        }

        private Vector3 AxisToV3(SnapAxis axis, Transform block)
        {
            if (SnapAxis == SnapAxis.X)
                return block.right;
            if (SnapAxis == SnapAxis.Y)
                return block.up;
            if (SnapAxis == SnapAxis.Z)
                return block.forward;
            return Vector3.zero;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}