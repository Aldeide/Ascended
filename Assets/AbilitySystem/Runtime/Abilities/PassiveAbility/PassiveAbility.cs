using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Abilities.PassiveAbility
{
    public class PassiveAbility : Ability
    {
        public PassiveAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        public override void ActivateAbility(params object[] args)
        {
        }

        public override void CancelAbility()
        {
        }

        public override void EndAbility()
        {
        }
    }
}