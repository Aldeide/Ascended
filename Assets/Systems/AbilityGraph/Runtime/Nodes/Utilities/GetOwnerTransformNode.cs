using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Scripts;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/Get Owner Transform")]
    public class GetOwnerTransformNode : AbilityNode
    {
        [Output(name = "Position")]
        public Vector3 Position;

        [Output(name = "Rotation")]
        public Vector3 Rotation;

        [Output(name = "Forward")]
        public Vector3 Forward;

        protected override void Process()
        {
            if (Owner?.NetworkRole is Component component)
            {
                var tr = component.transform;
                Position = tr.position;
                Rotation = tr.eulerAngles;
                Forward = tr.forward;
                return;
            }
            
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            Forward = Vector3.forward;
        }
    }
}
