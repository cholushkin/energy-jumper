// using Game;
// using Game.Level.Entities;
// using IngameDebugConsole;
// using UnityEngine;
//
// public class DbgWinLevel : Pane
// {
//     public override void InitializeState()
//     {
//         base.InitializeState();
//         SetText("Win level");
//     }
//
//     public override void OnClick()
//     {
//         WinLevel();
//     }
//
//     [ConsoleMethod("levels.win", "Win current level")]
//     public static void WinLevel()
//     {
//         var player = StateGameplay.Instance.Player;
//         if (player == null)
//         {
//             Debug.Log($"You need to start level first");
//             return;
//         }
//         GlobalEventAggregator.EventAggregator.Publish(new PortalSuckIn.EventPlayerExitLevel { Player = player });
//     }
//
//     [ConsoleMethod("levels.lose", "Lose current level")]
//     public static void LoseLevel()
//     {
//         var player = StateGameplay.Instance.Player;
//         if (player == null)
//         {
//             Debug.Log($"You need to start level first");
//             return;
//         }
//         GlobalEventAggregator.EventAggregator.Publish(new PlayerController.EventDeath { Player = player, Reason = PlayerController.EventDeath.DeathReason.PilotDeath });
//     }
// }
