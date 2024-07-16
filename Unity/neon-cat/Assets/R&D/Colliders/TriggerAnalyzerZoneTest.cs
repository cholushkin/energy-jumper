using UnityEngine;

public class TriggerAnalyzerZoneTest : MonoBehaviour
{
    public GameObject CubeA; // do nothing: start inside trigger prints "OnTriggerEnter CubeA"
    public GameObject CubeB; // enable object: enable inside trigger prints "OnTriggerEnter CubeB"
    public GameObject CubeC; // disable object: prints "OnTriggerExit CubeC" (also prints  "OnTriggerEnter CubeC" at first)
    public GameObject CubeD; // delete object: prints "OnTriggerExit 'name unavailable due to destroyed object'" 
    public GameObject CubeE; // set detectCollisions to false before test and then set it to true in the test. Prints "OnTriggerEnter CubeE"
    public GameObject CubeF; // disable collider: prints "OnTriggerExit CubeF" 
    public GameObject CubeG; // enable disabled collider: prints "OnTriggerEnter CubeG"
    public GameObject CubeH; // set detectCollisions to false. prints "OnTriggerExit CubeH" 

    public TriggerZoneAnalyzer TriggerZoneAnalyzer;

    public int Counter;

    void Awake()
    {
        // Before test
        TriggerZoneAnalyzer.OnEnter += OnZoneEnter;
        TriggerZoneAnalyzer.OnExit += OnZoneExit;
        CubeE.GetComponent<Rigidbody>().detectCollisions = false;
        Invoke("StartTest", 1f);
    }

    void OnDestroy()
    {
        TriggerZoneAnalyzer.OnEnter -= OnZoneEnter;
        TriggerZoneAnalyzer.OnExit -= OnZoneExit;
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

    void OnZoneEnter(CollisionAnalyzerBase.Entry zoneEntry)
    {
        Counter++;
        print($"OnTriggerEnter {zoneEntry.MainGameObject.name}");
    }

    void OnZoneExit(CollisionAnalyzerBase.Entry zoneEntry)
    {
        Counter--;
        var objName = zoneEntry.MainGameObject == null ? "'name unavailable due to destroyed object'" : zoneEntry.MainGameObject.name;
        print($"OnTriggerExit {objName}");
    }
}
