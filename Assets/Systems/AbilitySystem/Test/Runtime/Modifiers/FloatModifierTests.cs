using AbilitySystem.Runtime.Modifiers;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Modifiers
{
    /// <summary>
    /// Unit tests for the FloatModifier, ensuring it correctly returns its constant magnitude.
    /// </summary>
    public class FloatModifierTests
    {
        /// <summary>
        /// Verifies that a FloatModifier initialized with a specific magnitude correctly returns that value during calculation.
        /// </summary>
        [Test]
        public void FloatModifierTests_Calculate_ReturnsConfiguredMagnitude()
        {
            var floatModifier = new FloatModifier
            {
                ModifierMagnitude = 5f
            };

            // FloatModifier calculation is independent of the effect context
            Assert.AreEqual(5f, floatModifier.Calculate(null));
        }
    }
}