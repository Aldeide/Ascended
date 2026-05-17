using System;
using Systems.AbilitySystemGraph.VM;
using Unity.GraphToolkit.Editor;

namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    /// <summary>
    /// An asynchronous logic node that halts VM execution for a specific duration.
    /// </summary>
    [Serializable]
    public class WaitTimeNode : AbilityNode
    {
        public float Duration = 1.0f;

        public WaitTimeNode()
        {
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<ExecutionFlow>("InFlow").WithDisplayName("In").Build();
            context.AddInputPort<float>("DurationData").WithDisplayName("Duration (Data)").WithDefaultValue(1.0f).Build();
            context.AddOutputPort<ExecutionFlow>("OutFlow").WithDisplayName("Out").Build();
        }

        public override OpCode GetOpCode()
        {
            return OpCode.WaitTime;
        }
    }
}
