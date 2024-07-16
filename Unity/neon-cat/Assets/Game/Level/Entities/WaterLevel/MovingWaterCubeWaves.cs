using DG.Tweening;
using UnityEngine;

public class MovingWaterCubeWaves : MonoBehaviour
{
    public bool StartOnAwake;
    public Vector3 MaxWaveAmplitude;
    public float MovingDuration;
    public Transform WaterCube;

    void Awake()
    {
        if (StartOnAwake)
            StartMoving(true);
    }

    public void StartMoving(bool flag)
    {
        WaterCube.DOMove(WaterCube.transform.position + MaxWaveAmplitude, MovingDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
