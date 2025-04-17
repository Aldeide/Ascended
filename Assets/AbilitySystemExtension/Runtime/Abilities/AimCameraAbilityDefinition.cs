using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class AimCameraAbilityDefinition : AbilityDefinition
    {
        public AimCameraAbilityDefinition(AbilityAsset asset) : base(asset)
        {
        }

        public override Ability CreateSpec(IAbilitySystem owner)
        {
            return new AimCameraAbility(this, owner);
        }
    }
}