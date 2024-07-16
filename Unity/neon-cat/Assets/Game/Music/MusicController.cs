using DG.Tweening;
using GameLib.Alg;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : Singleton<MusicController>
{
    public AudioSource TrackCloserToDestination;
    public AudioSource TrackBackground;
    public AudioMixer AudioMixer;


    public void SetStopMode(bool stopMode)
    {
        const float duration = 0.2f;
        
        AudioMixer.DOSetFloat("Lowpass", stopMode ? 200.0f : 5000f, duration).SetUpdate(true);
    }

    public void SetCloserToDestination(float progress)
    {
        TrackCloserToDestination.volume = progress;
    }
}
