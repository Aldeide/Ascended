using NUnit.Framework;
using UnityEngine;
using ScalableFloat.Runtime;

namespace ScalableFloat.Test
{
    /// <summary>
    /// Unit tests for ScalableFloat, verifying curve evaluation, value scaling, and input clamping.
    /// </summary>
    public class ScalableFloatTests
    {
        /// <summary>
        /// Verifies that evaluating at 0 correctly returns the curve's start value scaled by the base value.
        /// </summary>
        [Test]
        public void ScalableFloatTests_EvaluateAtZero_ReturnsStartValue()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(0f);

            Assert.AreEqual(0f, result);
        }

        /// <summary>
        /// Verifies that evaluating at 1 correctly returns the curve's end value scaled by the base value.
        /// </summary>
        [Test]
        public void ScalableFloatTests_EvaluateAtOne_ReturnsEndValue()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(1f);

            Assert.AreEqual(10f, result);
        }

        /// <summary>
        /// Verifies that evaluating at a midpoint correctly returns the interpolated curve value scaled by the base value.
        /// </summary>
        [Test]
        public void ScalableFloatTests_EvaluateAtMidpoint_ReturnsInterpolatedValue()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(0.5f);

            Assert.AreEqual(5f, result);
        }

        /// <summary>
        /// Verifies that evaluation inputs less than zero are correctly clamped to the curve's start value.
        /// </summary>
        [Test]
        public void ScalableFloatTests_EvaluateBelowZero_ClampsToZero()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(-0.5f);

            Assert.AreEqual(0f, result);
        }

        /// <summary>
        /// Verifies that evaluation inputs greater than one are correctly clamped to the curve's end value.
        /// </summary>
        [Test]
        public void ScalableFloatTests_EvaluateAboveOne_ClampsToOne()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(1.5f);

            Assert.AreEqual(10f, result);
        }
        
        /// <summary>
        /// Verifies that changing the base value correctly scales the result of the curve evaluation.
        /// </summary>
        [Test]
        public void ScalableFloatTests_DifferentBaseValue_ScalesCorrectly()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 20f
            };

            var result = scalableFloat.Evaluate(0.5f);

            Assert.AreEqual(10f, result);
        }

        /// <summary>
        /// Verifies that different curve shapes result in correct evaluation at the specified input.
        /// </summary>
        [Test]
        public void ScalableFloatTests_CustomCurve_EvaluatesCorrectly()
        {
            // Inverted linear curve: starts at 1, ends at 0
            var curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = curve,
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(0.5f);

            Assert.AreEqual(5f, result);
        }
    }
}
