using System;
using Systems.AbilitySystemGraph.VM;
using Unity.GraphToolkit.Editor;

namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    /// <summary>
    /// The entry point of an ability execution.
    /// This is an unremovable default node.
    /// </summary>
    [Serializable]
    public class ActivateAbilityNode : AbilityNode
    {
        public ActivateAbilityNode()
        {
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<ExecutionFlow>("OutFlow")
                   .WithDisplayName("Out")
                   .Build();
        }

        public override OpCode GetOpCode()
        {
            return OpCode.Nop;
        }
    }
}
