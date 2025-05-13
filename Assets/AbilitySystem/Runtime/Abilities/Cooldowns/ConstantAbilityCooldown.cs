using System;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.Cooldowns
{
    /// <summary>
    /// A cooldown implementation that uses a constant value for its duration.
    /// </summary>
    /// <remarks>
    /// This class inherits from <see cref="AbilityCooldown"/> and overrides the cooldown calculation
    /// behavior to return a fixed duration specified by <c>BaseDuration</c>.
    /// </remarks>
    [Serializable]
    public class ConstantAbilityCooldown : AbilityCooldown
    {
        public override float Calculate(IAbilitySystem owner)
        {
            return CooldownEffect.durationSeconds;
        }
    }
}