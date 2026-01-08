using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.AbilityTasks
{
    [Serializable]
    public abstract class AbilityTask
    {
        protected Ability _owningAbility;
        protected IAbilitySystem _owner;

        public void Initialize(Ability owningAbility, IAbilitySystem owner)
        {
            _owningAbility = owningAbility;
            _owner = owner;
        }
    }
}