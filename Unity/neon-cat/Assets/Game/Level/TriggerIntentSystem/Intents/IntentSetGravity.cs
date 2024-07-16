using UnityEngine;

public class IntentSetGravitation : IntentBase
{
    public Vector3 Gravity;

    public override void DoIntention(GameObject destObject)
    {
        Debug.LogFormat($"Set Gravity {Gravity}");
        Physics.gravity = Gravity;
    }
}
