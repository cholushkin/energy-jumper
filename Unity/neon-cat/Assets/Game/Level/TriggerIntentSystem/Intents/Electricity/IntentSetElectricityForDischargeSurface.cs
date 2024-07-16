using DG.Tweening;

public class IntentSetElectricityForDischargeSurface : IntentBase
{
    public float ElectricityDuration;
    public PowerSupplyAndSwitcherDischargeSurface PowerSupply;
    private Tween _prevTween;

    void Reset()
    {
        PowerSupply = GetComponent<PowerSupplyAndSwitcherDischargeSurface>();
    }
    public override void Apply() // apply intent to objects
    {
        base.Apply();
        PowerSupply.SetPower(true);
        _prevTween?.Kill();
        _prevTween = null;
        _prevTween = DOVirtual.DelayedCall(ElectricityDuration, () => PowerSupply.SetPower(false), false);
    }

    void OnDestroy() // in case level is destroyed but we have virtual running
    {
        _prevTween?.Kill();
    }
}
