using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.PassiveAbility
{
    [CreateAssetMenu(fileName = "PassiveAbility", menuName = "AbilitySystem/Abilities/PassiveAbility")]
    public class PassiveAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(PassiveAbilityDefinition);
        }
    }
}