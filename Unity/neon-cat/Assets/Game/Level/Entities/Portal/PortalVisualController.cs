using DG.Tweening;
using UnityEngine;

public class PortalVisualController : MonoBehaviour
{
    public ParticleSystem Particles;
    public void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    public float PlayAppearingAnimation()
    {
        const float duration = 1f;
        transform.DOScale(Vector3.one, duration).SetEase(Ease.InOutQuint);
        return duration;
    }

    public float PlayDisappearingAnimation()
    {
        const float duration = 1f;
        Particles.Stop();
        transform.DOScale(Vector3.zero, duration).SetEase(Ease.InOutQuint);
        return duration;
    }

    public void PlaySpawnAnimation()
    {
        print("// todo: play spawn");
    }
}
