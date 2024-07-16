using System.Collections;
using DG.Tweening;
using GameLib.Random;
using UnityEngine;

public class ChargePillGeneratorVisual : MonoBehaviour
{
    public Transform[] Ring;
    public Range EachFluactuationDuration;
    public float PickupAnimationDuration;
    public float TickAnimationDuration;

    private bool _idle;
    private Tweener[] _ringTweener = new Tweener[3];
    private int _cooldownTick;

    public void PlayIdleAnimation()
    {
        if (_idle)
            return;

        _idle = true;
        StartCoroutine(IdleCoroutine());
    }

    private void StopIdleAnimation()
    {
        _idle = false;
        StartCoroutine("IdleCoroutine");
        for (int i = 0; i < 3; ++i)
        {
            _ringTweener[i].Kill();
            Ring[i].transform.localRotation = Quaternion.identity;
        }
    }
    private IEnumerator IdleCoroutine()
    {
        while (_idle)
        {
            int completeCounter = 0;
            var dur = Random.Range(EachFluactuationDuration.From, EachFluactuationDuration.To);
            for (int i = 0; i < 3; ++i)
            {
                _ringTweener[i] = Ring[i].DOLocalRotate(
                        new Vector3(0, Random.Range(-35, 35), 0)
                        , dur)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.InOutBounce)
                    .OnComplete(() => completeCounter++);
            }

            yield return new WaitUntil(() => completeCounter >= 3);
        }
    }

    public void PlayPickupAnimation()
    {
        StopIdleAnimation();

        Ring[1].DOScale(Vector3.one * 0.46083f, PickupAnimationDuration).SetEase(Ease.OutCubic);
        Ring[2].DOScale(Vector3.one * 0.25753f, PickupAnimationDuration).SetEase(Ease.OutCubic).SetDelay(0.2f);
        _cooldownTick = -1;
    }

    public void CooldownProgress(float progress)
    {
        var prevTick = _cooldownTick;
        if (progress >= 1.0f)
            _cooldownTick = -1;
        else if (progress > 0.75f)
            _cooldownTick = 0;
        else if (progress > 0.5f)
            _cooldownTick = 1;
        else if (progress > 0.25f)
            _cooldownTick = 2;
        else if (progress >= 0.0f)
            _cooldownTick = 3;
        if (prevTick != _cooldownTick)
            PlayTickAnimation(_cooldownTick);
    }

    private void PlayTickAnimation(int tick)
    {
        if (tick == 2)
            Ring[2].DOScale(Vector3.one, TickAnimationDuration).SetEase(Ease.InBack);
        if (tick == 3)
            Ring[1].DOScale(Vector3.one, TickAnimationDuration).SetEase(Ease.InBack);
    }

    public void PlaySpawnAnimation()
    {
        Ring[1].DOScale(Vector3.one, 0.2f).SetEase(Ease.OutElastic).SetLoops(2, LoopType.Yoyo);
        Ring[2].DOScale(Vector3.one, 0.2f).SetEase(Ease.OutElastic).SetLoops(2, LoopType.Yoyo); ;
        PlayIdleAnimation();
    }
}
