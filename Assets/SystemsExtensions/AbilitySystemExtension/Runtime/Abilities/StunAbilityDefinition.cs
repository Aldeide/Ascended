using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "StunAbility", menuName = "AbilitySystem/Abilities/StunAbility")]
    public class StunAbilityDefinition : AbilityDefinition
    {
        public override Type AbilityType()
        {
            throw new NotImplementedException();
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
           return new StunAbility(this, owner);
        }
    }
}