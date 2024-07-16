using UnityEngine;

public class IntentHitTrigger : IntentBase
{
    public TriggerCondtionCheckCounter Trigger;

    public override void DoIntention(GameObject destObject)
    {
        Trigger.OnHit();
    }
}
