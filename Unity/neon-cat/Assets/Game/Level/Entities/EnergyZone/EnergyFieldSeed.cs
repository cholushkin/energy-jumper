using GameLib.Random;
using UnityEngine;

[SelectionBase]
public class EnergyFieldSeed : MonoBehaviour
{
    public int Seed;

    void Awake()
    {
        IPseudoRandomNumberGenerator rnd = RandomHelper.CreateRandomNumberGenerator(Seed);
        var newRenderer = gameObject.GetComponent<Renderer>();
        var propBlock = new MaterialPropertyBlock();
        newRenderer.GetPropertyBlock(propBlock);
        propBlock.SetInt("Seed", rnd.ValueInt() % 10000);
        newRenderer.SetPropertyBlock(propBlock);
    }
}
