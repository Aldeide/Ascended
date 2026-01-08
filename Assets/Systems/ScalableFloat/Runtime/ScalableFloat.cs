using System;
using UnityEngine;

namespace ScalableFloat.Runtime
{
    /// <summary>
    /// Represents a float value that can be scaled based on an animation curve and a base value.
    /// </summary>
    [Serializable]
    public class ScalableFloat
    {
        public AnimationCurve Curve;
        public float BaseValue;

        /// <summary>
        /// Evaluates the scalable float by applying the given value to the animation curve and scaling the result by the base value.
        /// </summary>
        /// <param name="value">The input value to evaluate, clamped between 0 and 1.</param>
        /// <returns>The evaluated float value after applying the animation curve and scaling with the base value.</returns>
        public float Evaluate(float value)
        {
            return Curve.Evaluate(Mathf.Clamp(value, 0, 1)) * BaseValue;
        }
    }
}