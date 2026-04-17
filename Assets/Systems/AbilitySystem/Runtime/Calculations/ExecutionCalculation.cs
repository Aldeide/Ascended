using System;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Runtime.Calculations
{
    /// <summary>
    /// Base class for custom Execution Calculations.
    /// Execution Calculations allow complex mathematical formulas involving multiple attributes 
    /// from both the source and the target to be encapsulated and executed during instant effects.
    /// </summary>
    public abstract class ExecutionCalculation : ScriptableObject
    {
        public abstract void Execute(Effect effect);
    }
}
