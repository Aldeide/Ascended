using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Abilities.InstantAbility
{
    public abstract class InstantAbility : Ability
    {
        protected InstantAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }
    }
}