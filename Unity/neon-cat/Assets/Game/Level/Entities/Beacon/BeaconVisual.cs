using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeaconVisual : MonoBehaviour
{
    public Rotating Rotation;
    public Transform EyeLashes;
    public Transform EyeBall;
    public Transform EyePupil;

    public void PlayActivation()
    {
        Rotation.StartRotating();
        Rotation.Tweener.SetUpdate(true);

        EyeLashes.gameObject.SetActive(true);
        EyeLashes.DOScale(Vector3.zero, 0.8f).SetEase(Ease.InOutCubic).From().SetUpdate(true);

        EyeBall.gameObject.SetActive(true);
        EyeBall.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutCubic).From().SetUpdate(true);

        EyePupil.gameObject.SetActive(true);
        EyePupil.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutCubic).From().SetUpdate(true);
    }
}
