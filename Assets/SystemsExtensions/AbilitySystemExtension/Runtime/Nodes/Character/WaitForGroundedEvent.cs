using System;
using System.Threading.Tasks;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Abilities;
using AbilitySystemExtension.Scripts;
using GraphProcessor;
using UnityEngine;

using AbilityGraph.Runtime;

namespace AbilitySystemExtension.Runtime.Nodes.Character
{
    [Serializable, NodeMenuItem("Character/WaitForGroundedEvent")]
    public class WaitForGroundedEvent : WaitableNode
    {
        private TaskCompletionSource<object> _eventReceived;
        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            var ability = context.Ability;
            if (ability == null) return;
            var movementController = ((UnityEngine.Component)ability.Owner.NetworkRole)?.gameObject.GetComponent<PlayerMovementController>();
            if (!movementController) return;
            movementController.OnGroundedChanged += OnGroundedChanged;
        }

        public void OnGroundedChanged(bool grounded)
        {
            if (!grounded) return;
            if (_eventReceived != null)
            {
                _eventReceived.TrySetResult(null);
            }
            
        }
        
        protected override void Process()
        {
            var ignoredAwaitableResult = WaitForEvent();
        }
        
        private async Task WaitForEvent()
        {
            _eventReceived = new TaskCompletionSource<object>();
            await _eventReceived.Task;
            ProcessFinished();
        }
    }
}
