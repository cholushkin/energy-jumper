using Events;
using Game;
using Game.Level;
using UnityEngine;
using UnityEngine.Assertions;

//public class TriggerOnEnterPattern : TriggerBase, IHandle<PlayerController.EventEnterPattern>
//{
//    public override void Awake()
//    {
//        base.Awake();
//        GlobalEventAggregator.EventAggregator.Subscribe(this);
//    }

//    public void Handle(PlayerController.EventEnterPattern message)
//    {
//        Debug.LogFormat("EventEnterPattern in trigger {0}", transform.GetDebugName());
//        if (message.Pattern == null)
//            return;

//        var currentPattern = gameObject.GetComponent<Node>().GetMyPattern();
//        Assert.IsNotNull(currentPattern);

//        if (currentPattern == message.Pattern)
//        {
//            Debug.LogFormat("hit, hitCounter {0}", HitCount);
//            OnHit();
//        }
//    }
//}