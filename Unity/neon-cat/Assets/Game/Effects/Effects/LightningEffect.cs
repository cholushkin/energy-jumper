using DG.Tweening;
using GameLib.Random;
using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    public Light LightSoure;
    public Range DelayBetweenShots;
    public float OneShotDuration;
    public float MaxIntensity;
    private float _defaultIntensity;


    public void Start()
    {
        _defaultIntensity = LightSoure.intensity;
        NextIteration();
    }

    private void NextIteration()
    {
        DOVirtual.DelayedCall(Random.Range(DelayBetweenShots.From, DelayBetweenShots.To), () =>
        {
            Shot();
            NextIteration();
        });
    }

    private void Shot()
    {
        // safe assignment
        LightSoure.intensity = _defaultIntensity;

        // from current intensity to MaxIntensity
        DOVirtual.Float(LightSoure.intensity, 4f, OneShotDuration, (x) => LightSoure.intensity = x)
            .SetEase(Ease.InOutBounce)
            .SetLoops(2, LoopType.Yoyo);

    }
}
