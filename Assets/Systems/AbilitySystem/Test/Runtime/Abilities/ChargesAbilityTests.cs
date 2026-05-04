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
        public void ChargesAbilityTests_TaggedModifier_OnlyAppliesIfTagsMatch()
        {
            _abilityDef.ModifierRequiredTags = new[] { new Tag("Ability.Charges.Boost") };
            
            // Create an effect that adds 1 to MaxChargesMetaAttribute
            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.DurationType = EffectDurationType.Infinite;
            
            var modifier = new FloatModifier
            {
                AttributeName = "Ability.Charges.Max",
                Operation = EffectOperation.Additive,
                ModifierMagnitude = 5f
            };
            effectDef.Modifiers = new Modifier[] { modifier };

            var effect = new Effect(effectDef);
            effect.Initialise(Source, Target);
            
            // 1. Apply without tag
            Target.ApplyEffectToSelf(effect);
            Assert.AreEqual(2, _ability.GetMaxCharges(), "Modifier should not apply without required tag");

            // 2. Add the required tag to the target
            Target.TagManager.AddTag(new Tag("Ability.Charges.Boost"));
            Assert.AreEqual(7, _ability.GetMaxCharges(), "Modifier should apply when required tag is present");
        }
    }
}
