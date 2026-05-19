using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Attributes;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Runtime.Modifiers
{
    /// <summary>
    /// Interface for modifiers that rely on dynamic attribute values which can change over time.
    /// This allows the AttributeAggregator to subscribe to value change events of the dependency
    /// and recalculate the modifier's owner attribute accordingly.
    /// </summary>
    public interface IDynamicDependency
    {
        /// <summary>
        /// Retrieves the specific attribute instance that this modifier depends on for its calculation.
        /// </summary>
        /// <param name="effect">The active effect instance being evaluated.</param>
        /// <returns>The attribute instance to track, or null if no dynamic tracking is required.</returns>
        Attribute GetDynamicDependency(Effect effect);
    }

    /// <summary>
    /// Interface for modifiers that rely on multiple dynamic attribute values.
    /// </summary>
    public interface IMultiDynamicDependency
    {
        /// <summary>
        /// Retrieves the specific attribute instances that this modifier depends on.
        /// </summary>
        IEnumerable<Attribute> GetDynamicDependencies(Effect effect);
    }
}
