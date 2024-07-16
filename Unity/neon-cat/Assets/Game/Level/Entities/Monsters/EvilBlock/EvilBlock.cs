using GameLib.Alg;
using UnityEngine;


namespace Game
{
    public class EvilBlock : MonoBehaviour, CollisionPropagator.ICollisionListener
    {
        public float Damage;

        public void OnCollisionEnter(Collision collision)
        {
            Debug.LogFormat("Evil block '{0}' hit : {1}", transform.GetDebugName(), collision.gameObject);
            //var isHitPlayer = StateGameplay.Instance.GameplayAccessors.Player.gameObject == collision.gameObject;
            //if(isHitPlayer)
            //    DoTargetDamage(StateGameplay.Instance.GameplayAccessors.Player.Health);
        }

        private void DoTargetDamage(Health health)
        {
            //Assert.IsNotNull(health);
            //health.RecieveDamage(Damage);
        }

        public void OnCollisionExit(Collision collisionInfo)
        {
        }
    }
}