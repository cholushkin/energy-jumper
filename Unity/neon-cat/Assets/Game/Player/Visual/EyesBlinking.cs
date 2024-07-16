using System.Linq;
using GameLib.Alg;
using GameLib.Random;
using UnityEngine;

public class EyesBlinking : MonoBehaviour
{
    public enum EyeState
    {
        OpenedEyes,
        ClosedEyes
    }

    public Range BlinkDelay;
    public float PerBlinkDuration;
    public Transform[] GroupEyes;

    private float _curStateRemainingDuration;
    private EyeState _state;

    void Reset()
    {
        if (BlinkDelay == null || BlinkDelay.IsZero())
            BlinkDelay = new Range(1f, 3f);
        if (PerBlinkDuration == 0f)
            PerBlinkDuration = 0.2f;
        if (GroupEyes == null || GroupEyes.Length == 0)
            GroupEyes = transform.Children().ToArray();
    }
    void Start()
    {
        _state = EyeState.OpenedEyes;
        _curStateRemainingDuration = 3f;
    }

    public void Stop()
    {
        enabled = false;
        CloseEyes();
    }

    void Update()
    {
        _curStateRemainingDuration -= Time.unscaledDeltaTime;
        if (_state == EyeState.OpenedEyes)
        {
            if (_curStateRemainingDuration < 0f)
            {
                // goto closed eyes state
                _state = EyeState.ClosedEyes;
                _curStateRemainingDuration = PerBlinkDuration;
                CloseEyes();
            }
        }
        else if (_state == EyeState.ClosedEyes)
        {
            if (_curStateRemainingDuration < 0f)
            {
                // goto opened eyes state
                _state = EyeState.OpenedEyes;
                _curStateRemainingDuration = Random.Range(BlinkDelay.From, BlinkDelay.To);
                OpenEyes();
            }
        }
    }

    private void OpenEyes()
    {
        foreach (var eye in GroupEyes)
            eye.gameObject.SetActive(true);
    }

    private void CloseEyes()
    {
        foreach (var eye in GroupEyes)
            eye.gameObject.SetActive(false);
    }
}
