using System;
using System.Collections;
using DG.Tweening;
using GameLib.Log;
using TMPro;
using UnityEngine;

/*
 *  - IconIndicator
 *    - Container
 *      - Image
 *      - Text
 */

public class IconIndicatorView : MonoBehaviour
{
    public IconPopupMessenger PopupMesseger;
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

        if (PopupMesseger)
            PopupMesseger.ShowMessage((valDelta < 0 ? "- " : "+ ") + Mathf.Abs(valDelta), PopupTextDuration);
    }

    public void SetValueVisible(bool flag, bool instant = false)
    {
        Text.DOFade(flag ? 1f : 0f, instant ? 0f : 1f);
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
        _t = DOTween.To(() => _currentValue, x => _currentValue = x, _targetValue, ValueChangeDuration)
            .SetEase(TextChangingEasing)
            .SetUpdate(true);

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
        _s.SetUpdate(true);
    }

    #region test methods
    [ContextMenu("Test: +250")]
    public void TestAddValue()
    {
        ChangeValue(250);
    }

    [ContextMenu("Test: -250")]
    private void TestDeductValue()
    {
        ChangeValue(-250);
    }

    [ContextMenu("Test: -1")]
    private void TestDeduct1()
    {
        ChangeValue(-1);
    }


    [ContextMenu("Test: 5x +")]
    public void TestAddValue5X()
    {
        StartCoroutine(TestMultiple());
    }

    IEnumerator TestMultiple()
    {
        ChangeValue(10);
        yield return new WaitForSeconds(0.5f);
        ChangeValue(20);
        yield return new WaitForSeconds(0.5f);
        ChangeValue(30);
        yield return new WaitForSeconds(0.5f);
        ChangeValue(40);
        yield return new WaitForSeconds(0.5f);
        ChangeValue(50); // 150
    }

    [ContextMenu("Test: 5x -")]
    private void TestDeductValue5x()
    {
        ChangeValue(-110);
        ChangeValue(-120);
        ChangeValue(-130);
        ChangeValue(-140);
        ChangeValue(-150);
    }
    #endregion
}
