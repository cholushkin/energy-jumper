// using Game;
// using Game.Level;
//
// public class DbgCollectAll : Pane
// {
//     public override void InitializeState()
//     {
//         base.InitializeState();
//         SetText("Collect All");
//     }
//
//     public override void OnClick()
//     {
//         var player = StateGameplay.Instance.Player;
//         var level = StateGameplay.Instance.Level;
//         var pickables = level.transform.GetComponentsInChildren<Pickable>();
//         foreach (var pickable in pickables)
//         {
//             if (pickable.NodeType == NodeType.Coin)
//             {
//                 pickable.transform.position = player.transform.position;
//             }
//         }
//     }
// }
