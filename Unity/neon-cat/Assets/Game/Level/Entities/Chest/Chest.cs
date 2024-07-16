using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum ChestValue
    {
        Low,
        Medium,
        High
    }

    public ChestValue Value;

    public GameObject[] PrefabsContent;

    public void OnBreak()
    {
        SpawnGoods();
    }

    private void SpawnGoods()
    {
        
    }
}
