using System.Collections;
using System.Collections.Generic;
using LeakyAbstraction;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private static float _lastTimePlayed;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > 1f)
        {

            var volFactor = Mathf.Clamp(collision.impulse.magnitude, 1f, 25f) / 25f;
            if (volFactor > 0.01f)
            {
                if (Time.timeSinceLevelLoad - _lastTimePlayed < 0.1f)
                    return;
                _lastTimePlayed = Time.timeSinceLevelLoad;
                SoundManager.Instance.PlaySound(GameSound.SfxShatterHitGround, volFactor, 1f);
            }
        }
    }
}
