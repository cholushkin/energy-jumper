using Game;
using GameLib.Log;
using UnityEngine;

// * fractures 
//   * optional Health (if there is no health then use 3 hits to destroy)
public class Fractured : MonoBehaviour, Health.IHealthHandler
{
    public float[] Lives;
    public Health Health;
    public FractureController FractureController;
    public LogChecker LogChecker;
    public bool IsExploded { get; private set; }

    private int _currentLiveIndex;
    
    // last impact
    private GameObject _otherGameObject;
    private Vector3 _impactDirection;
    private float _energy;
    


    void Reset()
    {
        Lives = new[] {150f, 150f, 150f};
        _currentLiveIndex = 0;
        Health = GetComponent<Health>();
        FractureController = GetComponent<FractureController>();
    }

    void Awake()
    {
        Health.SetHandler(this);
        SetNextLive();
    }

    private void SetNextLive()
    {
        if(LogChecker.Verbose())
            Debug.Log($"Next life {_currentLiveIndex}");
        Health.Reset(Lives[_currentLiveIndex]);
        ++_currentLiveIndex;
    }

    public void Hit(GameObject otherGameObject, Vector3 impactDirection, float energy)
    {
        _otherGameObject = otherGameObject;
        _impactDirection = impactDirection;
        _energy = energy;

        Health.DoDamage(energy);
    }
   
    #region IHealthHandler
    public void OnDamage(float currentHealth, float healthBeforeDamage, Health.Damage.DamageTypeEnum damageType)
    {
    }

    public void OnDie(float currentHealth, float healthBeforeDie)
    {
        if (_currentLiveIndex == Lives.Length)
        {
            FractureController.Explode(_impactDirection, _energy, _otherGameObject.transform.position);
            IsExploded = true;
            return;
        }
        FractureController.Crack(_impactDirection, _energy, _otherGameObject.transform.position);
        SetNextLive();
    }

    public void OnMeatballed()
    {
    }

    public void OnHealed(float currentHealth, float healthBeforeHeal)
    {
    }

    public void OnResurrected()
    {
    }
    #endregion
}
