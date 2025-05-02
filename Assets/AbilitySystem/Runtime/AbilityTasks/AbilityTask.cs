using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.AbilityTasks
{
    [Serializable]
    public abstract class AbilityTask
    {
        private Ability _owningAbility;
        private IAbilitySystem _owner;
        protected AbilityTask(Ability owningAbility)
        {
            _owningAbility = owningAbility;
            _owner = owningAbility.Owner;
        }
    }
}