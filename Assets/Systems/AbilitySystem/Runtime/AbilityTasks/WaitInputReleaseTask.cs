using System;
using AbilitySystem.Runtime.Abilities;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class WaitInputReleaseTask : AbilityTask
    {
        public event Action OnReleased;

        public static WaitInputReleaseTask CreateWaitInputRelease(Ability owningAbility)
        {
            var task = new WaitInputReleaseTask();
            task.Initialize(owningAbility);
            return task;
        }

        protected override void Activate()
        {
            OwningAbility.OnInputReleased += HandleInputReleased;
        }

        private void HandleInputReleased()
        {
            OnReleased?.Invoke();
            EndTask();
        }

        protected override void OnDestroy()
        {
            if (OwningAbility != null)
            {
                OwningAbility.OnInputReleased -= HandleInputReleased;
            }
            OnReleased = null;
        }
    }
}
