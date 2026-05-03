using System;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Effects
{
    /// <summary>
    /// Unit tests for the EffectManager, verifying the lifecycle, stacking, and tag-based filtering of active effects.
    /// </summary>
    public class EffectManagerTests : AbilitySystemTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            SourceMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            base.SetUp();
        }
        /// <summary>
        /// Verifies that a newly instantiated EffectManager is empty and has no predicted effects.
        /// </summary>
        [Test]
        public void EffectManagerTests_Instantiate_StartsEmpty()
        {
            var effectManager = new EffectManager(Source);

            Assert.AreEqual(0, effectManager.Effects.Count);
            Assert.AreEqual(0, effectManager.PredictedEffects.Count);
        }
        
        /// <summary>
        /// Verifies that an effect added to the manager is correctly stored and active.
        /// </summary>
        [Test]
        public void EffectManagerTests_AddEffect_EffectIsStoredInManager()
        {
            var effectDef = EffectUtilities.CreateInfiniteEffectDefinitionWithModifier();
            var effect = effectDef.ToEffect(Source, Target);
            
            Target.EffectManager.AddEffect(effect);
            
            Assert.AreEqual(1, Target.EffectManager.Effects.Count);
            Assert.AreEqual(effect, Target.EffectManager.Effects[0]);
        }
        
        /// <summary>
        /// Verifies that removing an effect from the manager correctly cleans up the active effects list.
        /// </summary>
        [Test]
        public void EffectManagerTests_RemoveEffect_EffectIsRemovedFromManager()
        {
            var effectDef = EffectUtilities.CreateInfiniteEffectDefinitionWithModifier();
            var effect = effectDef.ToEffect(Source, Target);
            
            Target.EffectManager.AddEffect(effect);
            Target.EffectManager.RemoveEffect(effect);
            
            Assert.AreEqual(0, Target.EffectManager.Effects.Count);
        }
        
        /// <summary>
        /// Verifies that durational effects correctly update their remaining time when the system ticks.
        /// </summary>
        [Test]
        public void EffectManagerTests_Tick_UpdatesEffectRemainingDuration()
        {
            var effect = EffectUtilities.CreateDurationalEffect(Source, Target);
            Target.EffectManager.AddEffect(effect);
            effect.Initialise(Source, Target);
            effect.Activate();
            
            TargetMock.Setup(m => m.GetTime()).Returns(5f);
            Target.EffectManager.Tick();

            Assert.AreEqual(95f, effect.RemainingDuration());
        }
        
        /// <summary>
        /// Verifies that durational effects are automatically removed from the manager once they expire.
        /// </summary>
        [Test]
        public void EffectManagerTests_DurationalEffect_ExpiresAndIsRemoved()
        {
            var effect = EffectUtilities.CreateDurationalEffect(Source, Target);
            Target.EffectManager.AddEffect(effect);
            effect.Initialise(Source, Target);
            effect.Activate();
            
            TargetMock.Setup(m => m.GetTime()).Returns(200f);
            Target.EffectManager.Tick();
            
            Assert.AreEqual(0, Target.EffectManager.Effects.Count);
        }

        /// <summary>
        /// Verifies that effects configured for 'AggregateBySource' stack correctly when multiple instances from the same source are applied.
        /// </summary>
        [Test]
        public void EffectManagerTests_StackingBySource_AggregatesCorrectly()
        {
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.name = "TestStackingEffect";
            effectAsset.DurationType = EffectDurationType.Infinite;
            effectAsset.EffectStack = new EffectStack
            {
                EffectStackType = EffectStackType.AggregateBySource,
                MaxStacks = 3
            };
            
            var effectA1 = effectAsset.ToEffect(Source, Target);
            var effectA2 = effectAsset.ToEffect(Source, Target);
            
            var sourceB = AbilitySystemUtilities.CreateMockAbilitySystem().Object;
            var effectB1 = effectAsset.ToEffect(sourceB, Target);
            
            Target.EffectManager.AddEffect(effectA1);
            Target.EffectManager.AddEffect(effectA2);
            Target.EffectManager.AddEffect(effectB1);
            
            Assert.AreEqual(2, Target.EffectManager.Effects.Count);
            Assert.AreEqual(2, Target.EffectManager.Effects[0].NumStacks);
            Assert.AreEqual(1, Target.EffectManager.Effects[1].NumStacks);
        }

        /// <summary>
        /// Verifies that the OnEffectStacksChanged event is fired when a new instance of an effect increases the stack count.
        /// </summary>
        [Test]
        public void EffectManagerTests_StackIncrement_FiresStacksChangedEvent()
        {
            bool eventFired = false;
            Target.EffectManager.OnEffectStacksChanged += (eff, oldStacks, newStacks) =>
            {
                if (oldStacks == 1 && newStacks == 2) eventFired = true;
            };

            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.name = "TestStackingEventEffect";
            effectAsset.DurationType = EffectDurationType.Infinite;
            effectAsset.EffectStack = new EffectStack
            {
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 5
            };
            
            var effect1 = effectAsset.ToEffect(Source, Target);
            var effect2 = effectAsset.ToEffect(Source, Target);
            
            Target.EffectManager.AddEffect(effect1);
            Target.EffectManager.AddEffect(effect2);
            
            Assert.IsTrue(eventFired);
        }

        /// <summary>
        /// Verifies that applying an effect with 'RemoveGameplayEffectsWithTags' correctly cleanses existing effects on the target.
        /// </summary>
        [Test]
        public void EffectManagerTests_EffectRemovalByTag_CleansesMatchingEffects()
        {
            var poisonTag = new Tag("Debuff.Poison");
            var poisonAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            poisonAsset.AssetTags = new Tag[] { poisonTag };
            poisonAsset.DurationType = EffectDurationType.Infinite;
            var poisonEffect = poisonAsset.ToEffect(Source, Target);
            Target.EffectManager.AddEffect(poisonEffect);
            
            Assert.AreEqual(1, Target.EffectManager.Effects.Count);
            
            var antidoteAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            antidoteAsset.DurationType = EffectDurationType.Infinite;
            antidoteAsset.RemoveGameplayEffectsWithTags = new Tag[] { poisonTag };
            var antidoteEffect = antidoteAsset.ToEffect(Source, Target);
            Target.EffectManager.AddEffect(antidoteEffect);
            
            Assert.AreEqual(1, Target.EffectManager.Effects.Count);
            Assert.AreEqual(antidoteEffect, Target.EffectManager.Effects[0]);
        }

        /// <summary>
        /// Verifies that an effect is automatically suspended and resumed based on whether the target possesses its 'OngoingRequiredTags'.
        /// </summary>
        [Test]
        public void EffectManagerTests_OngoingRequirement_SuspendsAndResumesEffect()
        {
            var requirementTag = new Tag("State.Stunned");
            Target.TagManager.AddTag(requirementTag);
            
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.OngoingRequiredTags = new Tag[] { requirementTag };
            var effect = effectAsset.ToEffect(Source, Target);
            
            Target.EffectManager.AddEffect(effect);
            effect.Activate();
            
            Assert.IsTrue(effect.IsActive);
            
            bool suspensionInvoked = false;
            Target.EffectManager.OnEffectSuspended += (e) => suspensionInvoked = true;
            
            Target.TagManager.RemoveTag(requirementTag);
            
            Assert.IsFalse(effect.IsActive);
            Assert.IsTrue(suspensionInvoked);
            
            bool resumptionInvoked = false;
            Target.EffectManager.OnEffectResumed += (e) => resumptionInvoked = true;
            
            Target.TagManager.AddTag(requirementTag);
            
            Assert.IsTrue(effect.IsActive);
            Assert.IsTrue(resumptionInvoked);
        }

        /// <summary>
        /// Verifies that retracting a predicted effect fires the OnEffectRetracted event and removes it from the predicted list.
        /// </summary>
        [Test]
        public void EffectManagerTests_RetractPredictedEffect_FiresRetractedEvent()
        {
            var effectAsset = EffectUtilities.CreateInfiniteEffectDefinitionWithModifier();
            var effect = effectAsset.ToEffect(Source, Target);
            var predictionKey = new PredictionKey { currentKey = 1 };
            
            Target.EffectManager.AddPredictedEffect(predictionKey, effect);
            
            bool retractedInvoked = false;
            Target.EffectManager.OnEffectRetracted += (e) => retractedInvoked = true;
            
            Target.EffectManager.RetractPredictedEffect(predictionKey);
            
            Assert.IsTrue(retractedInvoked);
            Assert.AreEqual(0, Target.EffectManager.PredictedEffects.Count);
        }
    }
}
