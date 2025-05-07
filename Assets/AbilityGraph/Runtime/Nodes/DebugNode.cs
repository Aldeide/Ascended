using System;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes
{
    [Serializable, NodeMenuItem("Utilities/Debug")]
    public class DebugNode : LinearExecutableNode
    {
        [SerializeField] public string Message;
        protected override void Process()
        {
            Debug.Log(Message);
        }
    }
}