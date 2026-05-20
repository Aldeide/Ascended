using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AbilityTasks;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Wait Input Press")]
    public class WaitInputPressNode : WaitableNode
    {
        private WaitInputPressTask _task;

        protected override void Process()
        {
            if (Ability == null)
            {
                ProcessFinished();
                return;
            }

            _task = WaitInputPressTask.CreateWaitInputPress(Ability);
            _task.OnPressed += OnPressed;
            _task.ReadyForActivation();
        }

        private void OnPressed()
        {
            if (_task != null)
            {
                _task.OnPressed -= OnPressed;
            }
            ProcessFinished();
        }
    }
}
