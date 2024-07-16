using UnityEngine;

public class IntentSetGravity : IntentBase
{
    public Vector3 Gravity;

    public override void DoIntention(GameObject destObject)
    {
        Debug.LogFormat($"Set gravitation {Gravity}");
        Physics.gravity = Gravity;
    }
}
