using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Math
{
    public enum FloatArithmeticType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Power
    }

    [Serializable, NodeMenuItem("Math/Float Arithmetic")]
    public class FloatArithmeticNode : AbilityNode
    {
        [Input(name = "A")]
        public float A;

        [Input(name = "B")]
        public float B;

        public FloatArithmeticType Operation = FloatArithmeticType.Add;

        [Output(name = "Result")]
        public float Result;

        protected override void Process()
        {
            switch (Operation)
            {
                case FloatArithmeticType.Add:
                    Result = A + B;
                    break;
                case FloatArithmeticType.Subtract:
                    Result = A - B;
                    break;
                case FloatArithmeticType.Multiply:
                    Result = A * B;
                    break;
                case FloatArithmeticType.Divide:
                    Result = B != 0 ? A / B : 0;
                    break;
                case FloatArithmeticType.Modulo:
                    Result = B != 0 ? A % B : 0;
                    break;
                case FloatArithmeticType.Power:
                    Result = (float)System.Math.Pow(A, B);
                    break;
            }
        }
    }
}
