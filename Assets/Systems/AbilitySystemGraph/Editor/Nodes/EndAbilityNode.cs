using System;
using Systems.AbilitySystemGraph.VM;
using Unity.GraphToolkit.Editor;

namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    /// <summary>
    /// The terminal point of an ability execution.
    /// When execution hits this node, the VM halts.
    /// This is an unremovable default node.
    /// </summary>
    [Serializable]
    public class EndAbilityNode : AbilityNode
    {
        public EndAbilityNode()
        {
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<ExecutionFlow>("InFlow")
                   .WithDisplayName("In")
                   .Build();
        }

        public override OpCode GetOpCode()
        {
            return OpCode.End;
        }
    }
}
