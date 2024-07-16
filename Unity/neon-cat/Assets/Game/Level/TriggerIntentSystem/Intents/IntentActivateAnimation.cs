using UnityEngine;
using UnityEngine.Assertions;

public class IntentActivateAnimation : IntentBase
{
    public Animator AnimationToActivate;

    public override void DoIntention(GameObject destObject)
    {
        Assert.IsNotNull(AnimationToActivate);
        Debug.LogFormat($"Activating animation {AnimationToActivate}");
        AnimationToActivate.SetBool("Activated", true);
    }
}
