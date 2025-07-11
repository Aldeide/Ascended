using System;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.InstantAbility
{
    [CreateAssetMenu(fileName = "InstantAbility", menuName = "AbilitySystem/Abilities/InstantAbility")]
    public class InstantAbilityDefinition : AbilityDefinition
    {
        public InstantAbilityDefinition() : base()
        {
        }

        public override Type AbilityType()
        {
            return typeof(InstantAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new InstantAbility(this, owner);
        }
    }
}