using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    /// <summary>
    /// Base class for custom effect application requirements.
    /// Designers can inherit from this to create custom logic for whether an effect can be applied to a target.
    /// </summary>
    public abstract class EffectApplicationRequirement : ScriptableObject
    {
        /// <summary>
        /// Evaluates whether the effect can be applied.
        /// </summary>
        /// <param name="target">The target ability system.</param>
        /// <param name="source">The source ability system.</param>
        /// <param name="effect">The effect being applied.</param>
        /// <returns>True if the effect can be applied, false otherwise.</returns>
        public abstract bool CanApplyEffect(IAbilitySystem target, IAbilitySystem source, Effect effect);
    }
}
