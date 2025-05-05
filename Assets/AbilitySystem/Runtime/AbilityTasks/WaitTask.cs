using System;
using PlasticGui.WorkspaceWindow;

namespace AbilitySystem.Runtime.AbilityTasks
{
    [Serializable]
    public class WaitTask : DurationalAbilityTask
    {
        public override void Start()
        {
            StartTime = _owner.GetTime();
        }

        public override void Tick()
        {
            if (_owner.GetTime() - StartTime >= Duration) End();
        }

        public override void End()
        {
        }
    }
}