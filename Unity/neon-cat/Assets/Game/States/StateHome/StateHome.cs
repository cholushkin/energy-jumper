using Events;
using GameGUI;
using GameLib;
using IngameDebugConsole;
using uconsole;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    
    public class StateHome : AppStateManager.AppState<StateHome>, IHandle<HomeIslandController.EventIslandUpgrade>
    {
        public class EventSpendCoinsAtHome // Spawned when user coins has been spent 
        {
            public int Spent;
        }

        public class EventReceiveCoinsAtHome // Spawned when user gain coins 
        {
            public int Received;
        }

        public SimpleGUI OverlayWindowSystem;
        public SimpleGUI MainWindowSystem;
        public LevelManager LevelManager;
        public HomeIslandController HomeIslandController;
        public HomeIslandController HomeIslandControllerPrefab;
        public ScreenHome ScreenHome;

        public override void AppStateInitialization()
        {
            base.AppStateInitialization();
            GlobalEventAggregator.EventAggregator.Subscribe(this);
        }

        public override void AppStateEnter()
        {
            InputHandler.Instance.SwitchCurrentActionMap("PlayerHome");

            // For brand new game start (no cat appeared yet):
            // * Turn off ScreenHomeOverlay ( in StateHome )
            // * Hide start button
            // * Hide levels button
            var brandNewStart = UserAccounts.Instance.GetCurrentAccount().AccountData.IslandLevel == 0;

            if (!brandNewStart)
                OverlayWindowSystem.PushScreen("Screen.HomeOverlay");

            MainWindowSystem.PushScreen<ScreenHome>("Screen.Home", screenHome =>
            {
                screenHome.SetLevelSelectionButtonEnabled(!brandNewStart);
                screenHome.SetQuickStartButtonEnabled(!brandNewStart);

                // enable survival buttons
                var account = UserAccounts.Instance.GetCurrentAccount();
                for (int i = 0; i < account.AccountData.SurvivalLevelStates.Length; ++i)
                    screenHome.SetSurvivalButton(i, account.AccountData.SurvivalLevelStates[i].LastLevelIndex);
            });
            CameraControllerManager.Instance.SetCurrentCamera("HomeCamera");
            ScreenTransitionEffects.Instance.PlayEffect("ColorFadeReveal", null);
            HomeIslandController.SetEnabled(true);
        }

        public override void AppStateLeave()
        {
        }

        public void StartGame(int entryCoins, int levelID)
        {
            StateGameplay.Instance.LevelID = levelID;
            StateGameplay.Instance.LevelPrefab = StateGameplay.Instance.CurrentGameMode == StateGameplay.GameMode.Regular ? LevelManager.GetLevel(levelID) : LevelManager.GetSurvivalLevel(StateGameplay.Instance.CurrentGameMode);
            StateGameplay.Instance.EntryCoins = entryCoins;
            ScreenTransitionEffects.Instance.PlayEffect("ShipWipeHide", OnFinishTransitionToGameplayState);
            if(LogChecker.Normal())
                Debug.Log($"Starting level {StateGameplay.Instance.LevelPrefab.name}, index:{StateGameplay.Instance.LevelID}");

        }

        private void OnFinishTransitionToGameplayState()
        {
            MainWindowSystem.PopAll();
            OverlayWindowSystem.PopAll();
            AppStateManager.Instance.Start(StateGameplay.Instance);
        }

        public void Handle(HomeIslandController.EventIslandUpgrade message)
        {
            if (message.IslandLevel == 1)
            {
                OverlayWindowSystem.PushScreen("Screen.HomeOverlay");
                ScreenHome.SetQuickStartButtonEnabled(true);
            }
        }

        public bool IsEnoughCoins(int coins)
        {
            return UserAccounts.Instance.GetCurrentAccount().AccountData.Coins >= coins;
        }


        public bool SpendCoins(int coins)
        {
            Assert.IsTrue(coins >= 0);
            if (!IsEnoughCoins(coins))
                return false;

            var account = UserAccounts.Instance.GetCurrentAccount();
            account.AccountData.Coins -= coins;
            GlobalEventAggregator.EventAggregator.Publish(new EventSpendCoinsAtHome { Spent = coins });
            if (LogChecker.Normal())
                Debug.Log($"StateHome spend coins: {coins}. Remaining: {account.AccountData.Coins}");
            return true;
        }

        public bool ReceiveCoins(int coins, bool cheat)
        {
            Assert.IsTrue(coins > 0);
            var account = UserAccounts.Instance.GetCurrentAccount();
            account.AccountData.Coins += coins;
            if (cheat)
                account.AccountData.CheatedCounter++;

            GlobalEventAggregator.EventAggregator.Publish(new EventReceiveCoinsAtHome { Received = coins });
            if (LogChecker.Normal())
                Debug.Log($"StateHome receive coins: {coins}. Remaining: {account.AccountData.Coins}");
            return true;
        }


        // [ConsoleMethod("levels.start", "Starts level")]
        // public static void DbgStartLevel(int levelID, int entryCoins = 0)
        // {
        //     StateGameplay.Instance.CurrentGameMode = StateGameplay.GameMode.Regular;
        //     Instance.StartGame(entryCoins, levelID);
        // }
        //
        // public class Instance
        // {
        // }
    }
}