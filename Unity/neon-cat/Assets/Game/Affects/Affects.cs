using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Affects
{
    public interface IChargingHandler
    {
        bool IsCharged();
        void Charge(bool flag);
    }

    public interface IAntiGravityHandler
    {
        bool IsAntiGravityEnabled();
        void EnableAntigravity(bool flag);
    }

    public interface IPhysicalImpactHandler
    {
        void PhysicalImpact(
            GameObject otherGameObject, 
            float prevImpactDeltaTime, 
            float hitEnery, 
            Vector3 relativeVelocityNormalized);
    }

    // charge / discharge object
    public static void Charge(GameObject target, bool flag)
    {
    }
    
    // on / off the gravity for the target object
    public static void EnableAntigravity(GameObject target, bool flag)
    {
    }

    // physical impact to object
    public static void PhysicalImpact(
        GameObject gameObject, 
        GameObject otherGameObject, 
        float prevImpactDeltaTime, 
        float hitEnery,
        Vector3 relativeVelocityNormalized)
    {
        gameObject.GetComponent<IPhysicalImpactHandler>().PhysicalImpact(otherGameObject, prevImpactDeltaTime, hitEnery, relativeVelocityNormalized);
    }

    public static void ExplosionImpact(GameObject gameObject, float enery, Vector3 direction)
    {

    }

}
