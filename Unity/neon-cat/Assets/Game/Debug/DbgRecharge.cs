// using Game;
// using UnityEngine;
//
// public class DbgRecharge : Pane
// {
//     public StateGameplay Gameplay;
//
//     public override void InitializeState()
//     {
//         base.InitializeState();
//         SetText("Dev Recharge\n<size=10>hit <space></size>");
//     }
//
//     public override void OnClick()
//     {
//         RechargeChar();
//     }
//
//     public override void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             RechargeChar();
//         }
//     }
//
//     private void RechargeChar()
//     {
//         Gameplay.Player?.EnableEnergy(true);
//     }
// }
