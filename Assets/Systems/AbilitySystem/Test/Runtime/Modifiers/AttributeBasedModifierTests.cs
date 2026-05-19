using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Modifiers
{
    /// <summary>
    /// Unit tests for the AttributeBasedModifier, verifying correct attribute capture, calculation logic, and dynamic dependency tracking.
    /// </summary>
    public class AttributeBasedModifierTests : AbilitySystemTestBase
    {
        private Effect _effect;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            _effect = new Effect(effectDef);
            _effect.Initialise(Source, Target);
        }

        /// <summary>
        /// Verifies that a modifier with 'SnapshotOnCreation' capture type correctly preserves the attribute value at the time of capture, ignoring subsequent changes.
        /// </summary>
        [Test]
        public void AttributeBasedModifierTests_SnapshotOnCreation_CapturesValueAtCreationTime()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Source,
                attributeFromName = "TestAttributeSet.Health",
                captureType = AttributeBasedModifier.AttributeCaptureType.SnapshotOnCreation,
                k = 1,
                b = 0
            };

            SourceAttributes.Health.SetBaseValue(10f);
            SourceAttributes.Health.SetCurrentValue(10f);

            modifier.CaptureAttributes(_effect);
            
            // Change source value after capture
            SourceAttributes.Health.SetCurrentValue(20f);

            float result = modifier.Calculate(_effect);
            Assert.AreEqual(10f, result, "Snapshot should use value at capture time, not the updated live value");
        }

        /// <summary>
        /// Verifies that a modifier with 'OnApplication' capture type correctly utilizes the live attribute value at the time of calculation.
        /// </summary>
        [Test]
        public void AttributeBasedModifierTests_OnApplication_UsesLiveValue()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Source,
                attributeFromName = "TestAttributeSet.Health",
                captureType = AttributeBasedModifier.AttributeCaptureType.OnApplication,
                k = 1,
                b = 0
            };

            SourceAttributes.Health.SetCurrentValue(10f);
            Assert.AreEqual(10f, modifier.Calculate(_effect));

            SourceAttributes.Health.SetCurrentValue(20f);
            Assert.AreEqual(20f, modifier.Calculate(_effect), "OnApplication should always reflect the current live value");
        }

        /// <summary>
        /// Verifies that the modifier correctly applies the linear formula (Value * K + B) during calculation.
        /// </summary>
        [Test]
        public void AttributeBasedModifierTests_CalculationCoefficients_AppliesKAndBCorrectly()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.Energy",
                captureType = AttributeBasedModifier.AttributeCaptureType.OnApplication,
                k = 2f,
                b = 5f
            };

            TargetAttributes.Energy.SetCurrentValue(10f);
            // Expected: (10 * 2) + 5 = 25
            Assert.AreEqual(25f, modifier.Calculate(_effect));
        }

        /// <summary>
        /// Verifies that modifiers using 'Dynamic' capture type correctly register their source attribute as a dynamic dependency.
        /// </summary>
        [Test]
        public void AttributeBasedModifierTests_DynamicMode_RegistersDynamicDependency()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.MovementSpeed",
                captureType = AttributeBasedModifier.AttributeCaptureType.Dynamic
            };

            var dependency = modifier.GetDynamicDependency(_effect);
            Assert.AreEqual(TargetAttributes.MovementSpeed, dependency);
        }

        /// <summary>
        /// Verifies that modifiers using 'OnApplication' capture type do not register dynamic dependencies.
        /// </summary>
        [Test]
        public void AttributeBasedModifierTests_OnApplication_DoesNotRegisterDependency()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.MovementSpeed",
                captureType = AttributeBasedModifier.AttributeCaptureType.OnApplication
            };

            Assert.IsNull(modifier.GetDynamicDependency(_effect));
        }

        /// <summary>
        /// Verifies that an effect using a dynamic attribute-based modifier automatically updates its magnitude when the dependency attribute changes.
        /// </summary>
        [Test]
        public void AttributeBasedModifierTests_DynamicMode_AutomaticallyRecalculatesOnDependencyChange()
        {
            // Setup an effect that modifies Health based on Energy dynamically
            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.DurationType = EffectDurationType.Infinite;
            
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.Energy",
                captureType = AttributeBasedModifier.AttributeCaptureType.Dynamic,
                k = 1f,
                b = 0f
            };
            modifier.AttributeName = "TestAttributeSet.Health";
            modifier.Operation = EffectOperation.Additive;
            
            effectDef.Modifiers = new[] { modifier };

            TargetAttributes.Health.SetBaseValue(100f);
            TargetAttributes.Energy.SetBaseValue(10f);

            var effect = new Effect(effectDef);
            effect.Initialise(Source, Target);
            Target.ApplyEffectToSelf(effect);

            // Initially: 100 (base) + 10 (modifier) = 110
            Assert.AreEqual(110f, TargetAttributes.Health.CurrentValue);

            // Change Energy (the dynamic dependency)
            TargetAttributes.Energy.SetBaseValue(50f);

            // Should automatically update to: 100 (base) + 50 (modifier) = 150
            Assert.AreEqual(150f, TargetAttributes.Health.CurrentValue, "Target attribute should have updated automatically when the dependency attribute changed");
        }
    }
}
