using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReliableColliderTest : ReliableOnTriggerExit
{
    public GameObject CubeA; // do nothing: start inside trigger prints "OnTriggerEnter CubeA"
    public GameObject CubeB; // enable object: enable inside trigger prints "OnTriggerEnter CubeB"
    public GameObject CubeC; // disable object: DOESN'T print "OnTriggerExit CubeC"
    public GameObject CubeD; // delete object: DOESN'T print "OnTriggerExit CubeD"
    public GameObject CubeE; // set detectCollisions to false before test and then set it to true in the test. DOESN'T print "OnTriggerEnter CubeD"
    public GameObject CubeF; // disable collider: DOESN'T print "OnTriggerExit CubeF"
    

    void Awake()
    {
        // Before test
        CubeE.GetComponent<Rigidbody>().detectCollisions = false;
        Invoke("StartTest", 1f);
    }


    void FixedUpdate()
    {

    }

    void StartTest()
    {
        CubeB.SetActive(true);
        CubeC.SetActive(false);
        Destroy(CubeD);
        CubeE.GetComponent<Rigidbody>().detectCollisions = true;
        CubeF.GetComponent<Collider>().enabled = false;
        
    }

    void OnTriggerEnter(Collider collider)
    {
        ReliableOnTriggerExit.NotifyTriggerEnter(collider, gameObject, OnTriggerExit);
        print($"OnTriggerEnter {collider.name}");
    }

    void OnTriggerExit(Collider collider)
    {
        ReliableOnTriggerExit.NotifyTriggerExit(collider, gameObject);
        print($"OnTriggerExit {collider.name}");
    }
}
