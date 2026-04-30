using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AbilityTasks;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/Wait Delay Task")]
    public class WaitDelayTaskNode : WaitableNode
    {
        [Input(name = "Duration")]
        public float Duration;

        private WaitDelayTask _task;

        protected override void Process()
        {
            if (Ability == null)
            {
                ProcessFinished();
                return;
            }

            _task = WaitDelayTask.CreateWaitDelay(Ability, Duration);
            _task.OnFinished += OnDelayFinished;
            _task.ReadyForActivation();
        }

        private void OnDelayFinished()
        {
            if (_task != null)
            {
                _task.OnFinished -= OnDelayFinished;
            }
            ProcessFinished();
        }
    }
}
