using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AbilityTasks;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Wait Input Release")]
    public class WaitInputReleaseNode : WaitableNode
    {
        private WaitInputReleaseTask _task;

        protected override void Process()
        {
            if (Ability == null)
            {
                ProcessFinished();
                return;
            }

            _task = WaitInputReleaseTask.CreateWaitInputRelease(Ability);
            _task.OnReleased += OnReleased;
            _task.ReadyForActivation();
        }

        private void OnReleased()
        {
            if (_task != null)
            {
                _task.OnReleased -= OnReleased;
            }
            ProcessFinished();
        }
    }
}
