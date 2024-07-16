using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

public class IntentSetElecrticityForChargeSurface : IntentBase
{
    [Tooltip("How long electricity will be turned on before turn off again. -1 = forever")]
    public float ElectricityDuration;
    public PowerSupplyAndSwitcherChargeSurface PowerSupply;
    private Tween _prevTween;

    void Reset()
    {
        PowerSupply = GetComponent<PowerSupplyAndSwitcherChargeSurface>();
    }
    public override void Apply() // apply intent to objects
    {
        //Assert.IsTrue(DestinationObjects==null||DestinationObjects.Count == 0, "use PowerSupply ref instead");
        base.Apply();
        PowerSupply.SetPower(true);
        _prevTween?.Kill();
        _prevTween = null;
        if(ElectricityDuration != -1)
            _prevTween = DOVirtual.DelayedCall(ElectricityDuration, () => PowerSupply.SetPower(false), false);
    }

    void OnDestroy() // in case level is destroyed but we have virtual running
    {
        _prevTween?.Kill();
    }
}
