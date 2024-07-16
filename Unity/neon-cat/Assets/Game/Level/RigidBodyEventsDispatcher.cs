using Game;
using UnityEngine;

public class RigidBodyEventsDispatcher : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            var blockStaticAttached = contact.thisCollider.gameObject.GetComponent<BlockStatic>();
            if (blockStaticAttached != null)
            {
                blockStaticAttached.OnCollisionEnter(collision);
            }
        }
    }
}
