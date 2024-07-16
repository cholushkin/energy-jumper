using DG.Tweening;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI
{
    public class Arrow : MonoBehaviour
    {
        public LookAt LookAt;
        public float PointerSpeed; // speed to move along the path
        public float DumpingFactor; // speed for SmoothDump
        public float Duration; // -1 ininite
        public float CaptionDuration; // -1 ininite

        private Transform _target; // following target
        public Transform _player { get; private set; }
        public Camera _camera { get; private set; }

        private Vector3 _velocity = Vector3.zero; // for SmoothDump
        private Vector3 _movingPointer; // for SmoothDump
        private float _currentDuration;
        private float _currentCaptionDuration;
        private float _currentPath;
        
        private const float TargetOffset = 20f;
        private const float ArrowToPlayerOffset = 12f;
        private const float ScreenFrameMargin = 32f;

        private bool _isPLayerTooClose;
        private bool _isProcessingTarget;
        private bool _isDeathRequested;

        private Tween _tween;
        //public class TargetItem
        //{
        //    public float ArrowSpeed;
        //    public Transform Target;
        //    public Transform Arrow;
        //    public float Path;
        //    public bool IsPlayerTooClose;
        //}

        void Awake()
        {
            Assert.IsNotNull(LookAt);

        }

        void Start()
        {
            _currentDuration = Duration;
        }

        void Update()
        {
            // process death 
            if (ShouldBeKilled())
            {
                // wait all animations to be completed
                if (_tween != null && _tween.IsActive())
                    return;
                _startDying();
            }


            // process duration
            if (_currentDuration >= 0f)
                _currentDuration -= Time.deltaTime;

            if (_isProcessingTarget)
            {
                // update _movingPointer by PlayerPos, TargetPos and ScreenFrame, based on 
                UpdateArrowPosition();

            }





            // simple animation, later just play appera\disappear animation for the arrow
            //if (targetItem.IsPlayerTooClose && targetItem.Arrow.transform.localScale == Vector3.one)
            //{
            //    targetItem.Arrow.transform.DOScale(Vector3.zero, 1f)
            //        .OnComplete(() => targetItem.Path = 0f);
            //}
            //if (!targetItem.IsPlayerTooClose && targetItem.Arrow.transform.localScale == Vector3.zero)
            //    targetItem.Arrow.transform.DOScale(Vector3.one, 1f);
        }


        public void Set(Transform player, Transform target, float duration, float captionDuration, Camera camera)
        {
            transform.localScale = Vector3.zero;
            _player = player;
            _target = target;
            Duration = duration;
            _currentDuration = duration;
            CaptionDuration = captionDuration;
            _camera = camera;
            _currentPath = 0f;
            LookAt.Target = target;
        }

        public void EnableFollowing(bool flag)
        {
            _isProcessingTarget = flag;
        }

        public void Appear()
        {
            if (ShouldBeKilled())
                return;
            if (transform.localScale == Vector3.zero)
                _tween = transform.DOScale(Vector3.one, 1f);
        }

        public void Disappear()
        {
            if (ShouldBeKilled())
                return;
            if (transform.localScale == Vector3.one)
                _tween = transform.DOScale(Vector3.zero, 1f);
        }

        public void Die()
        {
            _isDeathRequested = true;
        }



        public bool ShouldBeKilled()
        {
            if (_isDeathRequested)
                return true;
            if (_target == null)
                return false;
            if (Duration > 0f) // if not infinite
                return _currentDuration > 0f; // 
            return false;
        }


        private void _startDying()
        {
            Debug.Log("start die");
            if (transform.localScale == Vector3.zero)
            {
                Destroy(gameObject);
                return;
            }
            _tween = transform.DOScale(Vector3.zero, 1f).OnComplete(() => Destroy(gameObject));
        }


        private void ProcessMovement()
        {

        }

        private void ShowCaption()
        {

        }

        private void HideCaption()
        {
        }


        //public void Appear()
        //{
        //    if (_stateMachine.CurrentState.State == State.Hidden)
        //        _stateMachine.GoTo(State.Appearing);
        //}

        //public void Disappear()
        //{
        //    if (_stateMachine.CurrentState.State == State.Alive)
        //        _stateMachine.GoTo(State.Disappearing);
        //}

        //public void Die()
        //{
        //    StartDisappearing().OnComplete(() => Destroy(gameObject));
        //}


        // alg 
        // * move along direction to target
        // * when next step is out of screen coordinates (screen space) move in opposite direction
        void UpdateArrowPosition()
        {
            var target2Player = _target.position - _player.transform.position;
            var pointerPos = _player.position + target2Player.normalized * (ArrowToPlayerOffset + _currentPath);
            var pointerScreenPos = _camera.WorldToScreenPoint(pointerPos);

            var isOutOfScreen = (pointerScreenPos.x > Screen.width - ScreenFrameMargin) || (pointerScreenPos.x < ScreenFrameMargin)
                                || (pointerScreenPos.y < ScreenFrameMargin) || (pointerScreenPos.y > Screen.height - ScreenFrameMargin);

            var dx = PointerSpeed * Time.deltaTime;
            _currentPath += isOutOfScreen ? -dx : dx;

            // _isPLayerTooClose ?
            {
                _isPLayerTooClose = false;
                if (_currentPath < 0)
                {
                    _currentPath = 0;
                    _isPLayerTooClose = true;
                }

                if (ArrowToPlayerOffset + _currentPath + TargetOffset > target2Player.magnitude)
                {
                    _currentPath = target2Player.magnitude - TargetOffset - ArrowToPlayerOffset;
                    _isPLayerTooClose = true;
                }
            }

            // update pointer
            _movingPointer = _player.position + target2Player.normalized * (ArrowToPlayerOffset + _currentPath);
            
            // update smoother
            transform.position = Vector3.SmoothDamp(
                transform.position, _movingPointer,
                ref _velocity, DumpingFactor);
        }

        void OnDrawGizmos()
        {
            if (_player == null || _target == null)
                return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_player.transform.position, _target.position);
            Gizmos.DrawWireSphere(
                _player.transform.position + (_target.position - _player.transform.position).normalized * ArrowToPlayerOffset, 0.2f);
            Gizmos.DrawWireSphere(
                _target.position + (_player.position-_target.position).normalized * TargetOffset, 0.3f);

            if(_isPLayerTooClose)
                Gizmos.color = Color.red;
            Gizmos.DrawSphere(_movingPointer, 0.5f);
        }
    }
}