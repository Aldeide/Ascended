using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Modifiers
{
    /// <summary>
    /// Unit tests for the CostModifier, verifying how base costs are modified by active gameplay effects based on tag requirements.
    /// </summary>
    public class CostModifierTests : AbilitySystemTestBase
    {
        private static readonly Tag AbilityCostTag = new Tag("Cost.Ability.TestAbility");
        private static readonly Tag GeneralCostTag = new Tag("Cost.Ability");

        /// <summary>
        /// Verifies that CostModifier returns the base cost when no relevant cost-modifying effects are active on the system.
        /// </summary>
        [Test]
        public void CostModifierTests_NoRelevantEffects_ReturnsBaseCost()
        {
            var costModifier = new CostModifier()
            {
                CostMetaAttribute = "TestAttributeSet.AbilityCost",
                BaseCost = 10f,
                ModifierTags = new[] { AbilityCostTag }
            };

            var dummyEffect = EffectUtilities.CreateInfiniteEffect(Source, Source);
            Source.EffectManager.AddEffect(dummyEffect);

            Assert.AreEqual(10f, costModifier.Calculate(dummyEffect));
        }

        /// <summary>
        /// Verifies that CostModifier correctly aggregates multiple operations (Add, Mul, Div, Sub) from a matching active effect.
        /// </summary>
        [Test]
        public void CostModifierTests_MatchingEffect_CalculatesModifiedCost()
        {
            var costModifier = new CostModifier()
            {
                CostMetaAttribute = "TestAttributeSet.AbilityCost",
                BaseCost = 10f,
                ModifierTags = new[] { AbilityCostTag }
            };

            var effectToModify = EffectUtilities.CreateInfiniteEffect(Source, Source);
            
            var modifyCostEffect = EffectUtilities.CreateInfiniteEffect(Source, Source);
            modifyCostEffect.Definition.AssetTags = new[] { AbilityCostTag };
            modifyCostEffect.Definition.Modifiers = new Modifier[]
            {
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Additive, ModifierMagnitude = 5f },
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Subtractive, ModifierMagnitude = 1f },
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Multiplicative, ModifierMagnitude = 1.5f },
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Divisive, ModifierMagnitude = 2f }
            };
            
            modifyCostEffect.Activate();
            Source.EffectManager.AddEffect(effectToModify);
            Source.EffectManager.AddEffect(modifyCostEffect);

            // Calculation: ((10 + 5 - 1) * 1.5) / 2 = (14 * 1.5) / 2 = 21 / 2 = 10.5
            Assert.AreEqual(10.5f, costModifier.Calculate(effectToModify));
        }

        /// <summary>
        /// Verifies that CostModifier respects tag hierarchy, correctly applying modifiers from effects with parent tags.
        /// </summary>
        [Test]
        public void CostModifierTests_ParentTagEffect_CalculatesModifiedCost()
        {
            var costModifier = new CostModifier()
            {
                CostMetaAttribute = "TestAttributeSet.AbilityCost",
                BaseCost = 10f,
                ModifierTags = new[] { AbilityCostTag }
            };

            var effectToModify = EffectUtilities.CreateInfiniteEffect(Source, Source);
            
            var modifyCostEffect = EffectUtilities.CreateInfiniteEffect(Source, Source);
            modifyCostEffect.Definition.AssetTags = new[] { GeneralCostTag }; // Parent tag of AbilityCostTag
            modifyCostEffect.Definition.Modifiers = new Modifier[]
            {
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Additive, ModifierMagnitude = 5f },
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Multiplicative, ModifierMagnitude = 1.5f }
            };
            
            modifyCostEffect.Activate();
            Source.EffectManager.AddEffect(effectToModify);
            Source.EffectManager.AddEffect(modifyCostEffect);

            // Calculation: (10 + 5) * 1.5 = 22.5
            Assert.AreEqual(22.5f, costModifier.Calculate(effectToModify));
        }

        /// <summary>
        /// Verifies that CostModifier ignores active effects that do not have any matching tags from its 'ModifierTags' list.
        /// </summary>
        [Test]
        public void CostModifierTests_IrrelevantEffect_ReturnsBaseCost()
        {
            var costModifier = new CostModifier()
            {
                CostMetaAttribute = "TestAttributeSet.AbilityCost",
                BaseCost = 10f,
                ModifierTags = new[] { AbilityCostTag }
            };

            var effectToModify = EffectUtilities.CreateInfiniteEffect(Source, Source);
            
            var modifyCostEffect = EffectUtilities.CreateInfiniteEffect(Source, Source);
            modifyCostEffect.Definition.AssetTags = new[] { new Tag("Irrelevant.Tag") };
            modifyCostEffect.Definition.Modifiers = new Modifier[]
            {
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Additive, ModifierMagnitude = 500f }
            };
            
            modifyCostEffect.Activate();
            Source.EffectManager.AddEffect(effectToModify);
            Source.EffectManager.AddEffect(modifyCostEffect);

            Assert.AreEqual(10f, costModifier.Calculate(effectToModify));
        }

        /// <summary>
        /// Verifies that an 'Override' operation correctly sets the cost to its magnitude, ignoring the base cost and other operations.
        /// </summary>
        [Test]
        public void CostModifierTests_OverrideEffect_ReturnsOverrideMagnitude()
        {
            var costModifier = new CostModifier()
            {
                CostMetaAttribute = "TestAttributeSet.AbilityCost",
                BaseCost = 10f,
                ModifierTags = new[] { AbilityCostTag }
            };

            var effectToModify = EffectUtilities.CreateInfiniteEffect(Source, Source);
            
            var modifyCostEffect = EffectUtilities.CreateInfiniteEffect(Source, Source);
            modifyCostEffect.Definition.AssetTags = new[] { GeneralCostTag };
            modifyCostEffect.Definition.Modifiers = new Modifier[]
            {
                new FloatModifier { AttributeName = "TestAttributeSet.AbilityCost", Operation = EffectOperation.Override, ModifierMagnitude = 100f }
            };
            
            modifyCostEffect.Activate();
            Source.EffectManager.AddEffect(effectToModify);
            Source.EffectManager.AddEffect(modifyCostEffect);

            Assert.AreEqual(100f, costModifier.Calculate(effectToModify));
        }
    }
}