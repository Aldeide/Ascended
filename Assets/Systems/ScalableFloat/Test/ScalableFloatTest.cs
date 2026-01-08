using NUnit.Framework;
using UnityEngine;
using ScalableFloat.Runtime;

namespace ScalableFloat.Test
{
    public class ScalableFloatTest
    {
        [Test]
        public void Evaluate_WithValueZero_ReturnsCorrectValue()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(0f);

            Assert.AreEqual(0f, result);
        }

        [Test]
        public void Evaluate_WithValueOne_ReturnsCorrectValue()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(1f);

            Assert.AreEqual(10f, result);
        }

        [Test]
        public void Evaluate_WithValueBetweenZeroAndOne_ReturnsCorrectValue()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(0.5f);

            Assert.AreEqual(5f, result);
        }

        [Test]
        public void Evaluate_WithValueLessThanZero_ClampsToZero()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(-0.5f);

            Assert.AreEqual(0f, result);
        }

        [Test]
        public void Evaluate_WithValueGreaterThanOne_ClampsToOne()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 10f
            };

            var result = scalableFloat.Evaluate(1.5f);

            Assert.AreEqual(10f, result);
        }
        
        [Test]
        public void Evaluate_WithDifferentBaseValue_ReturnsCorrectlyScaledValue()
        {
            var scalableFloat = new Runtime.ScalableFloat
            {
                Curve = AnimationCurve.Linear(0, 0, 1, 1),
                BaseValue = 20f
            };

            var result = scalableFloat.Evaluate(0.5f);

            Assert.AreEqual(10f, result);
        }

        [Test]
        public void Evaluate_WithDifferentCurve_ReturnsCorrectValue()
        {
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
