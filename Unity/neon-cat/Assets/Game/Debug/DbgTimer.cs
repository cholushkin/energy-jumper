// using System;
// using Events;
// using Game.Level.Entities;
// using UnityEngine;
//
// // todo: stopwatch that adds time to the button itself and to the log
// public class DbgTimer : Pane
//     , IHandle<PortalControllerBase.EventPortalSpawned>
//     , IHandle<PortalOut.EventSpawnPlayer>
//     , IHandle<ScreenHome.EventEnterHomeScreen>
// {
//     private enum State
//     {
//         Active,
//         NonActive
//     }
//
//     private State _state;
//     private float _startTime;
//
//     public override void InitializeState()
//     {
//         base.InitializeState();
//         GlobalEventAggregator.EventAggregator.Subscribe(this);
//         _state = State.NonActive;
//         SetText("timer");
//     }
//
//     public override void Update()
//     {
//         if (_state == State.Active)
//         {
//             TimeSpan t = TimeSpan.FromSeconds(Time.time - _startTime);
//             string formatedTime = t.ToString(@"hh\:mm\:ss");
//             SetText($"play time:\n {formatedTime}");
//         }
//     }
//
//     public override void OnClick()
//     {
//         TimeSpan t = TimeSpan.FromSeconds(Time.time - _startTime);
//         string formatedTime = t.ToString(@"hh\:mm\:ss");
//         Debug.Log(formatedTime);
//     }
//
//     public void Handle(PortalControllerBase.EventPortalSpawned message)
//     {
//         if (!message.Portal.gameObject.CompareTag("Finish"))
//             return;
//         
//         _state = State.NonActive;
//     }
//
//     public void Handle(PortalOut.EventSpawnPlayer message)
//     {
//         _startTime = Time.time;
//         _state = State.Active;
//         SetText("timer");
//     }
//
//     public void Handle(ScreenHome.EventEnterHomeScreen message)
//     {
//         _state = State.NonActive;
//     }
// }
