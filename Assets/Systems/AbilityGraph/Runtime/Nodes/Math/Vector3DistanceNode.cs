using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Math
{
    [Serializable, NodeMenuItem("Math/Vector3 Distance")]
    public class Vector3DistanceNode : AbilityNode
    {
        [Input(name = "A")]
        public Vector3 A;

        [Input(name = "B")]
        public Vector3 B;

        [Output(name = "Distance")]
        public float Distance;

        protected override void Process()
        {
            Distance = Vector3.Distance(A, B);
        }
    }
}
