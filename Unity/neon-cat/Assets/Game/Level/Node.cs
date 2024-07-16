using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum NodeType
    {
        Undefined, 
        Level,
        Decoration,
        BlockFree,
        BlockStatic,
        PortalPullIn,
        PortalPushOut,
        Coin,
        DeadlyWater,
        Player,
        Hostage,
        ChargePill
    }

    [Flags]
    public enum NodeProps
    {
        NoProps = 0,
        ChargeSurface = 1,
        DischargeSurface = 2,
        PlusMagnet = 4,
        MinusMagnet = 8,
        Pickup = 16,
        NoGravity = 32,
        Damaging = 64,
    }

    public enum NodePropsFilteringMode
    {
        IGNORE,
        AND, // additionally mask by NodeProps to the result of the filtering by NodeType
        EXCLUDE, // exclude all objects masked by NodeProps to the result of filtering by NodeType
    }

    // Node represents  physical object which works correctly with OnEnter-OnExit collision and triggers pairs.
    public class Node : MonoBehaviour
    {
        public NodeType NodeType;
        public NodeProps NodeProps;

        // todo: logical material: wood, glass (could be used for sfx)
    }

    public static class NodeHelper
    {
        public static (NodeType type, NodeProps props) GetNodeTypeAndProps(this GameObject mainGameObject)
        {
            var node = mainGameObject.GetComponent<Node>();
            if (node == null)
                return (NodeType.Undefined, NodeProps.NoProps);
            return (node.NodeType, node.NodeProps);
        }
    }
}

