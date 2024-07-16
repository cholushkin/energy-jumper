using GameLib;
using UnityEngine;

namespace Game
{
    public class Roll : MonoBehaviour
    {
        public float torque;
        public Rigidbody rigidBody;
        public ForceImpact ForceImpact;
        public PlayerController PlayerController;

        void FixedUpdate()
        {
            // input vector 
            var input = InputHandler.Instance.MoveVector;
            var normInput = input.normalized;
            var maxx = normInput.x; // projection of x component to horizontal axis (maximum value with a sign for current direction)
            var maxy = normInput.y;
            var circledInput =
                new Vector2(maxx * Mathf.Abs(input.x),
                    maxy * Mathf.Abs(input.y)); // Mathf.Abs() here is percent of propagation on current axis

            // get normalized gravity vector
            var gravity = Physics.gravity;
            var additionalPLayerForce = InputHandler.Instance.MoveVector.ToVector3(0f) * 2f;
            //var normalizedGravity = gravity.magnitude < 0.1f ? Vector3.down : gravity.normalized;
            ForceImpact.AddForce(gravity + additionalPLayerForce, ForceMode.Acceleration);

            rigidBody.AddForce(additionalPLayerForce, ForceMode.Acceleration);

            // get normalized force impact to the player
            var forceImpactNormalized = ForceImpact.GetForceImpact().normalized;

            // right direction
            var right = Vector3.Cross(Vector3.forward, forceImpactNormalized);

            // power 
            var powerForceVector = Vector3.Project(circledInput, right);
            var sign = Vector3.Dot(right, powerForceVector) > 0 ? 1f : -1f;
            var power = powerForceVector.magnitude;

            torque = 0.5f;
            rigidBody.AddTorque(-Vector3.forward * sign * power, ForceMode.Impulse);
        }
    }
}