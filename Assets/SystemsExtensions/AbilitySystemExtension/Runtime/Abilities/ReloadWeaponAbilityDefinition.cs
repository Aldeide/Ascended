using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "ReloadWeaponAbility", menuName = "AbilitySystem/Abilities/ReloadWeaponAbility")]
    public class ReloadWeaponAbilityDefinition : AbilityDefinition
    {
        public override Type AbilityType()
        {
            return typeof(ReloadWeaponAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new ReloadWeaponAbility(this, owner);
        }
    }
}
