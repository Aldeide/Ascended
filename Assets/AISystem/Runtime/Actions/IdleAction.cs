using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Actions
{
    public class IdleAction : GoapActionBase<IdleAction.Data>
    {
        
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            public float Timer { get; set; }
        }
        
        public override void Start(IMonoAgent agent, Data data)
        {
            data.Timer = Random.Range(0.5f, 1.5f);
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            if (data.Timer <= 0f)
                // Return completed to stop the action
                return ActionRunState.Completed;
            
            // Lower the timer for the next frame
            data.Timer -= context.DeltaTime;
            
            // Return continue to keep the action running
            return ActionRunState.Continue;
        }
    }
}