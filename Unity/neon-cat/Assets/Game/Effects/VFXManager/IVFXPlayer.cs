using UnityEngine;

namespace Game.VFX
{
    public interface IVFXPlayer
    {
        bool isPlaying { get; }
        string id { get; }
        GameObject gameObject { get; }
        void Play();
        void Stop();
        void Reset(Vector3 position);
        void OnCreated(string id, VFXFactory factory);
    }
}