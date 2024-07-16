using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class PlayerTimeController : MonoBehaviour
    {
        enum State
        {
            Frozen,
            Unfrozen
        }

        private State _curState;
        public float TransitionDuration;
        private float _curValue;
        private Tween _curTween;
        private Tween _lensDistortionTween;

        public void Awake()
        {
            _curValue = 1f;
            _curState = State.Unfrozen;
        }

        public void Freeze()
        {
            if (_curState == State.Frozen)
                return;

            _curState = State.Frozen;
            if (_curTween != null)
                _curTween.Kill();
            _curTween = DOTween.To(() => _curValue, TimeScaleSetter, 0f, TransitionDuration).SetUpdate(true);
            MusicController.Instance.SetStopMode(true);

            // todo: move to EffectManager
            // lens animation
            {
                _lensDistortionTween?.Kill();

                var lensDistortion = GlobalVolume.Instance.GetLensDistortion();
                if (!lensDistortion.IsActive())
                    lensDistortion.active = true;
                const float duration = 0.3f;
                _lensDistortionTween = DOTween.To(() => LensDistortionIntensityGetter(), LensDistortionIntensitySetter,
                    -0.3f, duration).SetUpdate(true).SetEase(Ease.OutCubic);
            }
        }

        public void Unfreeze()
        {
            if (_curState == State.Unfrozen)
                return;

            _curState = State.Unfrozen;
            if (_curTween != null)
                _curTween.Kill();
            TimeScaleSetter(1f);
            MusicController.Instance.SetStopMode(false);

            // lens animation
            {
                _lensDistortionTween?.Kill();
                const float duration = 0.25f;
                _lensDistortionTween = DOTween.To(() => LensDistortionIntensityGetter(), LensDistortionIntensitySetter,
                    0.0f, duration).SetUpdate(true);
            }
        }

        void TimeScaleSetter(float val)
        {
            _curValue = val;
            if(StateGameplay.Instance.IsInPause())
                return;
            Time.timeScale = val;
        }
        private float LensDistortionIntensityGetter()
        {
            return GlobalVolume.Instance.GetLensDistortion().intensity.value;
        }

        private void LensDistortionIntensitySetter(float val)
        {
            GlobalVolume.Instance.GetLensDistortion().intensity.value = val;
            if (val == 0f)
            {
                var lensDistortion = GlobalVolume.Instance.GetLensDistortion();
                lensDistortion.active = false;
            }
        }
    }
}