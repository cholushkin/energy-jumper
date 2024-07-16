// using Game;
// using IngameDebugConsole;
// using UnityEngine;
//
// public class DbgGiveCoins : Pane
// {
//     public int CoinIncrement;
//     public override void InitializeState()
//     {
//         base.InitializeState();
//         SetText($"Give +${CoinIncrement}");
//     }
//
//     public override void OnClick()
//     {
//         GiveCoins(CoinIncrement);
//     }
//
//     private static  void GiveToGameState(int coins)
//     {
//         Debug.Log($"Give coins at Game Session: +{coins}");
//         StateGameplay.Instance.ReceiveCoins(coins, true);
//     }
//
//     private static void GiveToHomeState(int coins)
//     {
//         Debug.Log($"Giving coins at Home: +{coins}");
//         StateHome.Instance.ReceiveCoins(coins, true);
//     }
//
//     [ConsoleMethod("coins.give", "Gives coins to local or global state based on calling context")]
//     public static void GiveCoins(int coins)
//     {
//         if (AppStateManager.Instance.IsCurrentState(StateGameplay.Instance)) // get current context
//             GiveToGameState(coins);
//         if (AppStateManager.Instance.IsCurrentState(StateHome.Instance))
//             GiveToHomeState(coins);
//     }
// }
