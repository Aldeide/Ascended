using System;
using AbilitySystem.Runtime.Abilities;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class WaitDelayTask : AbilityTask
    {
        public event Action OnFinished;

        private float _duration;
        private float _startTime;

        public static WaitDelayTask CreateWaitDelay(Ability owningAbility, float duration)
        {
            var task = new WaitDelayTask();
            task.Initialize(owningAbility);
            task._duration = duration;
            return task;
        }

        protected override void Activate()
        {
            _startTime = OwnerSystem.GetTime();
            if (_duration <= 0f)
            {
                CompleteTask();
            }
        }

        public override void TickTask()
        {
            if (OwnerSystem.GetTime() - _startTime >= _duration)
            {
                CompleteTask();
            }
        }

        private void CompleteTask()
        {
            OnFinished?.Invoke();
            EndTask();
        }

        protected override void OnDestroy()
        {
            OnFinished = null;
        }
    }
}
