using System;
using System.Collections.Generic;
using System.Linq;
using GameLib.Alg;
using GameLib.Log;
using UnityEngine;
using UnityEngine.Assertions;

public interface IIntentExecutor
{
}

public interface IIntentExecutor<TIntent> : IIntentExecutor
{
    void OnIntent(TIntent message);
}

// Allows apply intention to destination objects in 2 different ways
// Way 1. Using IIntentExecutor - target objects should implement OnIntent of appropriate intention type
// Way 2. By applying intention to object via     protected virtual void DoIntention(GameObject destObject)

public abstract class IntentBase : MonoBehaviour
{
    public bool UseIntentExecutorOnTargetObjects; // alternative way to apply intention

    [Tooltip("All targets to apply this intention to. If empty - use current game object as a target")]
    public List<GameObject> DestinationObjects;

    [Tooltip("-1 = infinite number of executions")]
    public int MaxIntentExecution = -1;

    public LogChecker Log = new LogChecker(LogChecker.Level.Disabled);
    private int _executionCounter;

    public virtual void Awake()
    {
        Assert.IsTrue(MaxIntentExecution != 0, $" {transform.GetDebugName()} use mute instead");

        DestinationObjects ??= new List<GameObject>();
        if (DestinationObjects.Count == 0)
            DestinationObjects.Add(gameObject);
    }

    public virtual void Apply() // apply intent to objects
    {
        if (Log.Normal())
            Debug.Log("Apply intent " + GetType().Name);
        
        if (MaxIntentExecution != -1)
            if (_executionCounter >= MaxIntentExecution)
            {
                if (Log.Verbose())
                    Debug.Log("Ignoring apply intent due to MaxIntentExecution restriction");
                return;
            }

        ++_executionCounter;

        foreach (var destinationObject in DestinationObjects)
        {
            if(UseIntentExecutorOnTargetObjects)
                ExecuteOnIntentionDestinationObject(destinationObject);
            DoIntention(destinationObject);
        }
    }

    public virtual void DoIntention(GameObject destObject)
    {
    }

    private void ExecuteOnIntentionDestinationObject(GameObject targetGameObject)
    {
        var executors = targetGameObject.GetComponentsInChildren<IIntentExecutor>(true); // get all intent executors from target object including children
        foreach (var intentExecutor in executors)
        {
            var interfaces = intentExecutor.GetType().GetInterfaces()
                .Where(x => typeof(IIntentExecutor).IsAssignableFrom(x) && x.IsGenericType);

            // call all OnIntent<CurrentIntentionType>
            foreach (var @interface in interfaces)
            {
                var type = @interface.GetGenericArguments()[0];
                if (type == this.GetType())
                {
                    var method = @interface.GetMethod("OnIntent");
                    method.Invoke(intentExecutor, new object[]
                    {
                            Convert.ChangeType(this, type)
                    });
                }
            }
        }
    }
}
