using System.Collections;
using UnityEngine;

namespace Game.VFX
{
    public class SpriteRendererVFXPlayer : MonoBehaviour, IVFXPlayer
    {
        public bool isPlaying => _isPlaying;
        public string id => _id;

        [SerializeField] private bool _loop;
        [SerializeField] private int _framerate = 24;
        
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private SpriteRenderer _renderer;

        private bool _isPlaying;
        private float _frameNumber;
        private int _currentFrame = 0;
        private string _id;
        private VFXFactory _factory;
        public void Play()
        {
            StartCoroutine(StartSequence());
        }
        public void Stop()
        {
            _isPlaying = false;
            _factory.Remove(this);
        }
        public void Reset(Vector3 position)
        {
            _currentFrame = 0;
            _renderer.sprite = _sprites[0];
            transform.position = position;
            gameObject.SetActive(true);
        }
        public void OnCreated(string id, VFXFactory factory)
        {
            _id = id;
            _factory = factory;
            _frameNumber = 1f / _framerate;
        }

        private IEnumerator StartSequence()
        {
            _isPlaying = true;

            while (_currentFrame < _sprites.Length || _loop)
            {
                _renderer.sprite = _sprites[_currentFrame];

                if (_currentFrame >= _sprites.Length && _loop)
                    _currentFrame = 0;
                else
                {
                    _currentFrame++;
                }
                
                if (!_isPlaying)
                    break;

                yield return new WaitForSecondsRealtime(_frameNumber);
            }

            Stop();
        }
    }
}