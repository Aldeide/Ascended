using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using AISystem.Runtime.TargetKeys;
using AISystem.Runtime.WorldKeys;

namespace AISystem.Runtime.Actions
{
    public class MoveToFlankAction : GoapActionBase<MoveToFlankAction.Data>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
        }

        public override void Start(IMonoAgent agent, Data data)
        {
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            // The movement system handles moving the agent to the FlankTarget.
            // Once in range, the action completes and sets 'IsFlanking' to true.
            return ActionRunState.Completed;
        }
    }
}
