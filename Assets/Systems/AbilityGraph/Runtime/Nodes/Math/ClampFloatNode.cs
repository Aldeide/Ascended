using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Math
{
    [Serializable, NodeMenuItem("Math/Clamp Float")]
    public class ClampFloatNode : AbilityNode
    {
        [Input(name = "Value")]
        public float Value;

        [Input(name = "Min")]
        public float Min = 0f;

        [Input(name = "Max")]
        public float Max = 1f;

        [Output(name = "Result")]
        public float Result;

        protected override void Process()
        {
            Result = Mathf.Clamp(Value, Min, Max);
        }
    }
}
