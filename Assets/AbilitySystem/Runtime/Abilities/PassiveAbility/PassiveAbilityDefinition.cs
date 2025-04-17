using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Abilities.PassiveAbility
{
    public class PassiveAbilityDefinition : AbilityDefinition
    {
        public PassiveAbilityDefinition(AbilityAsset asset) : base(asset)
        {
        }

        public override Ability CreateSpec(IAbilitySystem owner)
        {
            return new PassiveAbility(this, owner);
        }
    }
}