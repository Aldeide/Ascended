using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;

namespace AbilitySystem.Test.Runtime.Modifiers
{
    public class AttributeBasedModifierTests
    {
        private TestAttributeSet _sourceAttributes;
        private TestAttributeSet _targetAttributes;
        private Effect _effect;

        [SetUp]
        public void SetUp()
        {
            var source = CreateMockAbilitySystem();
            var target = CreateMockAbilitySystem();

            _sourceAttributes = source.Object.AttributeSetManager.GetAttributeSet<TestAttributeSet>();
            _targetAttributes = target.Object.AttributeSetManager.GetAttributeSet<TestAttributeSet>();

            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            _effect = new Effect(effectDef);
            _effect.Initialise(source.Object, target.Object);
        }

        [Test]
        public void AttributeBasedModifier_SnapshotSource_CapturesValueAtCreation()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Source,
                attributeFromName = "TestAttributeSet.Health",
                captureType = AttributeBasedModifier.AttributeCaptureType.SnapshotOnCreation,
                k = 1,
                b = 0
            };

            _sourceAttributes.Health.SetBaseValue(10);
            _sourceAttributes.Health.SetCurrentValue(10);

            modifier.CaptureAttributes(_effect);
            
            // Change source value after capture
            _sourceAttributes.Health.SetCurrentValue(20);

            float result = modifier.Calculate(_effect);
            Assert.AreEqual(10f, result, "Snapshot should use value at capture time");
        }

        [Test]
        public void AttributeBasedModifier_OnApplicationSource_UsesLiveValue()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Source,
                attributeFromName = "TestAttributeSet.Health",
                captureType = AttributeBasedModifier.AttributeCaptureType.OnApplication,
                k = 1,
                b = 0
            };

            _sourceAttributes.Health.SetCurrentValue(10);
            Assert.AreEqual(10f, modifier.Calculate(_effect));

            _sourceAttributes.Health.SetCurrentValue(20);
            Assert.AreEqual(20f, modifier.Calculate(_effect), "OnApplication should use live value");
        }

        [Test]
        public void AttributeBasedModifier_CalculationWithKAndB()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.Energy",
                captureType = AttributeBasedModifier.AttributeCaptureType.OnApplication,
                k = 2,
                b = 5
            };

            _targetAttributes.Energy.SetCurrentValue(10);
            // 10 * 2 + 5 = 25
            Assert.AreEqual(25f, modifier.Calculate(_effect));
        }

        [Test]
        public void AttributeBasedModifier_DynamicMode_RegistersDependency()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.MovementSpeed",
                captureType = AttributeBasedModifier.AttributeCaptureType.Dynamic
            };

            var dependency = modifier.GetDynamicDependency(_effect);
            Assert.AreEqual(_targetAttributes.MovementSpeed, dependency);
        }

        [Test]
        public void AttributeBasedModifier_OnApplication_DoesNotRegisterDependency()
        {
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.MovementSpeed",
                captureType = AttributeBasedModifier.AttributeCaptureType.OnApplication
            };

            Assert.IsNull(modifier.GetDynamicDependency(_effect));
        }

        [Test]
        public void AttributeBasedModifier_DynamicMode_UpdatesWhenAttributeChanges()
        {
            // Setup an effect that modifies Health based on Energy
            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.DurationType = EffectDurationType.Infinite;
            
            var modifier = new AttributeBasedModifier
            {
                attributeFromType = AttributeBasedModifier.AttributeFrom.Target,
                attributeFromName = "TestAttributeSet.Energy",
                captureType = AttributeBasedModifier.AttributeCaptureType.Dynamic,
                k = 1,
                b = 0
            };
            modifier.AttributeName = "TestAttributeSet.Health";
            modifier.Operation = EffectOperation.Additive;
            
            effectDef.Modifiers = new[] { modifier };

            _targetAttributes.Health.SetBaseValue(100);
            _targetAttributes.Health.SetCurrentValue(100);
            _targetAttributes.Energy.SetCurrentValue(10);

            var effect = new Effect(effectDef);
            effect.Initialise(_effect.Source, _effect.Owner);
            _effect.Owner.ApplyEffectToSelf(effect);

            // Initially: 100 (base) + 10 (modifier) = 110
            Assert.AreEqual(110f, _targetAttributes.Health.CurrentValue);

            // Change Energy (dependency)
            _targetAttributes.Energy.SetCurrentValue(50);

            // Should update to: 100 (base) + 50 (modifier) = 150
            Assert.AreEqual(150f, _targetAttributes.Health.CurrentValue, "Health should update automatically when Energy changes");
        }
    }
}
