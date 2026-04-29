using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Events;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class WaitGameplayEventTask : AbilityTask
    {
        public event Action<GameplayEvent> OnEventReceived;

        private Type _eventType;

        public static WaitGameplayEventTask CreateWaitGameplayEvent(Ability owningAbility, Type eventType)
        {
            var task = new WaitGameplayEventTask();
            task.Initialize(owningAbility);
            task._eventType = eventType;
            return task;
        }

        protected override void Activate()
        {
            OwnerSystem.EventManager?.Subscribe(_eventType, OnGameplayEvent);
        }

        private void OnGameplayEvent(GameplayEvent payload)
        {
            OnEventReceived?.Invoke(payload);
        }

        protected override void OnDestroy()
        {
            OwnerSystem.EventManager?.Unsubscribe(_eventType, OnGameplayEvent);
            OnEventReceived = null;
        }
    }
}
