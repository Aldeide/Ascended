using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Abilities.PassiveAbility
{
    public class PassiveAbility : Ability
    {
        public PassiveAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(params object[] args)
        {
        }

        protected override void CancelAbility()
        {
        }

        public override void EndAbility()
        {
        }
    }
}