using UnityEngine;

public class UnityTriggerTest : MonoBehaviour
{
    public GameObject CubeA; // do nothing: start inside trigger prints "OnTriggerEnter CubeA"
    public GameObject CubeB; // enable object: enable inside trigger prints "OnTriggerEnter CubeB"
    public GameObject CubeC; // disable object: DOESN'T print "OnTriggerExit CubeC" !!!    (also prints  "OnTriggerEnter CubeC" at first)
    public GameObject CubeD; // delete object: DOESN'T print "OnTriggerExit CubeD" !!!
    public GameObject CubeE; // set detectCollisions to false before test and then set it to true in the test. DOESN'T print "OnTriggerEnter CubeD" !!!
    public GameObject CubeF; // disable collider: DOESN'T print "OnTriggerExit CubeF" !!!
    public GameObject CubeG; // enable disabled collider: prints "OnTriggerEnter CubeG"
    public GameObject CubeH; // set detectCollisions to false. DOESN'T print "OnTriggerExit CubeH" !!!
    void Awake()
    {
        // Before test
        CubeE.GetComponent<Rigidbody>().detectCollisions = false;
        Invoke("StartTest", 1f);
    }
    void StartTest()
    {
        CubeB.SetActive(true);
        CubeC.SetActive(false);
        Destroy(CubeD);
        CubeE.GetComponent<Rigidbody>().detectCollisions = true;
        CubeF.GetComponent<Collider>().enabled = false;
        CubeG.GetComponent<Collider>().enabled = true;
        CubeH.GetComponent<Rigidbody>().detectCollisions = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        print($"OnTriggerEnter {collider.name}");
    }

    void OnTriggerExit(Collider collider)
    {
        print($"OnTriggerExit {collider.name}");
    }
}
