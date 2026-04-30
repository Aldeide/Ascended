using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Events;
using GameplayTags.Runtime;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/Wait Gameplay Event Task")]
    public class WaitGameplayEventTaskNode : WaitableNode
    {
        [Input(name = "Event Type Name")]
        public string EventTypeName;

        [Output(name = "Payload")]
        public GameplayEvent Payload;

        private WaitGameplayEventTask _task;

        protected override void Process()
        {
            if (Ability == null || string.IsNullOrEmpty(EventTypeName))
            {
                ProcessFinished();
                return;
            }

            var type = System.Type.GetType(EventTypeName);
            if (type == null)
            {
                ProcessFinished();
                return;
            }

            _task = WaitGameplayEventTask.CreateWaitGameplayEvent(Ability, type);
            _task.OnEventReceived += OnEventReceived;
            _task.ReadyForActivation();
        }

        private void OnEventReceived(GameplayEvent payload)
        {
            Payload = payload;
            if (_task != null)
            {
                _task.OnEventReceived -= OnEventReceived;
            }
            ProcessFinished();
        }
    }
}
