using System;
using Systems.AbilitySystemGraph.VM;
using AbilitySystem.Runtime.Effects;
using Unity.GraphToolkit.Editor;

namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    /// <summary>
    /// An action node that applies a Gameplay Effect to a target.
    /// Commands the VM to push an ApplyEffect request to the Main Thread.
    /// </summary>
    [Serializable]
    public class ApplyEffectNode : AbilityNode
    {
        public Effect EffectToApply;

        public ApplyEffectNode()
        {
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<ExecutionFlow>("InFlow").WithDisplayName("In").Build();
            context.AddOutputPort<ExecutionFlow>("OutFlow").WithDisplayName("Out").Build();
        }

        public override OpCode GetOpCode()
        {
            return OpCode.CmdApplyEffect;
        }
    }
}
