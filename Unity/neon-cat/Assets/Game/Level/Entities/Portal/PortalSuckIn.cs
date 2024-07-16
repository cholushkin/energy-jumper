using System;
using DG.Tweening;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Level.Entities
{
    public class PortalSuckIn : PortalControllerBase
    {
        public class EventPlayerExitLevel // Spawned when player sucked in portal without destination ( end level portal )
        {
            public PlayerController Player;
            public PortalSuckIn Portal;
        }

        public interface ISuckingSupport // objects that wants to work with portal should implement this
        {
            void OnStartSuckIn(Transform destination, float portalRadius, TweenCallback onFinishSuckIn);
            bool IsInSucking();
            GameObject GetGameObject();
        }

        public enum SuckingState
        {
            Waiting, // for next sucking candidate
            Sucking, // playing animation
            Recovering // cooldown recovering
        }


        [Tooltip("Cooldown between next object suck in ")]
        public float CoolDown;
        public const  float SuckingDuration = 2f; // animation
        public float StayInCenterDuration; // how long to stay in center to get suck in


        public Transform[] LinkedPortals;
        private Chooser<Transform> _outPortalChooser;

        public SuckingState CurSuckingState { get; set; }
        private float _cooldownRemaining;

        public override void Awake()
        {
            base.Awake();
            _outPortalChooser = new Chooser<Transform>(LinkedPortals, CyclerType.CyclerRand, -1);
        }

        void Update()
        {
            if (CurrentState != State.Working)
                return;

            if (CurSuckingState == SuckingState.Recovering)
            {
                _cooldownRemaining -= Time.deltaTime;
                if (_cooldownRemaining <= 0f)
                    CurSuckingState = SuckingState.Waiting;
                return;
            }

            if (CurSuckingState == SuckingState.Waiting)
            {
                var candidate = ForceField.ZoneAnalyzer.GetFirstStayInZoneMoreThan( StayInCenterDuration, true);
                if (candidate != null)
                {
                    var agent = candidate.gameObject.GetComponent<ISuckingSupport>();
                    Assert.IsNotNull(agent, "Object doesn't support sucking in portal, but got accepted by ZoneAnalyzer of portal");
                    StartSucking(agent);
                    CurSuckingState = SuckingState.Sucking;
                }
            }
        }

        private void StartSucking(ISuckingSupport agent)
        {
            Assert.IsNotNull(agent);
            agent.OnStartSuckIn(transform, GetPortalRadius(), ()=>FinishSucking(agent));
        }

        private void FinishSucking(ISuckingSupport agent)
        {
            // if we have linked portal move to there
            var connectedPortal = _outPortalChooser.GetCurrent();
            _outPortalChooser.Step();

            if (connectedPortal == null)
            {
                // if agent is Player and portal has no connected portal( exit portal)  than spawn EventPlayerExitLevel
                if (agent.GetGameObject().GetNodeTypeAndProps().Item1 == NodeType.Player)
                {
                    var player = agent.GetGameObject().GetComponent<PlayerController>();
                    Assert.IsNotNull(player);
                    GlobalEventAggregator.EventAggregator.Publish(new EventPlayerExitLevel { Player = player, Portal = this});
                }
            }
            else
            {
                connectedPortal.GetComponent<PortalOut>()
                    .SpawnQueue
                    .AddItem(agent.GetGameObject(), Vector3.zero, 1f);
            }

            _cooldownRemaining = CoolDown;
            CurSuckingState = SuckingState.Recovering;
            ProcessWorkingCounter();
        }
    }
}
