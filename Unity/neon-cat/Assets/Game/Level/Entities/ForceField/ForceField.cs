using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using GameLib.Alg;
using GameLib.Log;
using UnityEngine;
using UnityEngine.Assertions;

public class ForceField : MonoBehaviour
{
    public enum ForceType
    {
        Gravity,
        Magnetism
    }

    [Serializable]
    public class NodeTypeForceMutiplier
    {
        public NodeType NodeType;
        [Range(0f, 10f)]
        public float ForceMultiplier = 1f;
    }

    public ForceType FieldForceType;

    [Tooltip("Ignored when FieldForceType == Gravity. Equal to -1 +1 ")]
    public bool PositiveMagnetism;

    public TriggerZoneAnalyzer ZoneAnalyzer;
    public float MaxForce;
    public AnimationCurve ForceDistribution;
    public bool InverseDistribution;

    [Range(0f, 1f)]
    public float DampingFactor;

    [Range(0f, 1f)]
    public float CompensateGravityFactor;

    public NodeTypeForceMutiplier[] ForceMultipliers;
    public LogChecker LogChecker;




    public Rigidbody MagneticOppositeForceReceiver;

    private Dictionary<NodeType, float> _nodeTypeToMultiplier;

    void Awake()
    {
        _nodeTypeToMultiplier = new Dictionary<NodeType, float>(ForceMultipliers.Length);
        foreach (var nodeTypeForceMutiplier in ForceMultipliers)
            _nodeTypeToMultiplier.Add(nodeTypeForceMutiplier.NodeType, nodeTypeForceMutiplier.ForceMultiplier);
    }

    public void SetEnabled(bool flag)
    {
        enabled = flag;
    }


    void FixedUpdate()
    {
        var items = ZoneAnalyzer.GetEntries();
        foreach (var zoneItem in items)
        {
            var (normalizedDistance, distance, normal) = ZoneAnalyzer.GetDistanceTo(zoneItem.Value);
            var factor = Mathf.Clamp01(normalizedDistance); // 0 - center; 1 - on the radius

            var revFactor = 1f - factor; // 1 - center; 0 - on the radius
            var typeAndProps = zoneItem.Value.MainGameObject.GetNodeTypeAndProps();
            
            Assert.IsFalse(typeAndProps.props.HasFlag(NodeProps.MinusMagnet) && typeAndProps.props.HasFlag(NodeProps.PlusMagnet), "item has both + and - magnetism");
            int itemMagnetism = 0; // -1 0 +1
            if (typeAndProps.props.HasFlag(NodeProps.MinusMagnet))
                itemMagnetism = -1;
            if (typeAndProps.props.HasFlag(NodeProps.PlusMagnet))
                itemMagnetism = +1;

            if(itemMagnetism == 0 && FieldForceType == ForceType.Magnetism)
                continue;

            var forceDistFactor = ForceDistribution.Evaluate(InverseDistribution ? revFactor : factor);
            var force = MaxForce * forceDistFactor;
            var rb = zoneItem.Value.Rigidbody;
            Assert.IsNotNull(rb, $"{zoneItem.Value.Node?.name}");

            if (LogChecker.Verbose())
            {
                Debug.Log($"ForceField {transform.GetDebugName()}");
                Debug.Log($"normalized dist = {normalizedDistance} ");
                Debug.Log($"force = {force} ");
            }


            //rb.AddForce(Physics.gravity * -1f, ForceMode.Acceleration);
            //rb.AddForce(-rb.velocity * revFactor, ForceMode.VelocityChange);
            //rb.AddForce(-normal * revFactor * 5f, ForceMode.VelocityChange);



            // force multiplier
            var forceMultiplier = 1f;
            if (!_nodeTypeToMultiplier.TryGetValue(typeAndProps.type, out forceMultiplier))
                forceMultiplier = 1f;

            // magnetism
            var forceMagnetism = 1;
            if (itemMagnetism != 0 && FieldForceType == ForceType.Magnetism)
            {
                var forceFieldMagnetism = PositiveMagnetism ? 1 : -1;
                if (forceFieldMagnetism != itemMagnetism)
                    forceMagnetism = -1;
            }

            forceMultiplier *= forceMagnetism;

            // apply force
            rb.AddForce(normal * force * forceMultiplier, ForceMode.Force);


            if (itemMagnetism != 0 && MagneticOppositeForceReceiver != null && !typeAndProps.props.HasFlag(NodeProps.Pickup))
            {
                MagneticOppositeForceReceiver.AddForce(-normal * force * forceMultiplier);
            }

            // damping
            if (DampingFactor != 0f)
            {
                if (factor < DampingFactor)
                {
                    if (CompensateGravityFactor != 0f)
                        rb.AddForce(Physics.gravity * -CompensateGravityFactor, ForceMode.Acceleration);
                    rb.AddForce(-rb.linearVelocity * 0.1f, ForceMode.VelocityChange);
                }
            }

            if(zoneItem.Value.Node != null)
                zoneItem.Value.Node.gameObject.GetComponent<ForceImpact>()?.AddForce(normal * force, ForceMode.Force);
        }
    }
}
