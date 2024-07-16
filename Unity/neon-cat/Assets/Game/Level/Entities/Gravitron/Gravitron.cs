using DG.Tweening;
using UnityEngine;

public class Gravitron : MonoBehaviour
{
    public float ChangeDuration;
    public Vector2 GravityVectorTarget;
    void Update()
    {
        Physics.gravity = GravityVectorTarget;
    }

    public void SetGravity(Vector2 gravity)
    {
        DOTween.To(() => GravityVectorTarget, x => GravityVectorTarget = x, gravity, ChangeDuration);
    }
}
