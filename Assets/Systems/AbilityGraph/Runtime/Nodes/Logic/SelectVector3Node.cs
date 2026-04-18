using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, Obsolete("Use the new generic SelectNode instead.")]
    public class SelectVector3Node : AbilityNode
    {
        [Input(name = "Condition")]
        public bool Condition;

        [Input(name = "True Value")]
        public Vector3 TrueValue;

        [Input(name = "False Value")]
        public Vector3 FalseValue;

        [Output(name = "Result")]
        public Vector3 Result;

        protected override void Process()
        {
            Result = Condition ? TrueValue : FalseValue;
        }
    }
}
