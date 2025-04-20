using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "AimCameraAbility", menuName = "AbilitySystem/Abilities/AimCameraAbility")]
    public class AimCameraAbilityDefinition : AbilityDefinition
    {
        public AimCameraAbilityDefinition() : base()
        {
            AbilityTags = new AbilityTags(
                AssetTags, CancelAbilityTags, BlockAbilityTags, ActivationOwnedTags,
                ActivationRequiredTags, ActivationBlockedTags
            );
        }

        public override Type AbilityType()
        {
            return typeof(AimCameraAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new AimCameraAbility(this, owner);
        }
    }
}