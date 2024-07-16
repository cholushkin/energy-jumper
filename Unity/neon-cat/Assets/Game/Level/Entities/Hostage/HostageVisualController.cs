using System;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class HostageVisualController : MonoBehaviour
    {
        public void PlayAnimation(float suckingDuration, TweenCallback action)
        {
            transform.DOScale(Vector3.zero, suckingDuration)
                .SetEase(Ease.InExpo)
                .OnComplete(action);
        }
    }
}
