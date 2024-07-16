using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ThumbnailView : MonoBehaviour
{
    public Image MainImage;
    public Image EffectImg1;
    public Image EffectImg2;
    public float AnimationDuration;

    void OnEnable()
    {
        PlayEffect();
    }

    private void PlayEffect()
    {
        EffectImg1.DOKill();
        EffectImg2.DOKill();
        EffectImg1.transform.DOKill();
        EffectImg1.transform.DOKill();
        EffectImg1.gameObject.SetActive(true);
        EffectImg2.gameObject.SetActive(true);
        EffectImg1.transform.localScale = Vector3.one * 1.5f;
        EffectImg2.transform.localScale = Vector3.one * 2.0f;
        EffectImg1.color = new Color(1, 1, 1, 0.5f);
        EffectImg2.color = new Color(1, 1, 1, 0.4f);

        MainImage.transform.DOPunchScale(Vector3.one*0.1f, AnimationDuration, 1);
        EffectImg1.transform.DOScale(Vector3.one, AnimationDuration).SetUpdate(true);
        EffectImg2.transform.DOScale(Vector3.one*1.1f, AnimationDuration).SetUpdate(true);
        EffectImg1.DOFade(0f, AnimationDuration).SetUpdate(true);
        EffectImg2.DOFade(0f, AnimationDuration).SetUpdate(true);
    }

    public void SetSprite(Sprite thumbnail)
    {
        MainImage.sprite = thumbnail;
        EffectImg1.sprite = thumbnail;
        EffectImg2.sprite = thumbnail;
    }
}
