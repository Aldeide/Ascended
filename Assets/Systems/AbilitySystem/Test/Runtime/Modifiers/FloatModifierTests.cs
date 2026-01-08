using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Modifiers
{
    public class FloatModifierTests
    {
        [Test]
        public void FloatModifierTests_Calculate_HasCorrectModifier()
        {
            var floatModifier = new FloatModifier
            {
                ModifierMagnitude = 5
            };

            Assert.AreEqual(5, floatModifier.Calculate(null));
        }
    }
}