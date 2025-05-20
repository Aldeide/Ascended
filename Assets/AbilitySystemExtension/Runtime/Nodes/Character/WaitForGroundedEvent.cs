using System;
using System.Threading;
using System.Threading.Tasks;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Abilities;
using GraphProcessor;
using Systems.Movement;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Nodes.Character
{
    [Serializable, NodeMenuItem("Character/WaitForGroundedEvent")]
    public class WaitForGroundedEvent : WaitableNode
    {
        private TaskCompletionSource<object> eventReceived;
        public override void Initialise(Ability ability)
        {
            base.Initialise(ability);
            var movementController = ability.Owner.Component.gameObject.GetComponent<PlayerMovementController>();
            movementController.OnGroundedChanged += OnGroundedChanged;
        }

        public void OnGroundedChanged(bool grounded)
        {
            if (!grounded) return;
            if (eventReceived != null)
            {
                eventReceived.TrySetResult(null);
            }
            
        }
        
        protected override void Process()
        {
            Debug.Log("ProcessWaitForGroundedEvent");
            var ignoredAwaitableResult = WaitForEvent();
        }
        
        private async Task WaitForEvent()
        {
            eventReceived = new TaskCompletionSource<object>();
            await eventReceived.Task;
            Debug.Log("FinishedWaitForGroundedEvent");
            ProcessFinished();
        }
    }
}