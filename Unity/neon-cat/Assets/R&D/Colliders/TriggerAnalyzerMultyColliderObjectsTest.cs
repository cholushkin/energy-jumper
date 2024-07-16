using UnityEngine;

public class TriggerAnalyzerMultyColliderObjectsTest : MonoBehaviour
{
    public TriggerZoneAnalyzer analizer;
    
    void Awake()
    {
        analizer.OnEnter += OnZoneEnter;
        analizer.OnExit += OnZoneExit;
    }

    public void OnZoneEnter(CollisionAnalyzerBase.Entry zoneEntry)
    {
        print($"OnZoneEnter {zoneEntry.Node.name}");

    }

    public void OnZoneExit(CollisionAnalyzerBase.Entry zoneEntry)
    {
        print($"OnZoneExit {zoneEntry.Node.name}");
    }
}
