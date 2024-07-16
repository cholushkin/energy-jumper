using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickStartView : MonoBehaviour
{
    public NonDrawingGraphic RaycastTarget;
    public CanvasGroup CanvasGroup;
    public TextMeshProUGUI Text;
    public Button Button;

    public bool IsHidden => !RaycastTarget.raycastTarget;

    public void Hide()
    {
        RaycastTarget.raycastTarget = false;
        CanvasGroup.alpha = 0f;
    }

    public void PlayHideAnimation()
    {
        RaycastTarget.raycastTarget = false;
        float duration = 1.0f;
        DOTween.To(() => CanvasGroup.alpha, x => CanvasGroup.alpha = x, 0.0f, duration);
    }

    public void PlayRevealAnimation()
    {
        RaycastTarget.raycastTarget = true;
        float duration = 1.0f;
        DOTween.To(() => CanvasGroup.alpha, x => CanvasGroup.alpha = x, 1.0f, duration);
    }

    public void SetLevel(int level)
    {
        Text.text = $"Level <b>{(level + 1):00}</b>";
    }
}
