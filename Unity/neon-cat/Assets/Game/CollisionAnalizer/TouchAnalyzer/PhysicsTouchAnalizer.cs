using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Assertions;


// same as ZoneTriggerAnalizer but for RigidBody instead of trigger
public class PhysicsTouchAnalizer : CollisionAnalyzerBase
{
    public bool AnalizeCrushPenetration;
    private List<ContactPoint> _contacts;
    public float MaxPenetration;
    public Action<GameObject> OnPenetrated;

    public override void Awake()
    {
        base.Awake();
        if (AnalizeCrushPenetration)
            _contacts = new List<ContactPoint>(64);
    }

    public override (float normalizedDistance, float distance, Vector3 normal) GetDistanceTo(Entry item)
    {
        return (0f, 0f, Vector3.zero);
    }

    void OnCollisionEnter(Collision collision)
    {
        var otherCollider = collision.collider;
        var mainGameObject = otherCollider.gameObject.GetMainGameObject();
        var rigidBody = mainGameObject.GetComponent<Rigidbody>(); // could be null

        
        var node = mainGameObject.GetComponent<Node>(); // could be null
        CollisionEnter(mainGameObject, node, rigidBody, otherCollider);
    }

    void OnCollisionStay(Collision collision)
    {
        var otherCollider = collision.collider;
        var mainGameObject = otherCollider.gameObject.GetMainGameObject();
        var rigidBody = mainGameObject.GetComponent<Rigidbody>();
        var node = mainGameObject.GetComponent<Node>(); // could be null

        // analyze contact separation
        if (AnalizeCrushPenetration)
        {
            int points = collision.GetContacts(_contacts);
            for (int i = 0; i < points; i++)
            {
                if (Mathf.Abs(_contacts[i].separation) > MaxPenetration)
                {
                    Debug.Log($"Death penetration: {_contacts[i].separation}");
                    OnPenetrated?.Invoke(collision.gameObject);
                }
            }
        }

        CollisionStay(mainGameObject, node, rigidBody, otherCollider);
    }


    void OnCollisionExit(Collision collision)
    {
        var otherCollider = collision.collider;
        var mainGameObject = otherCollider.gameObject.GetMainGameObject();
        Assert.IsNotNull(mainGameObject);
        var node = mainGameObject.GetComponent<Node>(); // optional
        CollisionExit(mainGameObject, otherCollider);
    }
}