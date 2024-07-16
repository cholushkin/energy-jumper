using GameLib.Random;
using TowerGenerator;
using UnityEngine;

public class ChunkRandomization : MonoBehaviour
{
    public bool SupportsMirroring; // mirror vertically
    public bool SupportsShrink; // make 2 times smaller

    public StablePosition[] StablePositions { get; set; }
    public DoIf[] DoIfs { get; set; }

    public ChunkControllerBase Chunk;

    public bool IsRotated { get; set; }
    public bool IsShrinked { get; set; }

    [ContextMenu("Randomizes")]
    public void Randomize(IPseudoRandomNumberGenerator rnd)
    {
        if (SupportsMirroring)
        {
            // do flip
        }

        if (SupportsShrink)
        {
            if (rnd.TrySpawnEvent(0.2f))
            {
                Shrink(0.6f);
            }
        }

        if (Chunk.Seed == -1)
            Chunk.Seed = rnd.ValueInt();

        Chunk.Init();
        Chunk.SetConfiguration();
        StablePositions = GetComponentsInChildren<StablePosition>(includeInactive:true);
        DoIfs = GetComponentsInChildren<DoIf>(includeInactive: true);

        foreach (var doIf in DoIfs)
            doIf.DoConditionally();
    }

    private void Shrink(float s)
    {
        Chunk.transform.localScale *= s;

        // some object shouldn't be scaled
        var pickables = GetComponentsInChildren<Pickable>(includeInactive: true);
        foreach (var pickable in pickables)
        {
            pickable.transform.localScale *= 1f / s;
        }


        IsShrinked = true;
    }
}
