using System.Collections.Generic;
using UnityEngine;

public class ConditionTriggerHit : ConditionBase
{
    public TriggerBase Trigger;

    private bool _isMet;
    void Update()
    {
        _isMet = Trigger.HitCount > 0;
    }
}
