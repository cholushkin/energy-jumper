using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameLib.Log;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUpgradeView : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI Text;
    public Transform ScalingPivot;
    public string ValueFormatString;
    public float PopupTextDuration;
    public float ValueChangeDuration;
    public float IndicatorScalingEffectDuration;
    public float IndicatorScalingEffectScale;

    public Ease TextChangingEasing;
    public Ease ScalingEasing;
    public bool ResetScaleEachAnimation;
    public LogChecker Log;

    private int _targetValue;

    private int _currentValue;
    private int _prevValue = Int32.MinValue;

    public void Reset()
    {
        // setting default values
        ValueFormatString = "{0}";
        PopupTextDuration = 2f;
        ValueChangeDuration = 2f;
        IndicatorScalingEffectDuration = 0.4f;
        IndicatorScalingEffectScale = 1.1f;
        TextChangingEasing = Ease.OutCubic;
        ScalingEasing = Ease.OutQuint;
    }

    public virtual void Update()
    {
        // updating the text
        {
            if (_prevValue == _currentValue)
                return;
            Text.text = string.Format(ValueFormatString, _currentValue);
            _prevValue = _currentValue;
        }
    }

    public void InitValue(int value)
    {
        KillTweening();
        Text.text = string.Format(ValueFormatString, value);
        _targetValue = value;
        _currentValue = value;
    }

    public virtual void SetValue(int newValue)
    {
        _targetValue = newValue;
        StartTweening();
    }

    public int GetValue()
    {
        return _targetValue;
    }

    public virtual void ChangeValue(int valDelta)
    {
        _targetValue += valDelta;
        StartTweening();
    }

    private Tweener _t;
    private Sequence _s;

    private void KillTweening()
    {
        _t?.Kill();
    }

    private void StartTweening()
    {
        _t?.Kill();
        _t = DOTween.To(() => _currentValue, x => _currentValue = x, _targetValue, ValueChangeDuration).SetEase(TextChangingEasing);

        if (_s != null)
            _s.Kill();
        if (ResetScaleEachAnimation)
            ScalingPivot.transform.localScale = Vector3.one;
        _s = DOTween.Sequence()
            .Append(
                ScalingPivot.transform.DOScale(Vector3.one * IndicatorScalingEffectScale, IndicatorScalingEffectDuration).SetEase(ScalingEasing)
            )
            .Append(
                ScalingPivot.transform.DOScale(Vector3.one, IndicatorScalingEffectDuration * 2f).SetEase(Ease.OutQuint)
            );
    }

    public void SetInteractable(bool flag)
    {
        Button.interactable = flag;
    }

    public void SetPriceInfoVisible(bool flag, bool noAnimation)
    {
        Text.DOFade(flag ? 1f : 0f, noAnimation? 0f:1f);
    }
}
