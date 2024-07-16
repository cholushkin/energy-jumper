//using DG.Tweening;
//using UnityEngine;
//using UnityEngine.Assertions;

////#define HAS_REAL_ANIMATION
//// todo del me

//public class PortalAnimationController : StateMachineBehaviour
//{
//    // controlling parameters
//    private const string TriggerNameSpawn = "Spawn";
//    private const string TriggerNameDespawn = "Despawn";
//    private const string BooleanNameOpened = "Opened";

//    // state names
//    private const string StateNameOpening = "Opening";
//    private const string StateNameIdle = "Idle";
//    private const string StateNameSpawning = "Spawning";
//    private const string StateNameDespawning = "Despawning";
//    private const string StateNameClosing = "Closing";
//    private const string StateNameClosed = "Closed";

//    private Tween _curTween;

//#if !HAS_REAL_ANIMATION
//    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//    {
//        var target = animator.gameObject;
//        var duration = stateInfo.length;
//        Assert.IsNotNull(target);

//        Debug.LogFormat("s:{0} sm:{1} dur:{2}", stateInfo.speed, stateInfo.speedMultiplier, duration);

//        if (stateInfo.IsName(StateNameOpening))
//        {
//            Assert.IsFalse(stateInfo.loop);
//            if (_curTween != null)
//                _curTween.Kill();
//            _curTween = DOTween.Sequence()
//                .Append(target.transform.DOScale(Vector3.one, duration * 0.3f))
//                .Append(
//                    target.transform.DOScale(Vector3.one * 1.5f, duration * 0.7f)
//                        .SetEase(Ease.InOutCubic)
//                        .SetLoops(2, LoopType.Yoyo));
//        }
//        else if (stateInfo.IsName(StateNameIdle))
//        {
//            Assert.IsTrue(stateInfo.loop);
//            if (_curTween != null)
//                _curTween.Kill();
//            target.transform.localScale = Vector3.one;
//            _curTween = target.transform.DOScale(Vector3.one * 1.5f, duration)
//                .From()
//                .SetEase(Ease.InOutElastic)
//                .SetLoops(-1, LoopType.Yoyo);
//        }
//        else if (stateInfo.IsName(StateNameSpawning))
//        {
//        }
//    }
//#endif



//    //public void PlayAnimation(Animation anim)
//    //{
//    //    Animator.SetBool("IsOpening", true);

//    //    _currentAnimation = anim;

//    //    //if (anim == Animation.Opening)
//    //    //{
//    //    //    DOTween.Sequence()
//    //    //        .Append(transform.DOScale(Vector3.one, 0.5f))
//    //    //        .Append(
//    //    //            transform.DOScale(Vector3.one * 1.5f, OpeningDuration)
//    //    //            .SetEase(Ease.InOutCubic)
//    //    //            .SetLoops(2, LoopType.Yoyo))
//    //    //        .OnComplete(() => PlayAnimation(Animation.Idle));
//    //    //}
//    //    //else if (anim == Animation.Idle)
//    //    //{
//    //    //    transform.DOScale(Vector3.one * 1.15f, Random.Range(2, 5))
//    //    //        .SetEase(Ease.InOutElastic)
//    //    //        .SetLoops(-1, LoopType.Yoyo);
//    //    //}
//    //    //else if (anim == Animation.Closing)
//    //    //{
//    //    //    transform.DOScale(Vector3.zero, 1f)
//    //    //        .SetEase(Ease.InOutQuint)
//    //    //        .OnComplete(() => PlayAnimation(Animation.Closed));
//    //    //}
//    //}
//}
