using GameLib.Alg;
using UnityEngine;

public class ArchimedeanForce: MonoBehaviour
{
    public Rigidbody RigidBody;
    public static void Apply(Rigidbody rb)
    {
        if (rb.gameObject.GetComponent<ArchimedeanForce>() != null)
            return;
        var wf = rb.gameObject.AddComponent<ArchimedeanForce>();
        wf.RigidBody = rb;
    }

    public static void Remove(Rigidbody rb)
    {
        rb.gameObject.RemoveComponent<ArchimedeanForce>(false);
    }

    void FixedUpdate()
    {
        RigidBody.linearVelocity *= 0.95f;
    }
}
