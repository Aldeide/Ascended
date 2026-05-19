using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class ChargesAbilityTests : AbilitySystemTestBase
    {
        private ChargesAbilityDefinition _abilityDef;
        private ChargesAbility _ability;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            TargetMock.Setup(x => x.IsServer()).Returns(true);
            
            _abilityDef = ScriptableObject.CreateInstance<ChargesAbilityDefinition>();
            _abilityDef.UniqueName = "TestChargesAbility";
            _abilityDef.MaxCharges = 2;
            
            // Setup a 1-second cooldown
            var cooldownEffect = ScriptableObject.CreateInstance<EffectDefinition>();
            cooldownEffect.DurationType = EffectDurationType.FixedDuration;
            cooldownEffect.DurationSeconds = 1f;
            cooldownEffect.GrantedTags = new[] { new Tag("Cooldown.Test") };
            
            var cooldown = new ConstantAbilityCooldown();
            cooldown.CooldownEffect = cooldownEffect;
            _abilityDef.Cooldown = cooldown;
            
            _abilityDef.MaxChargesMetaAttribute = "Ability.Charges.Max";
            
            _ability = (ChargesAbility)Target.AbilityManager.GrantAbility(_abilityDef);
        }

        [Test]
        public void ChargesAbilityTests_Activation_ConsumesCharge()
        {
            Assert.AreEqual(2, _ability.CurrentCharges);
            
            _ability.TryActivateAbility(default);
            Assert.AreEqual(1, _ability.CurrentCharges);
            
            _ability.TryActivateAbility(default);
            Assert.AreEqual(0, _ability.CurrentCharges);
            
            var result = _ability.CanActivate();
            Assert.AreEqual(AbilityActivationResult.NoCharges, result);
        }

        [Test]
        public void ChargesAbilityTests_Regeneration_RestoresChargesViaCooldown()
        {
            _ability.TryActivateAbility(default);
            _ability.TryActivateAbility(default);
            Assert.AreEqual(0, _ability.CurrentCharges);
            Assert.IsTrue(_ability.IsActive || true); // Just to call Tick

            // Initially, a cooldown should be running
            _ability.Tick(); 
            Assert.AreEqual(0, _ability.CurrentCharges);

            // Mock time forward by 1.1 seconds and tick effects to finish the cooldown
            TargetMock.Setup(x => x.GetTime()).Returns(1.5f);
            Target.EffectManager.Tick();
            
            // Now tick the ability to detect the finished cooldown and gain a charge
            _ability.Tick();
            Assert.AreEqual(1, _ability.CurrentCharges);
            
            // Cooldown should have restarted automatically. Move time to 2.1s
            TargetMock.Setup(x => x.GetTime()).Returns(2.5f);
            Target.EffectManager.Tick();
            _ability.Tick();
            Assert.AreEqual(2, _ability.CurrentCharges);
        }

        [Test]
        public void ChargesAbilityTests_MetaAttribute_IncreasesMaxCharges()
        {
            // Create an effect that adds 1 to MaxChargesMetaAttribute
            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.DurationType = EffectDurationType.Infinite;
            
            var modifier = new FloatModifier
            {
                AttributeName = "Ability.Charges.Max",
                Operation = EffectOperation.Additive,
                ModifierMagnitude = 1f
            };
            effectDef.Modifiers = new Modifier[] { modifier };

            var effect = new Effect(effectDef);
            effect.Initialise(Source, Target);
            Target.ApplyEffectToSelf(effect);

            // Max charges should now be 2 (base) + 1 (modifier) = 3
            Assert.AreEqual(3, _ability.GetMaxCharges());
            
            // Current charges should still be 2 until it recharges
            _ability.Tick(); // This starts the recharge cooldown
            Assert.AreEqual(2, _ability.CurrentCharges);

            // Mock time forward by 1.1 seconds and tick effects to finish the cooldown
            TargetMock.Setup(x => x.GetTime()).Returns(1.1f);
            Target.EffectManager.Tick();
            
            _ability.Tick();
            Assert.AreEqual(3, _ability.CurrentCharges);
        }

        [Test]
        public void ChargesAbilityTests_TagQuery_FiltersModifiers()
        {
            var boostTag = new Tag("Effect.Boost");
            _abilityDef.MaxChargesModifiersTagQuery = new TagQuery 
            { 
                Condition = new[]
                {
                    new TagCondition 
                    { 
                        MatchType = TagMatchType.AnyOfPartial, 
                        Tags = new[] { boostTag } 
                    }
                } 
            };
            
            // Effect 1: Has the tag (AssetTags)
            var effectDef1 = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef1.name = "EffectBoost";
            effectDef1.DurationType = EffectDurationType.Infinite;
            effectDef1.AssetTags = new[] { boostTag };
            var mod1 = new FloatModifier { AttributeName = "Ability.Charges.Max", Operation = EffectOperation.Additive, ModifierMagnitude = 1f };
            effectDef1.Modifiers = new Modifier[] { mod1 };
            
            // Effect 2: Doesn't have the tag
            var effectDef2 = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef2.name = "EffectNoBoost";
            effectDef2.DurationType = EffectDurationType.Infinite;
            effectDef2.AssetTags = new Tag[0];
            var mod2 = new FloatModifier { AttributeName = "Ability.Charges.Max", Operation = EffectOperation.Additive, ModifierMagnitude = 10f };
            effectDef2.Modifiers = new Modifier[] { mod2 };

            var effect1 = new Effect(effectDef1);
            effect1.Initialise(Source, Target);
            Target.ApplyEffectToSelf(effect1);
            
            var effect2 = new Effect(effectDef2);
            effect2.Initialise(Source, Target);
            Target.ApplyEffectToSelf(effect2);

            // Only Effect 1 should apply (2 base + 1 boost = 3)
            Assert.AreEqual(3, _ability.GetMaxCharges(), "Only the effect matching the TagQuery should apply");
        }
        [Test]
        public void ChargesAbilityTests_Events_FiresOnActivation()
        {
            int callCount = 0;
            _ability.OnChargesChanged += (curr, max) => callCount++;
            
            _ability.TryActivateAbility(default);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ChargesAbilityTests_Events_FiresOnRegeneration()
        {
            _ability.TryActivateAbility(default);
            _ability.TryActivateAbility(default);
            
            // We must tick once to start the regeneration cooldown at Time = 0
            _ability.Tick(); 
            
            int callCount = 0;
            _ability.OnChargesChanged += (curr, max) => callCount++;
            
            // Fast forward and tick to gain a charge
            TargetMock.Setup(x => x.GetTime()).Returns(1.1f);
            Target.EffectManager.Tick();
            _ability.Tick();
            
            Assert.AreEqual(1, callCount, "OnChargesChanged should fire when a charge is gained");
        }

        [Test]
        public void ChargesAbilityTests_Events_FiresCooldownEvents()
        {
            _ability.TryActivateAbility(default);
            
            float durationFired = 0;
            _ability.OnCooldownStarted += (dur) => durationFired = dur;
            
            float progressFired = -1;
            _ability.OnCooldownProgressChanged += (prog) => progressFired = prog;
            
            // First tick should start the cooldown and fire start event
            _ability.Tick();
            Assert.AreEqual(1f, durationFired);
            
            // Mock 0.5s elapsed (50% progress)
            TargetMock.Setup(x => x.GetTime()).Returns(0.5f);
            _ability.Tick();
            
            Assert.That(progressFired, Is.InRange(0.49f, 0.51f), $"Progress should be 0.5, but was {progressFired}");
        }
    }
}
