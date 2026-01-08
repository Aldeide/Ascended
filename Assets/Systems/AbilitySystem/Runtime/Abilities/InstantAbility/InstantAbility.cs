using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Abilities.InstantAbility
{
    public class InstantAbility : Ability
    {
        public InstantAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(AbilityData data)
        {
            
        }

        public override void EndAbility()
        {
            
        }
    }
}