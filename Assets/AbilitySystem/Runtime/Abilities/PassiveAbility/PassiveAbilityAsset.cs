using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.PassiveAbility
{
    public class PassiveAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(PassiveAbilityDefinition);
        }

        public PassiveAbilityDefinition ToDefinition()
        {
            return new PassiveAbilityDefinition();
        }
    }
}