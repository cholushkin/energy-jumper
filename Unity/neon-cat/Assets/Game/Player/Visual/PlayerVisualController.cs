using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using Game;
using Game.Input;
using LeakyAbstraction;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerVisualController : MonoBehaviour, IHandle<PlayerController.EventDeath>
{
    public enum Animation
    {
        Spawning,
        Despawning,
        Idle,
        DeathDisolve
    }

    public EyesBlinking Eyes;
    public Color ChargedColor;
    public Color UnchargedColor;
    public GameObject EnergyGameObject;


    public ParticleSystem ElectricityCircle;
    public ParticleSystem Trail;
    public Renderer Ship;
    public AimingArrow AimingArrow;

    public Collider[] Colliders; // for geometry colliders (not used when we are on simplified colliders)

    public Transform CharLookPoint;

    private Animation _currentAnimation;
    private Vector3 _initialScale;

    public Transform GetEffectsParent()
    {
        return transform;
    }

    public void Init()
    {
        _initialScale = transform.localScale;
        GlobalEventAggregator.EventAggregator.Subscribe(this);
    }


    public void PlayAnimation(Animation anim, float duration, TweenCallback onComplete = null)
    {
        _currentAnimation = anim;
        if (anim == Animation.Spawning)
        {
            transform.DOScale(_initialScale, duration)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    PlayAnimation(Animation.Idle, 1f);
                    onComplete?.Invoke();
                });
        }
        if (anim == Animation.Despawning)
        {
            transform.DOScale(Vector3.zero, duration)
                .SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    PlayAnimation(Animation.Idle, 1f);
                    onComplete?.Invoke();
                });
        }

        if (anim == Animation.DeathDisolve)
        {
            // todo: replace mesh with gost and TopMost layer and play particle effect on top of it
            // EffectManager.Instance.ApplyHologramHideEffect(gameObject, true);
        }
    }

    public void DisableEyes()
    {
        Eyes.Stop();
    }

    public void SetCharged(bool flag)
    {
        //Ship.materials[0].SetColor("_BaseColor", flag ? ChargedColor : UnchargedColor);
        //Ship.materials[1].SetColor("_BaseColor", flag ? ChargedColor : UnchargedColor);
        // charged color change
        {
            var destColor = flag ? ChargedColor : UnchargedColor;
            const float duration = 0.1f;
            EnergyGameObject.GetComponent<Renderer>().materials[0].DOColor(destColor, "EnergyColor", duration);
        }

        if (flag)
        {
            SoundManager.Instance.PlaySound(GameSound.SfxShipCharge);
            var electroShoot = Instantiate(ElectricityCircle, ElectricityCircle.transform.parent);
            electroShoot.gameObject.SetActive(true);
            electroShoot.transform.DOScale(electroShoot.transform.localScale * 0.8f, 0.6f).SetEase(Ease.InQuint)
                .OnComplete(() => Destroy(electroShoot.gameObject));
        }
    }

    public void ScaleToZero()
    {
        transform.localScale = Vector3.zero;
    }

    public Animation GetCurrentAnimation()
    {
        return _currentAnimation;
    }

    //public void OnGUI()
    //{
    //    OnGuiHelper.Instance?.Left(5, string.Format("PlayerVisualController.State:{0}", _currentAnimation));
    //}

    public void SetCollidersEnabled(bool flag)
    {
        Assert.IsNotNull(Colliders, "unity crashes if you scale active colliders to zero");
        foreach (var coll in Colliders)
        {
            coll.enabled = flag;
        }
    }

    public void ShiftLookPoint(Vector3 rigidbodyVelocity)
    {
        // todo:
        //var project = Vector3.Project(rigidbodyVelocity, Vector3.right);
        //Debug.Log($"{project.x}  {project.y} ");
        //CharLookPoint.rotation = Quaternion.identity;
        //CharLookPoint.localPosition = new Vector3(project.x * 0.5f, 0, -10);
    }

    public void Handle(PlayerController.EventDeath message)
    {
        Trail.Stop();
    }
}