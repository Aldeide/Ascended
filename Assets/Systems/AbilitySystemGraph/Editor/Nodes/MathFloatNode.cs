using System;
using Systems.AbilitySystemGraph.VM;
using Unity.GraphToolkit.Editor;

namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    [Serializable]
    public class MathFloatNode : AbilityNode
    {
        public MathOperation Operation = MathOperation.Add;

        public MathFloatNode()
        {
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<float>("A").WithDisplayName("A").WithDefaultValue(0f).Build();
            context.AddInputPort<float>("B").WithDisplayName("B").WithDefaultValue(0f).Build();
            context.AddOutputPort<float>("Result").WithDisplayName("Result").Build();
        }

        public override OpCode GetOpCode()
        {
            switch (Operation)
            {
                case MathOperation.Subtract: return OpCode.SubFloat;
                case MathOperation.Multiply: return OpCode.MulFloat;
                case MathOperation.Divide:   return OpCode.DivFloat;
                case MathOperation.Add:
                default:                     return OpCode.AddFloat;
            }
        }
    }
}
