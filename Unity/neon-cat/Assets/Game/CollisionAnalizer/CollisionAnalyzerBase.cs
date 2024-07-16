using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using GameLib.Alg;
using GameLib.Log;
using UnityEngine;
using UnityEngine.Assertions;


// Base class ofr some kind of extended triggers that can do following:
// * provides more advanced filtering on top of standard collision layers using InteractableNodeTypes and InteractableNodeProps
// * if props of the object inside zone is changed reevaluate enter zone (example: object was positive magnetic but then was switched to negative magnetic. Another example: object was ChargeSurface and then changed. Etc.)
// * process enter-exit zone for multi collider objects. Example: a new collider was attached to the analyzed object on runtime or part of the object was disconected from the owner

// note: OnTriggerExit is not called if the triggering object is destroyed, set inactive, or if the collider is disabled.

public abstract class CollisionAnalyzerBase : MonoBehaviour
{
    public class Entry
    {
        public Entry(GameObject mainGameObject, Node node, Rigidbody rb, Collider firstAcurranceCollider)
        {
            MainGameObject = mainGameObject;
            Node = node;
            Rigidbody = rb;
            HitColliders = new HashSet<Collider> { firstAcurranceCollider };
        }

        public GameObject MainGameObject;
        public Rigidbody Rigidbody;
        public Node Node; // could be null

        public HashSet<Collider> HitColliders; // colliders of the Node which are inside analyzer zone 
        public float TimeInsideZone;
        public float TimeInsideCenterZone;
    }
    public delegate void OnAnalyzerEnter(Entry zoneEntry);
    public delegate void OnAnalyzerExit(Entry zoneEntry);

    #region inspector exposed
    public LogChecker LogChecker;
    public NodeType[] InteractableNodeTypes;
    public NodePropsFilteringMode PropsFilteringMode;
    public NodeProps PropsFilter;

    [Range(0f, 1f)]
    [Tooltip("Percent from epicenter zone")]
    public float CenterZone;
    #endregion

    public OnAnalyzerEnter OnEnter;
    public OnAnalyzerExit OnExit;

    protected HashSet<NodeType> _interactWith;
    protected Dictionary<GameObject, Entry> _entries;


    public abstract (float normalizedDistance, float distance, Vector3 normal) GetDistanceTo(Entry item);

    public virtual void Awake()
    {
        _entries = new Dictionary<GameObject, Entry>(32);
        _interactWith = new HashSet<NodeType>(InteractableNodeTypes);
    }

    void Reset()
    {
        CenterZone = 1f;
    }

    void FixedUpdate()
    {
        Lazy<List<Entry>> lazyToDelete = new Lazy<List<Entry>>(() => new List<Entry>(4));

        foreach (var kv in _entries)
        {
            var entry = kv.Value;

            // props of the object or/and InteractableNodeProps changed and object stops pass the filter?
            {
                var filterPassed = CollisionAnalyzerHelper.FilterPass(_interactWith, PropsFilter, PropsFilteringMode, entry.Node);
                if (!filterPassed)
                {
                    lazyToDelete.Value.Add(entry);
                    continue;
                }
            }

            // entire MainGameObject was destroyed or disabled?
            if (entry.MainGameObject == null || entry.MainGameObject.activeInHierarchy == false)
            {
                lazyToDelete.Value.Add(entry);
                continue;
            }

            // rigidbody was disabled ?
            if (entry.Rigidbody != null && entry.Rigidbody.detectCollisions == false)
            {
                lazyToDelete.Value.Add(entry);
                continue;
            }

            // delete destroyed or disabled colliders from zoneEntry.HitColliders
            entry.HitColliders.RemoveWhere(x => x == null || x.gameObject.activeInHierarchy == false || x.enabled == false);

            if (entry.HitColliders.Count == 0)
            {
                lazyToDelete.Value.Add(entry);
                continue;
            }

            // update time
            entry.TimeInsideZone += Time.fixedDeltaTime;
            if (GetDistanceTo(entry).normalizedDistance < CenterZone)
                entry.TimeInsideCenterZone += Time.fixedDeltaTime;
        }

        if (lazyToDelete.IsValueCreated)
            foreach (var entry in lazyToDelete.Value)
            {
                _entries.Remove(entry.MainGameObject);
                OnExit?.Invoke(entry);
            }
    }

    public Dictionary<GameObject, Entry> GetEntries()
    {
        return _entries;
    }

    protected void CollisionEnter(GameObject mainGameObject, Node node, Rigidbody rigidbody, Collider collider)
    {
        var filterPassed = CollisionAnalyzerHelper.FilterPass(_interactWith, PropsFilter, PropsFilteringMode, node);
        if (!filterPassed)
        {
            if (LogChecker.Verbose())
                Debug.Log($"'{transform.GetDebugName(addCoordinate: true)}' FILTERED collision with {mainGameObject.transform.GetDebugName(addCoordinate: true)} COLLIDER: {collider.transform.GetDebugName()}");
            return;
        }

        if (LogChecker.Verbose())
            Debug.Log($"{transform.GetDebugName()}: Enter collision AnalizerObject:{transform.name}<->{collider.name} COLLIDER: {collider.transform.GetDebugName()}");

        Entry entry = null;
        if (_entries.TryGetValue(mainGameObject, out entry))
        { // we have such node in entries already (with another collider)
            Assert.IsTrue(entry.HitColliders.Count != 0);
            Assert.IsFalse(entry.HitColliders.Contains(collider));
            entry.HitColliders.Add(collider);
        }
        else
        { // first time occurrence 
            entry = new Entry(mainGameObject, node, rigidbody, collider);
            _entries.Add(mainGameObject, entry);
            OnEnter?.Invoke(entry);
        }
    }
    
