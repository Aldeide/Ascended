using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, Obsolete("Use the new generic SelectNode instead.")]
    public class SelectFloatNode : AbilityNode
    {
        [Input(name = "Condition")]
        public bool Condition;

        [Input(name = "True Value")]
        public float TrueValue;

        [Input(name = "False Value")]
        public float FalseValue;

        [Output(name = "Result")]
        public float Result;

        protected override void Process()
        {
            Result = Condition ? TrueValue : FalseValue;
        }
    }
}
