using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using GameplayAttribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Test.Runtime.AttributeSets
{
    /// <summary>
    /// Unit tests verifying AttributeSet clamping behavior and execution callbacks.
    /// </summary>
    public class AttributeSetClampingTests : AbilitySystemTestBase
    {
        public class GapsTestAttributeSet : AttributeSet
        {
            public GameplayAttribute Health;
            public GameplayAttribute MaxHealth;

            public int PreGameplayEffectExecuteCount = 0;
            public int PostGameplayEffectExecuteCount = 0;
            public int PreAttributeChangeCount = 0;
            public int PostAttributeChangeCount = 0;

            public float LastPreGameplayEffectMagnitude = 0f;
            public float LastPostGameplayEffectMagnitude = 0f;

            public GapsTestAttributeSet(IAbilitySystem owner) : base(owner)
            {
                Name = nameof(GapsTestAttributeSet);
                Health = new GameplayAttribute("Health", this, 100f);
                MaxHealth = new GameplayAttribute("MaxHealth", this, 200f);
                AddAttribute(Health);
                AddAttribute(MaxHealth);
            }

            public override bool PreGameplayEffectExecute(Effect effect, Modifier modifier, ref float magnitude)
            {
                PreGameplayEffectExecuteCount++;
                LastPreGameplayEffectMagnitude = magnitude;

                if (magnitude < -50f)
                {
                    magnitude = -50f;
                }

                if (magnitude == 999f)
                {
                    return false;
                }

                return true;
            }

            public override void PostGameplayEffectExecute(Effect effect, Modifier modifier, float magnitude)
            {
                PostGameplayEffectExecuteCount++;
                LastPostGameplayEffectMagnitude = magnitude;
            }

            public override void PreAttributeChange(GameplayAttribute attribute, ref float newValue)
            {
                PreAttributeChangeCount++;

                if (attribute.GetName() == "Health")
                {
                    newValue = Mathf.Clamp(newValue, 0f, MaxHealth.BaseValue);
                }
            }

            public override void PostAttributeChange(GameplayAttribute attribute, float oldValue, float newValue)
            {
                PostAttributeChangeCount++;
            }

            public override void Reset()
            {
                PreGameplayEffectExecuteCount = 0;
                PostGameplayEffectExecuteCount = 0;
                PreAttributeChangeCount = 0;
                PostAttributeChangeCount = 0;
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
        public void ClampingAndCallbacks_PreAttributeChange_ClampsAttributeValue()
        {
            _gapsAttributeSet.Reset();

            _gapsAttributeSet.Health.SetBaseValue(300f);

            Assert.AreEqual(200f, _gapsAttributeSet.Health.BaseValue, "Value should be clamped to MaxHealth");
            Assert.Greater(_gapsAttributeSet.PreAttributeChangeCount, 0, "PreAttributeChange hook should have been called");
            Assert.Greater(_gapsAttributeSet.PostAttributeChangeCount, 0, "PostAttributeChange hook should have been called");
        }

        [Test]
        public void ClampingAndCallbacks_PreGameplayEffectExecute_CanModifyMagnitudeOrCancel()
        {
            _gapsAttributeSet.Reset();

            var modifier = new FloatModifier
            {
                AttributeName = "GapsTestAttributeSet.Health",
                Operation = EffectOperation.Additive,
                ModifierMagnitude = -100f
            };

            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.DurationType = EffectDurationType.Instant;
            effectDef.Modifiers = new[] { modifier };

            var effect = effectDef.ToEffect(Source, Source);

            Source.AttributeSetManager.ApplyInstantEffectModifiers(effect);

            Assert.AreEqual(50f, _gapsAttributeSet.Health.BaseValue, "Health base value should be 50 (100 - 50 clamped damage)");
            Assert.AreEqual(1, _gapsAttributeSet.PreGameplayEffectExecuteCount, "PreGameplayEffectExecute count should be 1");
            Assert.AreEqual(1, _gapsAttributeSet.PostGameplayEffectExecuteCount, "PostGameplayEffectExecute count should be 1");
            Assert.AreEqual(-50f, _gapsAttributeSet.LastPostGameplayEffectMagnitude, "The magnitude passed to post-execute should be the modified magnitude");

            var cancelMod = new FloatModifier
            {
                AttributeName = "GapsTestAttributeSet.Health",
                Operation = EffectOperation.Additive,
                ModifierMagnitude = 999f
            };
            var cancelEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            cancelEffectDef.DurationType = EffectDurationType.Instant;
            cancelEffectDef.Modifiers = new[] { cancelMod };
            var cancelEffect = cancelEffectDef.ToEffect(Source, Source);

            _gapsAttributeSet.Reset();
            Source.AttributeSetManager.ApplyInstantEffectModifiers(cancelEffect);

            Assert.AreEqual(50f, _gapsAttributeSet.Health.BaseValue, "Health value should remain unchanged as modifier was cancelled");
            Assert.AreEqual(1, _gapsAttributeSet.PreGameplayEffectExecuteCount, "PreGameplayEffectExecute count should increment");
            Assert.AreEqual(0, _gapsAttributeSet.PostGameplayEffectExecuteCount, "PostGameplayEffectExecute should not fire for cancelled modifier");
        }
    }
}
