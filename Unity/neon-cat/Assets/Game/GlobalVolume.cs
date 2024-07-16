using GameLib.Alg;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolume : Singleton<GlobalVolume>
{
    private Volume _volume;

    protected override void Awake()
    {
        base.Awake();
        _volume = GetComponent<Volume>();
    }

    public Bloom GetBloom()
    {
        Bloom bloom;
        _volume.profile.TryGet(out bloom);
        return bloom;
    }

    public LensDistortion GetLensDistortion()
    {
        LensDistortion lensDistortion;
        _volume.profile.TryGet(out lensDistortion);
        return lensDistortion;
    }
}
