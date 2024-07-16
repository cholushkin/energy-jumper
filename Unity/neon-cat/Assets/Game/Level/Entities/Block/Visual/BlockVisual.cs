using DG.Tweening;
using UnityEngine;

public class BlockVisual : MonoBehaviour
{
    public Renderer BlockRenderer;
    [Range(0.1f, 3f)]
    public float MovingAmplitude;

    [Range(0.0f, 3f)]
    public float AnimationDuration;

    [Tooltip("The delay on which button will return to its initial state. -1 never return")]
    public float ReturnOnPushedDelay;

    private Sequence _currentTween;

    public void EnableVisual(bool flag)
    {
        BlockRenderer.enabled = flag;
    }

    public bool IsVisualEnabled()
    {
        return BlockRenderer.enabled;
    }

    public void StartBlockPushAnimation(Transform block, Vector3 dir)
    {
        if (_currentTween != null && _currentTween.IsPlaying())
            return;

        _currentTween = DOTween.Sequence().Append(
            block.DOBlendableMoveBy(dir * MovingAmplitude, AnimationDuration).SetEase(Ease.InOutCubic)
        );

        if (ReturnOnPushedDelay >= 0f)
        {
            _currentTween.AppendInterval(ReturnOnPushedDelay);
            _currentTween.Append(
                block.DOBlendableMoveBy(-dir * MovingAmplitude, AnimationDuration * 0.25f).SetEase(Ease.InOutCubic)
            );
        }

    }

    [ContextMenu("DbgPlayAnimation")]
    public void DbgPlayAnimation()
    {
        StartBlockPushAnimation(transform, transform.up);
    }
}
