using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityCollisionTest : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        print($"OnCollisionEnter {gameObject.name}");
    }

    void OnCollisionExit(Collision collision)
    {
        print($"OnCollisionExit {gameObject.name}");
    }
}
