using UnityEngine;
using UnityEngine.Assertions;

public class IntentSpawnItem : IntentBase
{
    public GameObject ItemToSpawn;

    public override void DoIntention(GameObject destObject)
    {
        base.Apply();
        //var currentPattern = gameObject.GetComponent<Node>().GetMyPattern();
        //Assert.IsNotNull(currentPattern);
        Assert.IsNotNull(ItemToSpawn);
        Debug.LogFormat("Spawn item {0}", ItemToSpawn.name);
        //currentPattern.SpawnItem(ItemToSpawn);
    }
}
