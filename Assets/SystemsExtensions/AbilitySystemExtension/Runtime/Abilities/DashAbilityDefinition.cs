using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "DashAbility", menuName = "AbilitySystem/Abilities/DashAbility")]
    public class DashAbilityDefinition : AbilityDefinition
    {
        public override Type AbilityType()
        {
            return typeof(DashAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new DashAbility(this, owner);
        }
    }
}