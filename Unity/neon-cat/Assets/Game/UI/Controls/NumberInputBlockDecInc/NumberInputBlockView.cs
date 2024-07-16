using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberInputBlockView : MonoBehaviour
{
    public TextMeshProUGUI ValueText;
    public Image Icon;
    private Tween _tween;

    public void InitValue(int value)
    {
        ValueText.text = $"{value:00}";
    }

    public virtual void SetValue(int newValue)
    {
        ValueText.text = $"{newValue:00}";

        float duration = 0.3f;
        _tween?.Kill();
        
        _tween = DOTween.Sequence()
            .Append(
                Icon.transform.DOScale(Vector3.one * 1.1f, duration/2f).SetEase(Ease.OutCirc).SetUpdate(true)
            )
            .Append(
                Icon.transform.DOScale(Vector3.one, duration / 2f).SetEase(Ease.OutQuint).SetUpdate(true)
            );
    }
}
