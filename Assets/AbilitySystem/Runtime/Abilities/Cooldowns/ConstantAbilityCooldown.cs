using System;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.Cooldowns
{
    [Serializable]
    public class ConstantAbilityCooldown : AbilityCooldown
    {
        public override float Calculate(IAbilitySystem owner)
        {
            return BaseDuration;
        }
    }
}