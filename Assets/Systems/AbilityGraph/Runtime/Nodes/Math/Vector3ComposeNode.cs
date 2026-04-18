using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Math
{
    [Serializable, NodeMenuItem("Math/Vector3 Compose")]
    public class Vector3ComposeNode : AbilityNode
    {
        [Input(name = "X")]
        public float X;

        [Input(name = "Y")]
        public float Y;

        [Input(name = "Z")]
        public float Z;

        [Output(name = "Vector")]
        public Vector3 Vector;

        protected override void Process()
        {
            Vector = new Vector3(X, Y, Z);
        }
    }
}
