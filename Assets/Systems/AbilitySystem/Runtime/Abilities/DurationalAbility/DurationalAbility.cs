using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.DurationalAbility
{
    public class DurationalAbility : Ability
    {
        private AbilityTask _currentTask;
        private int _currentTaskIndex = 0;
        public DurationalAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            foreach (var task in ((DurationalAbilityDefinition)Definition)?.TickTasks)
            {
                if (task is DurationalAbilityTask abilityTask)
                {
                    abilityTask.Initialize(this, owner);
                }
            }
        }

        protected override void ActivateAbility(AbilityData data)
        {
            var instantAbilityTasks = (Definition as DurationalAbilityDefinition)?.ActivationTasks;
            if (instantAbilityTasks == null) return;
            foreach (var task in instantAbilityTasks)
            {
                task.Execute();
            }
            var tickTasks = (Definition as DurationalAbilityDefinition)?.TickTasks;
            if (tickTasks == null) return;
            if (tickTasks.Count == 0) return;
            _currentTask = tickTasks[_currentTaskIndex];
        }

        protected override void AbilityTick()
        {
            Debug.Log("Ticking ability");
            if (_currentTask is InstantAbilityTask task)
            {
                task?.Execute();
                if (_currentTaskIndex < ((DurationalAbilityDefinition)Definition).TickTasks.Count - 1)
                {
                    _currentTaskIndex++;
                    _currentTask = ((DurationalAbilityDefinition)Definition)?.TickTasks[_currentTaskIndex];
                    return;
                }

                EndAbility();
            }

            if (_currentTask is not DurationalAbilityTask abilityTask) return;
            if (abilityTask.IsActive)
            {
                abilityTask?.Tick();
                if (_currentTaskIndex < ((DurationalAbilityDefinition)Definition).TickTasks.Count - 1 && abilityTask.HasEnded())
                {
                    _currentTaskIndex++;
                    _currentTask = ((DurationalAbilityDefinition)Definition)?.TickTasks[_currentTaskIndex];
                    return;
                }

                EndAbility();
            }
            else
            {
                ((DurationalAbilityTask)_currentTask)?.Start();
            }
        }

        public override void EndAbility()
        {
            var instantAbilityTasks = (Definition as DurationalAbilityDefinition)?.EndTasks;
            if (instantAbilityTasks == null) return;
            foreach (var task in instantAbilityTasks)
            {
                task.Execute();
            }

            foreach (var task in ((DurationalAbilityDefinition)Definition)?.TickTasks)
            {
                if (task is DurationalAbilityTask abilityTask)
                {
                    abilityTask.Reset();
                }
            }
        }
    }
}