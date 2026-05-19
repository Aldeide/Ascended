using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using GameplayAttribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Test.Runtime.Modifiers
{
    /// <summary>
    /// Unit tests verifying Modifier Magnitude Calculation (MMC) attribute capturing and dynamic recalculation.
    /// </summary>
    public class ModifierMagnitudeCalculationTests : AbilitySystemTestBase
    {
        public class GapsTestAttributeSet : AttributeSet
        {
            public GameplayAttribute Health;
            public GameplayAttribute MaxHealth;

            public GapsTestAttributeSet(IAbilitySystem owner) : base(owner)
            {
                Name = nameof(GapsTestAttributeSet);
                Health = new GameplayAttribute("Health", this, 100f);
                MaxHealth = new GameplayAttribute("MaxHealth", this, 200f);
                AddAttribute(Health);
                AddAttribute(MaxHealth);
            }

            public override void Reset()
            {
            }
        }

        public class TestGapsMMC : ModifierMagnitudeCalculation
        {
            public override AttributeCaptureDefinition[] GetAttributeCaptures()
            {
                return new[]
                {
                    new AttributeCaptureDefinition
                    {
                        AttributeName = "GapsTestAttributeSet.Health",
                        CaptureSource = AttributeBasedModifier.AttributeFrom.Target,
                        Snapshot = false
                    },
                    new AttributeCaptureDefinition
                    {
                        AttributeName = "GapsTestAttributeSet.MaxHealth",
                        CaptureSource = AttributeBasedModifier.AttributeFrom.Source,
                        Snapshot = true
                    }
                };
            }

            public override float CalculateMagnitude(Effect effect, float modifierMagnitude)
            {
                var health = GetCapturedAttributeValue(effect, "GapsTestAttributeSet.Health", AttributeBasedModifier.AttributeFrom.Target, false);
                var maxHealth = GetCapturedAttributeValue(effect, "GapsTestAttributeSet.MaxHealth", AttributeBasedModifier.AttributeFrom.Source, true);
                return health * 0.5f + maxHealth * 0.1f + modifierMagnitude;
            }
        }

        protected override bool AddDefaultAttributes => false;

        private GapsTestAttributeSet _gapsAttributeSet;

        protected override void InitializeMocks()
        {
            if (SourceMock == null) SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem(AddDefaultAttributes);
            if (TargetMock == null) TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem(AddDefaultAttributes);
            AbilitySystemUtilities.LinkAbilitySystems(SourceMock, TargetMock);
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _gapsAttributeSet = new GapsTestAttributeSet(Source);
            Source.AttributeSetManager.AddAttributeSet(typeof(GapsTestAttributeSet), _gapsAttributeSet);
        }

        [Test]
        public void MMC_DynamicAndSnapshotCaptures_RecalculatesOnDependencyChange()
        {
            _gapsAttributeSet.Health.SetBaseValue(100f);
            _gapsAttributeSet.MaxHealth.SetBaseValue(200f);

            var mmc = ScriptableObject.CreateInstance<TestGapsMMC>();
            
            var modifier = new CalculationModifier
            {
                AttributeName = "GapsTestAttributeSet.Health",
                Operation = EffectOperation.Override,
                calculation = mmc,
                baseValue = 10f
            };

            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.DurationType = EffectDurationType.Infinite;
            effectDef.Modifiers = new[] { modifier };

            var effect = effectDef.ToEffect(Source, Source);
            effect.IsActive = true;
            Source.EffectManager.AddEffect(effect);

            // Health = 100 * 0.5 + 200 * 0.1 + 10 = 50 + 20 + 10 = 80
            Assert.AreEqual(80f, _gapsAttributeSet.Health.CurrentValue, "Initial MMC calculation mismatch");

            // Change MaxHealth (snapshot capture, shouldn't affect current calculations)
            _gapsAttributeSet.MaxHealth.SetBaseValue(300f);
            Assert.AreEqual(80f, _gapsAttributeSet.Health.CurrentValue, "Snapshotted attribute should not affect calculation after change");

            // Change Health base value (dynamic dependency, should trigger recalculation)
            // Health current = 50 * 0.5 + 200 * 0.1 + 10 = 25 + 20 + 10 = 55
            _gapsAttributeSet.Health.SetBaseValue(50f);
            Assert.AreEqual(55f, _gapsAttributeSet.Health.CurrentValue, "Dynamic dependency should cause recalculation on value change");
        }
    }
}
