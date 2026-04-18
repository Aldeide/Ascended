using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Math
{
    public enum ComparisonType
    {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual
    }

    [Serializable, NodeMenuItem("Math/Comparison")]
    public class ComparisonNode : AbilityNode
    {
        [Input(name = "A")]
        public float A;

        [Input(name = "B")]
        public float B;

        public ComparisonType Comparison = ComparisonType.Equal;

        [Output(name = "Result")]
        public bool Result;

        protected override void Process()
        {
            switch (Comparison)
            {
                case ComparisonType.Equal:
                    Result = UnityEngine.Mathf.Approximately(A, B);
                    break;
                case ComparisonType.NotEqual:
                    Result = !UnityEngine.Mathf.Approximately(A, B);
                    break;
                case ComparisonType.Greater:
                    Result = A > B;
                    break;
                case ComparisonType.Less:
                    Result = A < B;
                    break;
                case ComparisonType.GreaterOrEqual:
                    Result = A >= B;
                    break;
                case ComparisonType.LessOrEqual:
                    Result = A <= B;
                    break;
            }
        }
    }
}
