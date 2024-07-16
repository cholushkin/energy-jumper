using DG.Tweening;
using UnityEngine;

public class MovingWaterCube : MonoBehaviour
{
    public bool StartOnAwake;
    public Vector3 MovingDistance;
    public float MovingDuration;
    public Transform WaterCube;

    void Awake()
    {
        if (StartOnAwake)
            StartMoving(true);
    }

    public void StartMoving(bool flag)
    {
        WaterCube.DOMove(WaterCube.transform.position + MovingDistance, MovingDuration);
    }
}
