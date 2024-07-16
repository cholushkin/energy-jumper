public class TriggerCondtionCheckCounter : TriggerBase
{
    public int Counter;
    public override void OnHit()
    {
        if (IsMuted)
            return;

        --Counter;
        if (Counter <= 0)
            base.OnHit();
    }
}
