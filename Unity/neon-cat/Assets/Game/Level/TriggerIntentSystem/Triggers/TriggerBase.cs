using System;
using System.Collections;
using System.Collections.Generic;
using GameLib.Alg;
using GameLib.Log;
using UnityEngine;
using UnityEngine.Assertions;

public class TriggerBase : MonoBehaviour
{
    [Serializable]
    public class IntentSequenceItem
    {
        public IntentBase Intent;
        public float Delay;
    }

    public enum HitWhileWorkingStrategy
    {
        Queue, 
        Ignore,
        SkipButCount
    }

    [Tooltip("-1 means unlimited amount")]
    public int MaxHits; // how many times trigger could hit, -1 for infinity
    public bool IsMuted; // triger stop to process hits
    public HitWhileWorkingStrategy HitMode;

    public int HitCount { get; protected set; } // current hits count

    [Tooltip("Intents that will apply in chain using delay between each of them")]
    public List<IntentSequenceItem> IntentSequence = new List<IntentSequenceItem>();

    public bool HitOnAwake;

    private int _nextIntentIndex;
    private bool _isInSequence;
    private static readonly LogChecker _log = new LogChecker(LogChecker.Level.Important);
    private int _queuedHitsCounter;

    void Reset()
    {
        MaxHits = -1;
    }

    public virtual void Awake()
    {
        Assert.IsTrue(MaxHits == -1 || MaxHits > 0);
        if (HitOnAwake)
            OnHit();
    }

    protected virtual void Update()
    {
        if (IsMuted)
            return;
        Assert.IsTrue(HitMode == HitWhileWorkingStrategy.Queue);
        if (!_isInSequence && _queuedHitsCounter > 0)
        {
            --_queuedHitsCounter;
            OnHit();
        }
    }

    [ContextMenu("Hit the trigger")]
    public void DbgHitTrigger()
    {
        OnHit();
    }

    public virtual void OnHit()
    {
        if (IsMuted)
            return;

        // update counter
        if (MaxHits != -1 && HitCount >= MaxHits)
            return; // out of trigger work count

        // intent sequence
        Assert.IsTrue(IntentSequence.Count > 0, "Trigger has no intents: " + transform.GetDebugName());

        if (_isInSequence)
        {
            Debug.LogFormat("trigger hits while still playing sequence, applying strategy '{0}'", HitMode);
            if (HitMode == HitWhileWorkingStrategy.Queue)
            {
                ++_queuedHitsCounter;
                return;
            }
            else if (HitMode == HitWhileWorkingStrategy.Ignore)
            {
                return;
            }
            else if (HitMode == HitWhileWorkingStrategy.SkipButCount)
            {
                HitCount++;
                return;
            }
        }

        if (IntentSequence.Count > 0 && !_isInSequence)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            StartCoroutine(ProcessSequence());
        }
    }

    IEnumerator ProcessSequence()
    {
        _isInSequence = true;
        HitCount++;
        for (int nextIntentIndex = _nextIntentIndex; nextIntentIndex < IntentSequence.Count; ++nextIntentIndex)
        {
            var intSeqItem = IntentSequence[nextIntentIndex];

            if (Math.Abs(intSeqItem.Delay) > 0.001f)
                yield return new WaitForSeconds(intSeqItem.Delay);

            // special sequence intents
            {
                // ---
                var interrupt = intSeqItem.Intent as IntentSequenceInterrupt;
                if (interrupt != null)
                {
                    if (_log.Verbose())
                        Debug.LogFormat("Sequence 'Interrupt' on {0}", transform.GetDebugName());
                    _nextIntentIndex = nextIntentIndex + 1;
                    break;
                }

                // ---
                var intGoto = intSeqItem.Intent as IntentSequenceGoto;
                if (intGoto != null)
                {
                    if (_log.Verbose())
                        Debug.LogFormat("Sequence 'Goto' on {0}", transform.GetDebugName());
                    if (intGoto.Condition != null)
                        if (!intGoto.Condition.IsConditionMet)
                            continue;
                    nextIntentIndex = intGoto.IntentIndexGoto - 1;
                    continue;
                }
            }
            
            Debug.Assert(intSeqItem.Intent != null, 
                string.Format("TriggerBase:ProcessSequence: intent {0} is null. Object: {1} ", nextIntentIndex, transform.GetDebugName()));
            intSeqItem.Intent.Apply();
        }
        _isInSequence = false;
    }
}
