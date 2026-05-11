using System;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "StunAbility", menuName = "AbilitySystem/Abilities/Stun")]
    public class StunAbilityDefinition : AbilityDefinition
    {
        public override Type AbilityType() => typeof(StunAbility);

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new StunAbility(this, owner);
        }
    }
}
