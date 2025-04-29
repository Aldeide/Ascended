using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "DeathAbility", menuName = "AbilitySystem/Abilities/DeathAbility")]
    public class DeathAbilityDefinition : AbilityDefinition
    {
        public override Type AbilityType()
        {
            return typeof(DeathAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new DeathAbility(this, owner);
        }
    }
}