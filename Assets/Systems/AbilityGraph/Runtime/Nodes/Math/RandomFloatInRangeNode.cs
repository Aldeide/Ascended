using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Math
{
    [Serializable, NodeMenuItem("Math/Random Float In Range")]
    public class RandomFloatInRangeNode : AbilityNode
    {
        [Input(name = "Min")]
        public float Min;

        [Input(name = "Max")]
        public float Max = 1f;

        [Output(name = "Result")]
        public float Result;

        protected override void Process()
        {
            Result = UnityEngine.Random.Range(Min, Max);
        }
    }
}
