using System;
using AbilitySystem.Runtime.Abilities;

namespace AbilitySystem.Runtime.AbilityTasks
{
    [Serializable]
    public abstract class InstantAbilityTask : AbilityTask
    {
        public InstantAbilityTask(Ability owningAbility) : base(owningAbility)
        {
        }

        public abstract void Execute();
    }
}