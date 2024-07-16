using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Level.Entities
{
    // controls jumping using attached RigidBody
    public class ImpulseJumper : MonoBehaviour
    {
        public Rigidbody RBody;
        public float ImpulseCooldown;

        // axis power multipliers
        public float YMultiplier;
        public float XMultiplier;

        public float[] powerLevels;

        //public float powerLevel1 = 0.52f;
        //public float powerLevel2 = 0.78f;
        //public float powerLevel3 = 1f;

        public float MaxPower;
        
        private void Awake()
        {
            Assert.IsTrue(ImpulseCooldown >= 0.0f);
            Assert.IsNotNull(RBody);
        }

        internal void Jump(Vector3 norm, int powerLevel)
        {
            Vector3 imp = CalculateSpeed(norm, powerLevel);
            RBody.AddForce(imp, ForceMode.Impulse);

            if (powerLevel == 0)
            {
                RBody.angularVelocity = Vector3.zero;
                RBody.linearVelocity = Vector3.zero;
            }
        }

        private Vector3 CalculateSpeed(Vector3 norm, int powerLevel)
        {
            var power = MaxPower * powerLevels[powerLevel];
            
            RBody.linearVelocity = Vector3.zero;
            var imp = norm * power;
            imp.x *= XMultiplier;
            imp.y *= YMultiplier;
            return imp;
        }

        private void OnDrawGizmos()
        {
            // --- draw velocity
            var vel = GetComponent<Rigidbody>().linearVelocity;
            if (vel.magnitude > 0.01)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + vel);
            }
        }
    }
}