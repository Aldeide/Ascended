using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Effects
{
    /// <summary>
    /// Unit tests for EffectDefinition, verifying correct identification of periodic and durational policies based on configured parameters.
    /// </summary>
    public class EffectDefinitionTests
    {
        /// <summary>
        /// Verifies that an effect with a positive period value is correctly identified as periodic.
        /// </summary>
        [Test]
        public void EffectDefinitionTests_PositivePeriod_IdentifiesAsPeriodic()
        {
            var effectDefinition = EffectUtilities.CreateDurationEffectDefinition();
            effectDefinition.Period = 1.0f;

            Assert.IsTrue(effectDefinition.IsPeriodic());
        }
        
        /// <summary>
        /// Verifies that an effect with a period of zero is correctly identified as non-periodic.
        /// </summary>
        [Test]
        public void EffectDefinitionTests_ZeroPeriod_IdentifiesAsNonPeriodic()
        {
            var effectDefinition = EffectUtilities.CreateDurationEffectDefinition();
            effectDefinition.Period = 0.0f;

            Assert.IsFalse(effectDefinition.IsPeriodic());
        }
        
        /// <summary>
        /// Verifies that an instant effect is never identified as periodic, regardless of other settings.
        /// </summary>
        [Test]
        public void EffectDefinitionTests_InstantEffect_IdentifiesAsNonPeriodic()
        {
            var effectDefinition = EffectUtilities.CreateInstantEffectDefinition();

            Assert.IsFalse(effectDefinition.IsPeriodic());
        }
        
        /// <summary>
        /// Verifies that a duration-based effect is correctly identified as having a durational policy.
        /// </summary>
        [Test]
        public void EffectDefinitionTests_DurationPolicy_IdentifiesAsDurational()
        {
            var effectDefinition = EffectUtilities.CreateDurationEffectDefinition();

            Assert.IsTrue(effectDefinition.IsDurationalPolicy());
        }
        
        /// <summary>
        /// Verifies that an instant effect is correctly identified as not having a durational policy.
        /// </summary>
        [Test]
        public void EffectDefinitionTests_InstantPolicy_IdentifiesAsNonDurational()
        {
            var effectDefinition = EffectUtilities.CreateInstantEffectDefinition();

            Assert.IsFalse(effectDefinition.IsDurationalPolicy());
        }
    }
}