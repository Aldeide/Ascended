using System;
using AbilitySystem.Runtime.Abilities;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class WaitInputPressTask : AbilityTask
    {
        public event Action OnPressed;

        public static WaitInputPressTask CreateWaitInputPress(Ability owningAbility)
        {
            var task = new WaitInputPressTask();
            task.Initialize(owningAbility);
            return task;
        }

        protected override void Activate()
        {
            OwningAbility.OnInputPressed += HandleInputPressed;
        }

        private void HandleInputPressed()
        {
            OnPressed?.Invoke();
            EndTask();
        }

        protected override void OnDestroy()
        {
            if (OwningAbility != null)
            {
                OwningAbility.OnInputPressed -= HandleInputPressed;
            }
            OnPressed = null;
        }
    }
}
