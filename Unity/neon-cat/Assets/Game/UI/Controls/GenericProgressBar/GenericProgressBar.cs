using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GenericProgressBar : MonoBehaviour
{
    public float MaxValue;
    public float ValueChangeDuration;
    public Ease ValueChangeEasing;
    public Image FillPart;

    private float _targetValue;
    private float _currentValue;
    //private float _prevValue = float.MinValue;
    private Tweener _t;

    public Action OnReachMax;

    public void InitValue(float value)
    {
        _targetValue = value;
        _currentValue = value;
    }

    public virtual void SetValue(float newValue)
    {
        _targetValue = newValue;
        StartTweening();
    }

    private void StartTweening()
    {
        _t?.Kill();
        _t = DOTween.To(
                () => _currentValue,
                x => _currentValue = x,
                _targetValue,
                ValueChangeDuration)
            .SetEase(ValueChangeEasing)
            .OnUpdate(_tweenUpdate);
    }

    private void _tweenUpdate()
    {
        FillPart.fillAmount = Mathf.Clamp01(_currentValue / MaxValue);
    }
}
