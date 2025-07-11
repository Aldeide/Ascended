using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using GameplayTags.Runtime;
using NUnit.Framework;

using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using static AbilitySystem.Test.Utilities.EffectUtilities;

namespace AbilitySystem.Test.Runtime.Modifiers
{
    public class CostModifierTests
    {
        [Test]
        public void CostModifierTests_CalculateWithNoRelevantEffect_ReturnsBaseCost()
        {
            var abilitySystem = CreateMockAbilitySystem();
            var costModifier = new CostModifier()
            {
                costMetaAttribute = "TestAttributeSet.AbilityCost",
                baseCost = 10,
                modifierTags = new Tag[] { new Tag("Cost.Ability.TestAbility") }
            };

            var effect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            abilitySystem.Object.EffectManager.AddEffect(effect);

            Assert.AreEqual(10, costModifier.Calculate(effect));
        }

        [Test]
        public void CostModifierTests_CalculateWithRelevantEffect_ReturnsModifierCost()
        {
            var abilitySystem = CreateMockAbilitySystem();
            var costModifier = new CostModifier()
            {
                costMetaAttribute = "TestAttributeSet.AbilityCost",
                baseCost = 10,
                modifierTags = new[] { new Tag("Cost.Ability.TestAbility") }
            };

            var effect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            var additiveModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Additive,
                ModifierMagnitude = 5
            };
            var subtractiveModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Subtractive,
                ModifierMagnitude = 1
            };
            var multiplicativeModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Multiplicative,
                ModifierMagnitude = 1.5f
            };
            var divisiveModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Divisive,
                ModifierMagnitude = 2f
            };
            var modifyCostEffect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            modifyCostEffect.Definition.Modifiers = new Modifier[]
                { additiveModifier, multiplicativeModifier, divisiveModifier, subtractiveModifier };
            modifyCostEffect.Definition.AssetTags = new[] { new Tag("Cost.Ability.TestAbility") };
            modifyCostEffect.Activate();
            abilitySystem.Object.EffectManager.AddEffect(effect);
            abilitySystem.Object.EffectManager.AddEffect(modifyCostEffect);

            Assert.AreEqual(10.5f, costModifier.Calculate(effect));
        }

        [Test]
        public void CostModifierTests_CalculateWithRelevantAncestorEffect_ReturnsModifierCost()
        {
            var abilitySystem = CreateMockAbilitySystem();
            var costModifier = new CostModifier()
            {
                costMetaAttribute = "TestAttributeSet.AbilityCost",
                baseCost = 10,
                modifierTags = new[] { new Tag("Cost.Ability.TestAbility") }
            };

            var effect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            var additiveModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Additive,
                ModifierMagnitude = 5
            };
            var multiplicativeModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Multiplicative,
                ModifierMagnitude = 1.5f
            };
            var modifyCostEffect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            modifyCostEffect.Definition.Modifiers = new Modifier[] { additiveModifier, multiplicativeModifier };
            modifyCostEffect.Definition.AssetTags = new[] { new Tag("Cost.Ability") };
            modifyCostEffect.Activate();
            abilitySystem.Object.EffectManager.AddEffect(effect);
            abilitySystem.Object.EffectManager.AddEffect(modifyCostEffect);

            Assert.AreEqual(22.5f, costModifier.Calculate(effect));
        }

        [Test]
        public void CostModifierTests_CalculateWithIrrelevantEffect_ReturnsBaseCost()
        {
            var abilitySystem = CreateMockAbilitySystem();
            var costModifier = new CostModifier()
            {
                costMetaAttribute = "TestAttributeSet.AbilityCost",
                baseCost = 10,
                modifierTags = new[] { new Tag("Cost.Ability.TestAbility") }
            };

            var effect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            var additiveModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Additive,
                ModifierMagnitude = 5
            };
            var multiplicativeModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Multiplicative,
                ModifierMagnitude = 1.5f
            };
            var modifyCostEffect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            modifyCostEffect.Definition.Modifiers = new Modifier[] { additiveModifier, multiplicativeModifier };
            modifyCostEffect.Definition.AssetTags = new[] { new Tag("Irrelevant.Tag") };
            modifyCostEffect.Activate();
            abilitySystem.Object.EffectManager.AddEffect(effect);
            abilitySystem.Object.EffectManager.AddEffect(modifyCostEffect);

            Assert.AreEqual(10f, costModifier.Calculate(effect));
        }

        [Test]
        public void CostModifierTests_CalculateWithOverride_ReturnsOverridenCost()
        {
            var abilitySystem = CreateMockAbilitySystem();
            var costModifier = new CostModifier()
            {
                costMetaAttribute = "TestAttributeSet.AbilityCost",
                baseCost = 10,
                modifierTags = new[] { new Tag("Cost.Ability.TestAbility") }
            };

            var effect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            var overrideModifier = new FloatModifier()
            {
                attributeName = "TestAttributeSet.AbilityCost",
                operation = EffectOperation.Override,
                ModifierMagnitude = 100f
            };
            var modifyCostEffect = CreateInfiniteEffect(abilitySystem.Object, abilitySystem.Object);
            modifyCostEffect.Definition.Modifiers = new Modifier[] { overrideModifier };
            modifyCostEffect.Definition.AssetTags = new[] { new Tag("Cost.Ability") };
            modifyCostEffect.Activate();
            abilitySystem.Object.EffectManager.AddEffect(effect);
            abilitySystem.Object.EffectManager.AddEffect(modifyCostEffect);

            Assert.AreEqual(100f, costModifier.Calculate(effect));
        }
    }
}