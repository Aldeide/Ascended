using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using AISystem.Runtime.TargetKeys;
using AISystem.Runtime.WorldKeys;

namespace AISystem.Runtime.Actions
{
    public class TakeCoverAction : GoapActionBase<TakeCoverAction.Data>
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
            // The movement system handles moving the agent to the CoverTarget.
            // Once the agent is in range, Perform is called and we can complete the action,
            // which sets the world state key 'HasCover' to true.
            return ActionRunState.Completed;
        }
    }
}