    protected void CollisionStay(GameObject mainGameObject, Node node, Rigidbody rigidbody, Collider collider)
    {
        if(_entries == null)
            return;
        
        if (_entries.ContainsKey(mainGameObject))
            return;

        // check if it can pass the filter then add it ( object was inside the zone but was unable to pass filter)
        if (!CollisionAnalyzerHelper.FilterPass(_interactWith, PropsFilter, PropsFilteringMode, node))
            return;

        var entry = new Entry(mainGameObject, node, rigidbody, collider);
        _entries.Add(mainGameObject, entry);
        OnEnter?.Invoke(entry);
    }
    protected void CollisionExit(GameObject mainGameObject, Collider collider)
    {
        Entry entry = null;
        if (!_entries.TryGetValue(mainGameObject, out entry))
            return; // no such entry (must be filtered inside CollisionEnter)
        else
        {
            Assert.IsNotNull(entry);
            Assert.IsTrue(entry.HitColliders.Contains(collider)); // make sure collider was hit before
            entry.HitColliders.Remove(collider);
            if (entry.HitColliders.Count == 0)
            { // no more colliders hit remaining for this node, therefore we have to remove this Entry
                _entries.Remove(mainGameObject);
                OnExit?.Invoke(entry);
            }
        }
    }
}



public static class CollisionAnalyzerHelper
{
    public static GameObject GetFirstStayInZoneMoreThan(this CollisionAnalyzerBase analyzer, float duration, bool onlyInCenterZone = true)
    {
        var item = analyzer.GetEntries().FirstOrDefault(x => onlyInCenterZone ? x.Value.TimeInsideCenterZone > duration : x.Value.TimeInsideZone > duration);
        if (item.Key == null)
            return null;
        return item.Value.MainGameObject;
    }

    public static bool IsInCollision(this CollisionAnalyzerBase analyzer, GameObject item)
    {
        return analyzer.GetEntries().ContainsKey(item);
    }

    public static bool IsInCollision(this CollisionAnalyzerBase analyzer, NodeType checkNodeType, NodeProps checkNodeProps, NodePropsFilteringMode filteringMode)
    {
        var items = analyzer.GetEntries();
        foreach (var entry in items)
        {
            var node = entry.Value.Node;
            if (FilterPass(checkNodeType, checkNodeProps, filteringMode, node))
                return true;
        }
        return false;
    }

    public static GameObject GetClosestToCenter(this CollisionAnalyzerBase analyzer, bool onlyInCenterZone = true)
    {
        float lowestDistance = Single.MaxValue;
        CollisionAnalyzerBase.Entry closest = null;
        foreach (var kv in analyzer.GetEntries())
        {
            var (normalizedDistance,distance, normal) = analyzer.GetDistanceTo(kv.Value);
            if (normalizedDistance < lowestDistance)
            {
                lowestDistance = normalizedDistance;
                closest = kv.Value;
            }
        }

        if (closest == null)
            return null;

        if (onlyInCenterZone)
        {
            if (lowestDistance <= analyzer.CenterZone)
            {
                return closest.MainGameObject;
            }

            return null;
        }

        return closest.MainGameObject;
    }

    public static bool FilterPass(HashSet<NodeType> allowedNodeTypes, NodeProps propsMask,
        NodePropsFilteringMode propsMaskMode, Node targetNode)
    {
        var targetNodeType = targetNode == null ? NodeType.Undefined : targetNode.NodeType;
        var targetNodeProps = targetNode == null ? NodeProps.NoProps : targetNode.NodeProps;

        var allTypesAllowed = allowedNodeTypes == null || allowedNodeTypes.Count == 0;

        if (!allTypesAllowed && !allowedNodeTypes.Contains(targetNodeType))
            return false;

        if (propsMaskMode == NodePropsFilteringMode.IGNORE)
            return true;
        if (propsMaskMode == NodePropsFilteringMode.AND)
            return (targetNodeProps & propsMask) == propsMask;
        if (propsMaskMode == NodePropsFilteringMode.EXCLUDE)
            return (targetNodeProps & ~propsMask) == ~propsMask;
        
        return false;
    }

    public static bool FilterPass(NodeType allowedNodeType, NodeProps propsMask,
        NodePropsFilteringMode propsMaskMode, Node targetNode)
    {
        var targetNodeType = targetNode == null ? NodeType.Undefined : targetNode.NodeType;
        var targetNodeProps = targetNode == null ? NodeProps.NoProps : targetNode.NodeProps;

        if (allowedNodeType != targetNodeType && allowedNodeType != NodeType.Undefined)
            return false;

        if (propsMaskMode == NodePropsFilteringMode.IGNORE)
            return true;

        if (targetNodeProps == NodeProps.NoProps) // object doesn't specify any props therefore it doesn't matter what additional filtering by props we want to do filter will not pass
            return false;

        if (propsMaskMode == NodePropsFilteringMode.AND)
            return (targetNodeProps & propsMask) == propsMask;

        if (propsMaskMode == NodePropsFilteringMode.EXCLUDE)
            return (targetNodeProps & ~propsMask) == ~propsMask;

        return false;
    }
}
