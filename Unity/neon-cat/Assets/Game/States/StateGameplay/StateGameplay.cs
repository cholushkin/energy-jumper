using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Events;
using Game.Level.Entities;
using GameGUI;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class StateGameplay : AppStateManager.AppState<StateGameplay>,
        IHandle<PortalOut.EventSpawnPlayer>,
        IHandle<StateGameplay.EventGameOver>,
        IHandle<CaptureMagnet.EventGotPickup>,
        IHandle<PlayerController.EventDeath>,
        IHandle<PortalSuckIn.EventPlayerExitLevel>,
        IHandle<HostageController.EventHostageRescued>
    {
        public class EventSpendCoinsSession
        {
            public SessionData SessionData;
            public int Spent; // how much coins spent
        }

        public class EventReceiveCoinsSession
        {
            public SessionData SessionData;
            public int Received;
            public bool DevAction; // is it debug coins top up
        }

        public class EventGameOver // Spawned when game over is registered
        {
        }

        [Serializable]
        public class SessionData
        {
            public SessionData(int entryCoins)
            {
                CoinsExpedition = entryCoins;
            }

            // coins related 
            public int CoinsExpedition; // coins took for expedition
            public int CoinsCollected; // got from coins on the level, cheats, rewards during gameplay
            public int CoinsSpent; // all spent coins during gameplay session

            // info variables
            public string LevelName; // name of level prefab and name in spreadsheet
            public int LevelID; // index of the level in LevelManager level order or max reached level for survival mode
            public int CoinsLocated; // coins located on the level 
            public int JumpsCounter; // number of jumps player did
            public int DiesCounter; // how many times player died
            public int RechargeBoosterCounter; // how many times play use recharge booster
            public bool HasCheats; // does player use debug features due to the session
            public int HostagesRemained;

            public void SpendCoins(int spent)
            {
                CoinsSpent += spent;
            }
            public void CollectCoins(int collected)
            {
                CoinsCollected += collected;
            }

            public bool IsEnoughCoins(int coins)
            {
                return GetCoins() >= coins;
            }

            public int GetCoins() => CoinsExpedition + CoinsCollected - CoinsSpent;
        }
        
        public enum State
        {
            Undefined,
            Loading, // create stuff, initialize
            Starting, // gui appearing, spawn of hero ( goes with some delay after level loading)
            Gameplay,
            Cutscene, // for recovering, camera flights, etc.
            Death,
        }

        public enum GameMode
        {
            SurvivalA,
            SurvivalB,
            SurvivalC,
            Regular
        }

        #region Exposed toinspector ###########################################
        public HomeIslandController HomeIslandController;
        public SimpleGUI MainWindowSystem;
        public SimpleGUI OverlayWindowSystem;
        public ScreenGameHUD GameHUD;
        public ScreenBubbleDialog ScreenBubbleDialog;
        public DragControl DragControl; // we need it for instantiating rings during level loading for the first time
        #endregion


        // assigned at runtime
        public int EntryCoins { get; set; }
        public GameObject LevelPrefab { get; set; } // Prefab of the level to load
        public Level.Level Level { get; private set; } // Current active level
        public int LevelID { get; set; } // Id of the level in level manager
        public PlayerController Player { get; private set; }
        public SessionData Session { get; private set; } // Current game session data
        public GameMode CurrentGameMode { get; set; }

        private StateMachine<State> _stateMachine;
        private int _currentSession; // Session number since application start
        private Stack<float> _timeScaleStack = new Stack<float>(4);
        private bool _isPaused;


        #region Behaviour #####################################################
        public void Update()
        {
            _stateMachine.Update();
        }
        #endregion

        #region AppStates #####################################################

        public override void AppStateInitialization()
        {
            base.AppStateInitialization();
            _stateMachine = new StateMachine<State>(this, State.Undefined);
            // DebugLogConsole.AddCommand("levels.current", "Prints a list of registered levels", () =>
            // {
            //     Debug.Log($"Current level index: {LevelID}");
            //     Debug.Log($"Current level prefab: { (LevelPrefab == null ? "null" : LevelPrefab.name)}");
            // });
        }

        public override void AppStateEnter()
        {
            if (LogChecker.Verbose() && LogChecker.IsFilterPass())
                Debug.LogFormat("Enter game state '{0}'", GetName());
            GlobalEventAggregator.EventAggregator.Subscribe(this);
            HomeIslandController?.SetEnabled(false);
            _stateMachine.GoTo(State.Loading);
        }

        public override void AppStateLeave()
        {
            GlobalEventAggregator.EventAggregator.Unsubscribe(this);
            MainWindowSystem.PopAll();
            OverlayWindowSystem.PopAll();
            DestroyLevel();
            InputHandler.Instance.EnablePlayerInputs(false);
            InputHandler.Instance.DisableActionMap();

            if (LogChecker.Verbose() && LogChecker.IsFilterPass())
                Debug.LogFormat("Exit game state '{0}'", GetName());
        }
        #endregion

        #region Event handling ################################################
        public void Handle(PortalOut.EventSpawnPlayer message)
        {
            // let camera follow the player
            Player = message.SpawnedPlayer.GetComponent<PlayerController>();

            // set arrow manager to show to the exit
            // todo:
            // GameplayAccessors.ArrowManager.AddArrow2d(GameplayAccessors.ExitPortal.transform, ArrowManager.ArrowStyle.Std);
        }
        
        public void Handle(EventGameOver message)
        {
            StateGameplay.Instance.SetPause(true);
            MainWindowSystem.PushScreen<PopupGameOver>("Popup.GameOver", gameOverPopup =>
            {
                gameOverPopup.SetMode(PopupGameOver.Mode.GameOver, StateGameplay.Instance.CurrentGameMode);
            });
        }

        public void Handle(PortalSuckIn.EventPlayerExitLevel message)
        {
            StartCoroutine(ClosingLevelCoroutine(message.Portal));
        }

        public void Handle(CaptureMagnet.EventGotPickup message) // need to update session
        {
            if (message.PickableItem.NodeType == NodeType.Coin)
                ReceiveCoins(message.PickableItem.Quantity, false);
        }
        public void Handle(HostageController.EventHostageRescued message)
        {
            Session.HostagesRemained--;
        }
        public void Handle(PlayerController.EventDeath message)
        {
            // sfx: play death generic sound

            if (message.Reason == PlayerController.EventDeath.DeathReason.PilotDeath || message.Reason == PlayerController.EventDeath.DeathReason.Crushed)
            {
                // just wait for delay and show game over
                const float waitAfterDuration = 4f;
                DOVirtual.DelayedCall(waitAfterDuration, ()=>GlobalEventAggregator.EventAggregator.Publish(new EventGameOver()));
                return;
            }
            else if (message.Reason == PlayerController.EventDeath.DeathReason.DeadlyWater)
            {
                const float waitAfterDuration = 2f;
                DOVirtual.DelayedCall(waitAfterDuration, () => GlobalEventAggregator.EventAggregator.Publish(new EventGameOver()));
                return;
            }
        }
        #endregion

        #region StateMachine ##################################################
        private void OnEnterLoading()
        {
            if (LogChecker.Verbose() && LogChecker.IsFilterPass())
                Debug.Log("StateGameplay.OnEnterLoading");

            StartCoroutine(LoadingCoroutine());
        }

        private void OnEnterStarting()
        {
            OverlayWindowSystem.PushScreen("Screen.GameplayOverlay"); // should be enabled before Screen.GameplayHUD. GameplayHUD send event EventEnterScreenGameHUD and Screen.GameplayOverlay must properly respond on it
            MainWindowSystem.PushScreen("Screen.GameplayHUD"); // note: we push Screen.GameplayHUD after we instiated a player
            GameCamera.Instance.Follow(Player.transform);
        }
        #endregion

        #region StateGameplay API and implementation ##########################
        public void SetPause(bool flag, bool resetAndIgnoreStack = false)
        {
            Debug.Log($"Set pause:{flag}");
            _isPaused = flag;
            if (resetAndIgnoreStack)
            {
                Time.timeScale = flag ? 0f : 1f;
                _timeScaleStack.Clear();
                return;
            }

            if (flag) // set pause
            {
                Debug.Log($">Push:{Time.timeScale}");
                _timeScaleStack.Push(Time.timeScale);
                Time.timeScale = 0f;
            }
            else
            {
                Debug.Log($">Pop:{_timeScaleStack.Peek()}");
                Time.timeScale = _timeScaleStack.Pop();
            }
        }

        public bool IsInPause()
        {
            return _isPaused;
        }

        public void FocusOnPlayer()
        {
            GameCamera.Instance.Follow(Player.transform);
        }

        public void DestroyLevel()
        {
            if (Level != null)
            {
                Destroy(Level.gameObject);
                Level = null;

                // on level destroyed
                {
                    var hostageBubbles = ScreenBubbleDialog.FindObjectsOfType<HostageBubble>(true);
                    foreach (var hostageBubble in hostageBubbles)
                        Destroy(hostageBubble.gameObject);
                }
            }
        }

        public void ReturnToHomeState()
        {
            DestroyLevel();
            SetPause(false, true);
            ScreenTransitionEffects.Instance.PlayEffect("ColorFadeHide", () =>
            {
                AppStateManager.Instance.Start(StateHome.Instance);
            });
        }

        public void RestartState()
        {
            DestroyLevel();
            SetPause(false, true);
            ScreenTransitionEffects.Instance.PlayEffect("ShipWipeHideLeftVariant", () =>
            {
                AppStateManager.Instance.Start(this);
            });
        }

        public bool IsEnoughCoins(int coins)
        {
            return Session.IsEnoughCoins(coins);
        }

        public void ReceiveCoins(int received, bool devTopUp)
        {
            Session.CollectCoins(received);
            Debug.Log($"Session received coins: {received}. Remaining: {Session.GetCoins()}. Dev: {Session.HasCheats}");
            if (devTopUp)
                Session.HasCheats = true;
            GlobalEventAggregator.EventAggregator.Publish(new EventReceiveCoinsSession{ SessionData = Session, Received = received, DevAction = devTopUp});
        }

        public void SpendCoins(int spent)
        {
            Assert.IsTrue(Session.IsEnoughCoins(spent));
            Session.SpendCoins(spent);
            Debug.Log($"Session SpendCoins: {spent}. Remaining: {Session.GetCoins()}");
            GlobalEventAggregator.EventAggregator.Publish(new EventSpendCoinsSession { SessionData = Session, Spent = spent});
        }

        private void CreateNewSession()
        {
            ++_currentSession;
            Session = new SessionData(EntryCoins);
        }

        // Create, initialize, assign Level
        private void CreateNewLevel(GameObject levelPrefab)
        {
            Session.LevelID = LevelID;
            Session.LevelName = levelPrefab.name;

            var hostages = levelPrefab.GetComponentsInChildren<HostageController>();
            Session.HostagesRemained = hostages.Length;

            var pickables = levelPrefab.GetComponentsInChildren<Pickable>();
            Session.CoinsLocated = pickables.Count(x => x.NodeType == NodeType.Coin);
            print($"Coins located on the level {Session.CoinsLocated}");

            var level = Instantiate(levelPrefab);
            level.name = levelPrefab.name;
            level.SetActive(true);
            Level = level.GetComponent<Level.Level>();

            DragControl?.CreateSegments();

            var levelSettings = Level.GetComponent<LevelEnvironmentSettings>();
            Assert.IsNotNull(levelSettings, "There must be LevelSettings attached to the level");
            levelSettings.Apply();
        }

        IEnumerator ClosingLevelCoroutine(PortalSuckIn exitPortal)
        {
            // todo: camera effect lens distortion
            //EffectManager.Instance.LensDistortion
            yield return new WaitUntil(() => exitPortal == null);
            MainWindowSystem.PushScreen("Popup.LevelResults");
        }

        IEnumerator LoadingCoroutine()
        {
            if (Level != null)
                Destroy(Level.gameObject);
            CreateNewSession();
            GC.Collect();
            yield return null;

            CameraControllerManager.Instance.SetCurrentCamera("GameCamera");

            Assert.IsNotNull(LevelPrefab);
            CreateNewLevel(LevelPrefab);
            Assert.IsNotNull(Level);

            // if level is generator then wait for the first generation wave completed
            if (Level is LevelGenerator levGen)
                yield return new WaitUntil(() => levGen.IsFirstGenerationComplete());

            if (ScreenTransitionEffects.Instance.LastEffectPlayed == "ShipWipeHideLeftVariant")
                ScreenTransitionEffects.Instance.PlayEffect("ShipWipeRevealLeftVariant", null);
            else
                ScreenTransitionEffects.Instance.PlayEffect("ShipWipeReveal", null);
            GameCamera.Instance.Focus(Level.StartPoint.position, true);

            const float playerSpawnDelay = 1f;
            yield return new WaitForSeconds(playerSpawnDelay);

            // spawn player
            var playerPrefab = GamePrefabs.Instance.GameEntities["Player"].gameObject;
            Assert.IsNotNull(playerPrefab);
            Level.StartPoint.GetComponent<PortalOut>().SpawnQueue
                .AddItemPrefab(playerPrefab, Vector3.zero, 1f);

            yield return new WaitUntil(() => Player != null);

            // enable input
            InputHandler.Instance.SwitchCurrentActionMap("PlayerGameplay");
            InputHandler.Instance.EnablePlayerInputs(true);

            _stateMachine.GoTo(State.Starting);
        }
        #endregion

    }
}