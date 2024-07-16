using System.Collections.Generic;
using GameLib;
using GameLib.Alg;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI
{
    // controls:
    // * lifetime
    // * creation
    // * global modes (hide-all, remind)
    public class ArrowManager : Singleton<ArrowManager>
    {
        public enum Mode
        {
            Targeting, // show all arrows
            PauseAndHide, // hide all active arrows for a while
            Remind, // show appearing animation again and captions for a while
        }

        public enum ArrowStyle
        {
            Std,
            Tutorial,
            Portal,
            DangerZone
        }

        public GameObject PrefabArrowStd;
        public PlayerController Player;
        public Camera Camera;
        public Canvas Canvas;


        private List<ArrowCanvas> _arrows;
        private StateMachine<Mode> _stateMachine;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new StateMachine<Mode>(this, Mode.Targeting);
            _stateMachine.GoTo(Mode.Targeting);
        }

        void Update()
        {
            _stateMachine.Update();

            // control life time of arrows
            //foreach (var arrow in _arrows)
            //{
            //    if (arrow.IsLifetimeEnded())
            //    {
            //        RemoveArrow(arrow);
            //    }
            //    // todo: purge Items with dead target
            //}
        }

        //void OnDestroy()
        //{
        //    foreach (var arrow in _arrows)
        //        Destroy(arrow.gameObject);
        //}


        //public void AddArrow(Transform target, ArrowStyle style, float duration = -1f)
        //{
        //    Assert.IsNotNull(target);

        //    // cretate _arrows
        //    if (_arrows == null)
        //        _arrows = new List<Arrow>();

        //    // create and initialize arrow object
        //    var arrowPrefab = _arrowStyleToPrefab(style);
        //    Assert.IsNotNull(arrowPrefab);


        //    var arrowObj = Instantiate(PrefabArrowStd, Player.transform.position, Quaternion.identity);
        //    arrowObj.transform.SetParent(transform);

        //    var arrow = arrowObj.GetComponent<Arrow>();
        //    Assert.IsNotNull(arrow);
        //    arrow.Set(Player.transform, target, duration, duration, Camera);

        //    var playAppearingAnimation = _stateMachine.CurrentState.State == Mode.Targeting
        //                                 || _stateMachine.CurrentState.State == Mode.Remind;

        //    if (playAppearingAnimation)
        //    {
        //        arrow.EnableFollowing(true);
        //        arrow.Appear();
        //    }

        //    _arrows.Add(arrow);
        //}

        public void AddArrow2d(Transform target, ArrowStyle style, float duration = -1f)
        {
            Assert.IsNotNull(target);

            // cretate _arrows
            if (_arrows == null)
                _arrows = new List<ArrowCanvas>();

            // create and initialize arrow object
            var arrowPrefab = _arrowStyleToPrefab(style);
            Assert.IsNotNull(arrowPrefab);


            var arrowObj = Instantiate(PrefabArrowStd);
            arrowObj.transform.SetParent(Canvas.transform);

            var arrow = arrowObj.GetComponent<ArrowCanvas>();
            Assert.IsNotNull(arrow);
            arrow.Set(Player.transform, target, duration, duration, Camera);


            var playAppearingAnimation = _stateMachine.CurrentState.State == Mode.Targeting
                                         || _stateMachine.CurrentState.State == Mode.Remind;

            if (playAppearingAnimation)
            {
                //arrow.EnableFollowing(true);
                //arrow.Appear();
            }

            _arrows.Add(arrow);
        }

        public void RemoveAll()
        {
            if (_arrows == null || _arrows.Count == 0)
                return;
            foreach (var arrow in _arrows)
                Destroy(arrow.gameObject);
            _arrows = null;
        }

        //public void RemoveArrow(Arrow arrow)
        //{
        //    Assert.IsNotNull(arrow);
        //    _arrows.Remove(arrow);
        //    arrow.Die();
        //}

        public void HideAndPause()
        {
            _stateMachine.GoTo(Mode.PauseAndHide);
        }

        public void ShowAndContinue()
        {
            Assert.IsTrue(_stateMachine.CurrentState.State == Mode.PauseAndHide);
            if (_stateMachine.CurrentState.State == Mode.PauseAndHide)
                _stateMachine.GoTo(Mode.Remind);
        }

        #region ---------------------- StateMachine
        void OnEnterTargeting()
        {

        }

        void OnUpdateTargeting()
        {
            _processLifetime();
        }

        #endregion



        private void _processLifetime()
        {
            //_arrows.RemoveAll(x => x.IsAlive() == false || x.Target == null);
            //for (int i = _arrows.Count - 1; i >= 0; i--)
            //{
            //    var arrow = _arrows[i];
            //    Assert.IsNotNull(arrow);
            //    if (!arrow.IsAlive())
            //    {
            //        arrow.Die();
            //        _arrows.RemoveAt(i);
            //    }
            //}
        }





        // ---------------- helpers
        GameObject _arrowStyleToPrefab(ArrowStyle style)
        {
            // todo: implement me
            return PrefabArrowStd;
        }


    }
}
