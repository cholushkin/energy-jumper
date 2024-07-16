using UnityEngine;

public class IntentGravitronSetGravity : IntentBase
{
    public Gravitron Gravitron;
    public Vector2 Gravity;

    public override void DoIntention(GameObject destObject)
    {
        Gravitron.SetGravity(Gravity);
    }
}
