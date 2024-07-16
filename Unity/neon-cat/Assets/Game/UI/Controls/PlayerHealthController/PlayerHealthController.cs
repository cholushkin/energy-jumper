using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public GenericProgressBar NegativePart;
    public GenericProgressBar PositivePart;
    public Health Health;

    private float _targetValue;


    void Update()
    {
        Set(Health.Value);
    }


    public void Init(Health h, float maxValue, float minValue, float startValue)
    {
        Health = h;
        _targetValue = startValue;
        NegativePart.SetValue(0);
        NegativePart.MaxValue = Mathf.Abs(minValue);
        PositivePart.SetValue(startValue);
        PositivePart.MaxValue = maxValue;
    }

    public void Set(float value)
    {
        if(_targetValue == value)
            return;
        _targetValue = value;
        if (_targetValue >= 0f)
        {
            PositivePart.SetValue(_targetValue);
            NegativePart.SetValue(0f);
        }
        else
        {
            PositivePart.SetValue(0f);
            NegativePart.SetValue(_targetValue);
        }
    }

    public virtual void ChangeValue(int valDelta)
    {
        Set(_targetValue + valDelta);
    }

    #region test methods
    [ContextMenu("Test: +10")]
    public void TestAddValue()
    {
        ChangeValue(10);
    }

    [ContextMenu("Test: -10")]
    private void TestDeductValue()
    {
        ChangeValue(-10);
    }
    #endregion
}
