using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTriangle : MonoBehaviour
{
    public enum State
    {
        LookingForTarget, 
        Chasing,
        Attaking,
        Cooldown,
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



//public class Monster1 : MonoBehaviour
//{
//    public PhysicsFollower PsxFollower;
//    public PlayerController PlayerController;
//    public float ActiveRadius;
//    public bool IsAutoTargeting;
//    public bool EnableGravityOutOfZone;

//    void Update()
//    {
//        // ----- get the target if we still don't have
//        if (IsAutoTargeting && PlayerController == null && StateGameplay.Instance != null) // for gameplay state only
//        {
//            PlayerController = StateGameplay.Instance.GameplayAccessors.Player;
//            if (PlayerController != null)
//                PsxFollower.Target = PlayerController.transform;
//        }

//        // ----- activation - deactivation
//        if (PlayerController != null)
//        {
//            var isInActiveZone = (PlayerController.transform.position - transform.position).magnitude < ActiveRadius;
//            if (isInActiveZone)
//            {
//                PsxFollower.Target = PlayerController.transform;
//                if (EnableGravityOutOfZone)
//                    GetComponent<Rigidbody>().useGravity = false;
//            }
//            else
//            {
//                PsxFollower.Target = null;
//                if (EnableGravityOutOfZone)
//                    GetComponent<Rigidbody>().useGravity = true;
//            }
//        }
//    }

//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, ActiveRadius);
//    }
//}
