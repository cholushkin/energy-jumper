using DG.Tweening;
using Events;
using Game.Input;
using Game.Level.Entities;
using Game.VFX;
using GameLib.Alg;
using GameLib.Log;
using LeakyAbstraction;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Game
{
    [SelectionBase]
    public class PlayerController 
        : Node
        , PortalSuckIn.ISuckingSupport
        , PortalOut.IOutingPortalSupport
        , IHandle<InputHandler.EventInputFreezeRelease>
        , IHandle<InputHandler.EventInputFreezeStart>
        , IHandle<PlayerController.EventDeath>
        , IHandle<CaptureMagnet.EventGotPickup>
    {
        public enum State
        {
            Created,
            Spawning,
            Disappearing,
            Disappeared,
            TimeFrozen,
            TimeNormal, // alive and in normal time
            Dead
        }

        public class EventDeath // Spawned when death of character registered
        {
            public enum DeathReason
            {
                DeadlyWater, // fall in water
                PilotDeath, // gas, energy hit
                Crushed, // squished by rigid body, hit by high energy object, explosion
            }
            public PlayerController Player;
            public DeathReason Reason;
            public GameObject ReasonObject;
        }
        

        // exposed to Inspector
        // -------------------------------
        #region Inpsector
        public Rigidbody Rigidbody;
        public PlayerAudioController AudioController;
        public PlayerVisualController VisualController;
        public ImpulseJumper ImpulseJumper;
        public PhysicsTouchAnalizer PhysicsTouchAnalizer;
        public PlayerTimeController TimeController;
        public Roll RollController;
        public CaptureMagnet CaptureMagnet;
        public AntiGravityRayDevice AntiGravityRayDevice;
        public float ViewDistance;
        public LogChecker Log;
        #endregion



        private StateMachineImmediate<State> _stateMachine;
        private int _bgLayerMask;
        private GameObject _currentEnergyZone;
        private GameObject _prevEnergyZone;
        private bool _isEnergyOn;
        private TweenCallback _finishSpawningCallback;
        private TweenCallback _finishDisappearing;
        private Transform _suckingCenter;
        private float _suckingPortalRadius;
        private float _rechargeCooldown;
        private bool _jumpInHold; // waiting for get energy (holding jump button) to stop the time and do jump
        private const float RechargeCooldown = 0.1f;
        private EventDeath.DeathReason _deathReason;

        private ForceVisualizer _forceVisualizer;


        #region MonoBehaviour
        // ------------------------------------------------
        public void Awake()
        {
            _bgLayerMask = LayerMask.GetMask("BG");
            _forceVisualizer = GetComponentInChildren<ForceVisualizer>();

            Assert.IsNotNull(Rigidbody);
            Assert.IsNotNull(AudioController);
            Assert.IsNotNull(VisualController);
            Assert.IsNotNull(ImpulseJumper);
            Assert.IsNotNull(TimeController);

            VisualController.Init();

            _stateMachine = new StateMachineImmediate<State>(this, State.Created);
            _stateMachine.GoTo(State.Created);

            EnablePhysics(false);
            EnableEnergy(false);
            _rechargeCooldown = RechargeCooldown;
            VisualController.ScaleToZero();

            GlobalEventAggregator.EventAggregator.Subscribe(this, 256); // last object on a frame to receive all events
        }

        void Start()
        {
            var playerBodyCollider = GetComponent<SphereCollider>();

            // initialize magnet 
            CaptureMagnet.ForceField.MagneticOppositeForceReceiver = Rigidbody;
            CaptureMagnet.SetIgnoreCollision(playerBodyCollider);

            // initialize anti gravity device
            AntiGravityRayDevice.SetIgnoreCollision(playerBodyCollider);

            VisualController.AimingArrow.SetState(AimingArrow.State.Disabled);

            PhysicsTouchAnalizer.OnEnter += OnPlayerRigidTouch;
            PhysicsTouchAnalizer.OnPenetrated += OnPlayerPenetrated;
        }

        private void OnPlayerRigidTouch(CollisionAnalyzerBase.Entry zoneEntry)
        {
            // if we touch charge surface and we are not charged then charge player and animate surface charge
            if(IsEnergyEnabled())
                return;
            var node = zoneEntry.Node;
            if (CollisionAnalyzerHelper.FilterPass(NodeType.Undefined, NodeProps.ChargeSurface,
                NodePropsFilteringMode.AND, node))
            {
                var surfaceCooldown = node.gameObject.AddSingleComponentSafe<PowerSupplyAndSwitcherChargeSurface>(out var wasAdded);
                if (wasAdded)
                    surfaceCooldown.InitDefault();
                surfaceCooldown.ToggleSwitch(false);
                Assert.IsTrue(!IsEnergyEnabled());
                EnableEnergy(true);
            }
        }

        private void OnPlayerPenetrated(GameObject other)
        {
            PhysicsTouchAnalizer.OnPenetrated -= OnPlayerPenetrated;
            GlobalEventAggregator.EventAggregator.Publish(new PlayerController.EventDeath { Player = this, Reason = PlayerController.EventDeath.DeathReason.Crushed, ReasonObject = other });
        }

        void Update()
        {
            return;
            _stateMachine.Update();

            // update look on intention vector
            if (!AntiGravityRayDevice.HasAnyRay())
            {
                GameCamera.Instance.Follower.Offset = InputHandler.Instance.IntentionVector * ViewDistance;
            }

            // update head rotation in direction of movement
            //if (state == State.TimeNormal)
            {
                VisualController.ShiftLookPoint(Rigidbody.linearVelocity);
            }
        }

        void FixedUpdate()
        {
            if (GetState() == State.TimeNormal)
                UpdateBattery();
        }

        void OnCollisionEnter(Collision collision)
        {
            // play touch sfx, dust vfx, camera shake, gamepad vibration
            if (collision.impulse.magnitude > 1f)
            {
                var volFactor = Mathf.Clamp(collision.impulse.magnitude, 1f, 26f) / 26f;

                InputHandler.Instance.PlayHitVibration(volFactor);

                VFXManager.Instance.SpawnVFX("vfx_fallback", transform.position);
                if (volFactor > 0.01f)
                {
                    SoundManager.Instance.PlaySound(GameSound.SfxShipHitObstacle, volFactor, 1f);
                    GameCamera.Instance.Shake(1f, volFactor, collision.impulse.normalized, null);
                }
            }
        }
        #endregion

        #region PlayerController interface
        // -----------------------------------
        public void EnablePhysics(bool flag)
        {
            Rigidbody.detectCollisions = flag;
            Rigidbody.useGravity = flag;
            Rigidbody.linearVelocity = Vector3.zero;
            VisualController.SetCollidersEnabled(flag);
        }

        public bool IsPhysicsEnabled()
        {
            return Rigidbody.detectCollisions;
        }

        public void EnableEnergy(bool flag)
        {
            var state = GetState();
            var acceptableToEnable = (state == State.TimeFrozen || state == State.TimeNormal || state == State.Created);
            if(flag && !acceptableToEnable)
                return;
            if(flag)
                VFXManager.Instance.SpawnVFX("vfx_recharge", transform.position);
            VisualController.SetCharged(flag);
            _isEnergyOn = flag;
        }

        public bool IsEnergyEnabled()
        {
            return _isEnergyOn;
        }

        public void Die()
        {
            _stateMachine.GoToIfNotInState(State.Dead);
        }
        #endregion

        #region Implementation
        // -----------------------------------
        void UpdateBattery()
        {
            _rechargeCooldown += Time.deltaTime;
            if (_rechargeCooldown < RechargeCooldown)
                return;

            // inside energy zone
            if (_currentEnergyZone != null && !IsEnergyEnabled())
            {
                EnableEnergy(true);
                return;
            }

            if (PhysicsTouchAnalizer.IsInCollision(NodeType.Undefined, NodeProps.ChargeSurface, NodePropsFilteringMode.AND))
            {
                if (!IsEnergyEnabled())
                {
                    EnableEnergy(true);
                }
                return;
            }

            if (PhysicsTouchAnalizer.IsInCollision(NodeType.Undefined, NodeProps.DischargeSurface, NodePropsFilteringMode.AND))
            {
                if (IsEnergyEnabled())
                {
                    SoundManager.Instance.PlaySound(GameSound.SfxSurfaceDischarge);
                    EnableEnergy(false);
                }
                return;
            }
        }

        private void ProcessEnergyZoneEnterExit()
        {
            var prev = _currentEnergyZone;
            _currentEnergyZone = RaycastBG(10f);
            if (_currentEnergyZone == prev)
                return;
            _prevEnergyZone = prev;
            OnEnterEnergyZone(_currentEnergyZone);
            OnExitEnergyZone(_prevEnergyZone);
            EnableEnergy(true);
        }

        void OnEnterEnergyZone(GameObject eZone)
        {
            
        }
        void OnExitEnergyZone(GameObject eZone)
        {
            
        }

        private GameObject RaycastBG(float rayLength)
        {
            RaycastHit hit;
            var hasHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength, _bgLayerMask);

            if (Log.Verbose())
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLength,
                    hasHit ? Color.red : Color.yellow);
                //Debug.LogFormat("{0}", hit.collider.gameObject.name);
            }

            if (hasHit)
            {
                var zone = hit.collider.gameObject;
                return zone;
            }
            return null;
        }

        public bool CanStopTime()
        {
            return _stateMachine.CurrentState.State == State.TimeNormal && _isEnergyOn;
        }

        public bool CanResumeTime()
        {
            return _stateMachine.CurrentState.State == State.TimeFrozen;
        }

        #region ISuckingSupport
        public void OnStartSuckIn(Transform portalCenter, float portalRadius, TweenCallback onFinishSuckIn)
        {
            _suckingCenter = portalCenter;
            _suckingPortalRadius = portalRadius;
            _finishDisappearing = onFinishSuckIn;
            _stateMachine.GoTo(State.Disappearing);
        }

        public bool IsInSucking()
        {
            return _stateMachine.CurrentState.State == State.Disappearing;
        }
        #endregion

        #region IOutingPortalSupport
        public void OnStartOut(TweenCallback onFinishCallback)
        {
            _finishSpawningCallback = onFinishCallback;
            _stateMachine.GoTo(State.Spawning);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
        #endregion

        #region Event Handling
        // -----------------------------------
        public void Handle(InputHandler.EventInputFreezeStart message)
        {
            _jumpInHold = true;
        }

        int aimingLevel;
        Vector2 aimingDirection;

        public void Handle(InputHandler.EventInputFreezeRelease message)
        {
            aimingLevel = message.AimingLevel;
            aimingDirection = message.AimingDirection;
            _jumpInHold = false;
        }

        public void Handle(EventDeath message)
        {
            _deathReason = message.Reason;
            Die();
        }

        public void Handle(CaptureMagnet.EventGotPickup message) // need to update player state
        {
            if (message.PickableItem.NodeType == NodeType.ChargePill)
                EnableEnergy(true);

            // we got this message in the last order (GlobalEventAggregator.EventAggregator.Subscribe(this, 256)) so we can dispose an object
            print($"destroy pickup");
            VFXManager.Instance.SpawnVFX("vfx_pickup", transform.position);
            Destroy(message.PickableItem.gameObject);
        }

        #endregion

        #region StateMachine
        // ----------------------------

        public State GetState()
        {
            return _stateMachine.CurrentState.State;
        }

        // spawning
        void OnEnterSpawning()
        {
            EnablePhysics(false);
            RollController.enabled = false;
            VisualController.ScaleToZero();
            VisualController.PlayAnimation(PlayerVisualController.Animation.Spawning, 0.5f);
        }

        void OnExitSpawning()
        {
            EnablePhysics(true);
            RollController.enabled = true;
            CaptureMagnet.SetEnabled(true);
            AntiGravityRayDevice.SetEnabled(true);
            _finishSpawningCallback();
        }

        void OnUpdateSpawning()
        {
            if (VisualController.GetCurrentAnimation() == PlayerVisualController.Animation.Idle)
                _stateMachine.GoTo(State.TimeNormal);
        }

        // disappearing
        
        void OnEnterDisappearing()
        {
            CaptureMagnet.SetEnabled(false);
            AntiGravityRayDevice.SetEnabled(false);

            EnablePhysics(false);

            VisualController.transform.DOMove(_suckingCenter.position, PortalSuckIn.SuckingDuration).SetEase(Ease.OutElastic);
            VisualController
                .PlayAnimation(PlayerVisualController.Animation.Despawning, PortalSuckIn.SuckingDuration * 0.9f, () =>
                {
                    EnablePhysics(false);
                    _stateMachine.GoTo(State.Disappeared);
                });
            _isEnergyOn = false;
            RollController.enabled = false;
        }

        void OnUpdateDisappearing() // note: probably needs to be on FixedUpdate somehow
        {
        }

        // disappeared
        void OnEnterDisappeared()
        {
            _finishDisappearing();
        }

        // timenormal
        void OnUpdateTimeNormal()
        {
            ProcessEnergyZoneEnterExit();


            // switch to time stop mode
            if (_jumpInHold && CanStopTime())
            {
                VisualController.AimingArrow.SetState(AimingArrow.State.Enabled);
                TimeController.Freeze();
                _stateMachine.GoTo(State.TimeFrozen);
                _forceVisualizer.EnableDots();
                SoundManager.Instance.PlaySound(GameSound.SfxTimeFreezeEnter);
            }
        }

        // timefrozen
        void OnUpdateTimeFrozen()
        {
            ProcessEnergyZoneEnterExit();

            // do jump
            if (!_jumpInHold)
            {
                VisualController.AimingArrow.SetState(AimingArrow.State.Disabled);
                TimeController.Unfreeze();
                var aimingLevel = this.aimingLevel;

                var volume = 0f;
                if (aimingLevel == 1)
                    volume = 0.3f;
                if (aimingLevel > 1)
                    volume = 1.0f;

                SoundManager.Instance.PlaySound(GameSound.SfxShipJump, volume, 1f);
                ImpulseJumper.Jump(aimingDirection, aimingLevel);

                if (_currentEnergyZone == null)
                    EnableEnergy(false);


                // get charging surface in contact and disable their charging prop for cooldown time
                if(!IsEnergyEnabled())
                {
                    var entries = PhysicsTouchAnalizer.GetEntries();
                    foreach (var entry in entries)
                    {
                        var node = entry.Value.Node;
                        if (CollisionAnalyzerHelper.FilterPass(NodeType.Undefined, NodeProps.ChargeSurface,
                            NodePropsFilteringMode.AND, node))
                        {
                            var surfaceCooldown = node.gameObject.AddSingleComponentSafe<PowerSupplyAndSwitcherChargeSurface>(out var wasAdded);
                            if (wasAdded)
                                surfaceCooldown.InitDefault();
                            surfaceCooldown.ToggleSwitch(false);
                        }
                    }
                }

                _stateMachine.GoTo(State.TimeNormal);
                _forceVisualizer.DisableDots();
                SoundManager.Instance.PlaySound(GameSound.SfxTimeFreezeExit);
            }
        }

        // dead
        void OnEnterDead()
        {
            CaptureMagnet.SetEnabled(false);
            AntiGravityRayDevice.SetEnabled(false);

            _isEnergyOn = false;
            EnableEnergy(false);
            
            
            // disable input
            InputHandler.Instance.EnablePlayerInputs(false);

            if (_deathReason == EventDeath.DeathReason.Crushed)
            {
                EnablePhysics(false);
                VisualController.PlayAnimation(PlayerVisualController.Animation.DeathDisolve, 2f);
            }

            VisualController.DisableEyes();
            // todo: VisualController disable arrow (maybe it was visible)
        }

        #endregion
        #endregion


    }
}
