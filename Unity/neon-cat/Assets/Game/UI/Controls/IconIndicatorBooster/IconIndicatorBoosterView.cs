using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IconIndicatorBoosterView : IconIndicatorView
{
    public delegate float ProgressionMethod();

    public Image ImageProgression;
    public Ease ActivateIconEase;
    public float ActivateIconDuration;

    ProgressionMethod _progressionCallback;


    public void StartProgressing(ProgressionMethod progressionMethod)
    {
        _progressionCallback = progressionMethod;
    }

    public override void Update()
    {
        base.Update();
        if (_progressionCallback == null)
        {
            ImageProgression.fillAmount = 0f;
            return;
        }

        var progress = _progressionCallback();
        ImageProgression.fillAmount = 1f - progress;
        if (Math.Abs(progress - 1f) < 0.0001f)
        {
            _progressionCallback = null;
            PlayActivateIcon();
        }
    }

    private void PlayActivateIcon()
    {
        DOTween.Sequence()
            .Append(ScalingPivot.transform.DOScale(Vector3.one * IndicatorScalingEffectScale, ActivateIconDuration)
                .SetEase(ActivateIconEase))
            .Append(ScalingPivot.transform.DOScale(Vector3.one, 0.5f)).SetEase(Ease.OutQuart);
    }

    public bool IsDoneProgressing()
    {
        return _progressionCallback == null;
    }
}