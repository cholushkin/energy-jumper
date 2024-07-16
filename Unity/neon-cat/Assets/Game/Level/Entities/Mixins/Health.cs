using System;
using GameLib.Alg;
using UnityEngine;

public class Health : MonoBehaviour
{
    public interface IHealthHandler
    {
        void OnDamage(float currentHealth, float healthBeforeDamage, Damage.DamageTypeEnum damageType);
        void OnDie(float currentHealth, float healthBeforeDie);
        void OnMeatballed();
        void OnHealed(float currentHealth, float healthBeforeHeal);
        void OnResurrected();

    }

    public struct Damage
    {
        public enum DamageTypeEnum
        {
            Generic,
            Projectile,
            Field,
            PhysicsImpact
        }

        public DamageTypeEnum DamageType;
        public float Value;
    }

    public float InitialHealth;
    public float MaxHealth;
    public float MinHealth; // could be negative for dead body injuring
    public float MeatballValue; // negative val. If negative Value is less than MeatballValue than it's time for gibs
    public bool IsResurrectionPossible; // can it be healed after death (only if not meatballed)
    public bool IsHealPossible;
    public bool IsInvulnerable;

    public float Value { get; private set; }
    public bool IsDead => Value <= 0f;
    public bool IsMeatballed { get; private set; }


    private IHealthHandler _healthHandler;


    void Awake()
    {
        Value = InitialHealth;
        IsMeatballed = false;
    }

    public void Reset(float value)
    {
        InitialHealth = value;
        Value = value;
        IsMeatballed = false;
    }

    public void SetHandler(IHealthHandler healthHandler)
    {
        _healthHandler = healthHandler;
    }

    public void Heal(float hp)
    {
        if (!IsHealPossible)
            return;
        if(IsMeatballed)
            return;

        if (hp <= 0f)
        {
            Debug.LogError($"Trying to heal with the negative value {hp}, {transform.GetDebugName()}");
            return;
        }

        var wasDead = IsDead;
        var oldValue = Value;

        if (!IsDead || (IsResurrectionPossible && IsDead))
        {
            Value += hp;
            Value = Mathf.Clamp(Value, MinHealth, MaxHealth);
            _healthHandler?.OnHealed(Value, oldValue);
        }

        // resurrection?
        if (IsResurrectionPossible && wasDead && IsDead == false)
            _healthHandler?.OnResurrected();
    }

    public void DoDamage(float damage, Damage.DamageTypeEnum damageType = Damage.DamageTypeEnum.Generic)
    {
        _healthHandler?.OnDamage(Value, Value, damageType);
        
        if (IsInvulnerable)
            return;

        if (damage <= 0f)
        {
            Debug.LogError($"Trying to damage with the negative value {damage}, {transform.GetDebugName()}");
            return;
        }

        var prevIsDead = IsDead;
        var prevValue = Value;

        Value -= damage;
        Value = Mathf.Clamp(Value, MinHealth, MaxHealth);

        bool isDeadOnThisFrame = IsDead && prevIsDead == false;

        // death occured
        if (isDeadOnThisFrame)
            _healthHandler?.OnDie(Value, prevValue);

        if (Value < MeatballValue && !IsMeatballed)
        {
            IsMeatballed = true;
            _healthHandler?.OnMeatballed();
        }
    }
}
