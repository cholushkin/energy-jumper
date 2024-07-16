using System;
using UnityEngine;
using UnityEngine.UI;

public class NumberInputBlockController : MonoBehaviour
{
    public float ValChageAcceleration;
    public float ValChageRate;
    public NumberInputBlockView View;
    public Button DecValueButton;
    public Button IncValueButton;
    public int MaxValue;
    public int Value;

    private bool _decDown;
    private bool _incDown;
    private float MaxValChangeRate;
    public float _val;
    
    public Action LimitReached;
    private bool _warningFlag;


    void Awake()
    {
        View.InitValue(Value);
        MaxValChangeRate = ValChageRate;
    }

    void Update()
    {
        if (_decDown || _incDown)
        {
            ValChageRate += Time.unscaledDeltaTime * ValChageAcceleration;
            if (ValChageRate > MaxValChangeRate)
                ValChageRate = MaxValChangeRate;

        }
        if (_decDown)
        {
            _val -= Time.unscaledDeltaTime * ValChageRate;
            Value = Mathf.Clamp((int)Mathf.Round(_val), 0, MaxValue);
            View.SetValue(Value);
        }

        if (_incDown)
        {
            _val += Time.unscaledDeltaTime * ValChageRate;
            Value = Mathf.Clamp((int)Mathf.Round(_val), 0, MaxValue);
            var notClamped = (int)Mathf.Round(_val);
            if (notClamped > Value && !_warningFlag)
            {
                _warningFlag = true;
                LimitReached.Invoke();
            }

            View.SetValue(Value);
        }
    }

    public void Setup(int maxValue)
    {
        MaxValue = maxValue;
        Value = Mathf.Clamp(Value, 0, MaxValue);
        _val = Value;
        View.SetValue(Value);
    }

    public void OnDecDown()
    {
        ValChageRate = 0f;
        Value--;
        _val = Value + 0.5f;
        _decDown = true;
    }

    public void OnDecUp()
    {
        _decDown = false;
    }

    public void OnIncDown()
    {
        ValChageRate = 0f;
        Value++;
        _val = Value - 0.5f;
        _incDown = true;
        _warningFlag = false;
    }

    public void OnIncUp()
    {
        _incDown = false;
        _warningFlag = false;
    }
}
