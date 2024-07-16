using UnityEngine;


namespace Game.Level.Entities
{

    public class DangerZone : MonoBehaviour
    {
        public Follower Follower;
        public Transform Target;
        public ForceField ForceField;
        public bool IsAutoTargeting;
        public float DamageEnergy;
        public float DamageCooldown;
        public float CurrentDamageCooldown;

        public AnimationCurve DamageDistributionToCenter;

        //void Start()
        //{
        //    if( StateGameplay.Instance != null )
        //        Target = StateGameplay.Instance.Player.transform;
        //    Follower.Follow(Target);
        //    //ForceField.SetInteractiveLayers(new[] { "Player", "Pickup", "Derbis" });
        //}

        //void Update()
        //{
        //    if ((Target.transform.position - transform.position).magnitude < ForceField.Radius)
        //    {
        //        CurrentDamageCooldown -= Time.deltaTime;
        //        if (CurrentDamageCooldown <= 0f)
        //        {
        //            CurrentDamageCooldown = DamageCooldown;
        //            DoTargetDamage();
        //        }
        //    }
        //}

        //private void DoTargetDamage()
        //{
        //    var health = Target.GetComponent<Health>();
        //    if(health == null)
        //        return;

        //    var f = 1f - Mathf.Clamp01((Target.transform.position - transform.position).magnitude / ForceField.Radius);
        //    var e = DamageDistributionToCenter.Evaluate(f);
        //    health.DoDamage(DamageEnergy * e);
        //}
    }
}

