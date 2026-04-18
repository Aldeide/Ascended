using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Math
{
    [Serializable, NodeMenuItem("Math/Vector3 Decompose")]
    public class Vector3DecomposeNode : AbilityNode
    {
        [Input(name = "Vector")]
        public Vector3 Vector;

        [Output(name = "X")]
        public float X;

        [Output(name = "Y")]
        public float Y;

        [Output(name = "Z")]
        public float Z;

        protected override void Process()
        {
            X = Vector.x;
            Y = Vector.y;
            Z = Vector.z;
        }
    }
}
