using System;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "PassiveAbility", menuName = "AbilitySystem/Abilities/PassiveAbility")]
    public class PassiveAbilityDefinition : AbilityDefinition
    {
        public PassiveAbilityDefinition() : base()
        {
        }

        public override Type AbilityType()
        {
            return typeof(PassiveAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new PassiveAbility(this, owner);
        }
    }
}