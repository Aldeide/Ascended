using System;
using Systems.AbilitySystemGraph.VM;
using Unity.GraphToolkit.Editor;

namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    /// <summary>
    /// An execution node that splits flow based on a boolean condition.
    /// </summary>
    [Serializable]
    public class BranchNode : AbilityNode
    {
        public BranchNode()
        {
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<ExecutionFlow>("InFlow").WithDisplayName("In").Build();
            context.AddInputPort<bool>("Condition").WithDisplayName("Condition").WithDefaultValue(false).Build();
            context.AddOutputPort<ExecutionFlow>("TrueFlow").WithDisplayName("True").Build();
            context.AddOutputPort<ExecutionFlow>("FalseFlow").WithDisplayName("False").Build();
        }

        public override OpCode GetOpCode()
        {
            return OpCode.BranchTrue;
        }
    }
}
