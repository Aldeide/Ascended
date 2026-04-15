using System;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Effects
{
    public class EffectManagerTest
    {
        [Test]
        public void EffectManagerTest_Instantiate_HasDefaultValues()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);

            Assert.AreEqual(0, effectManager.Effects.Count);
            Assert.AreEqual(0, effectManager.PredictedEffects.Count);
        }
        
        [Test]
        public void EffectManagerTest_AddEffect_HasAddedEffect()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);
            
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.ApplicationImmunityTags = Array.Empty<Tag>();
            effectAsset.ApplicationRequiredTags = Array.Empty<Tag>();
            var effect = effectAsset.ToEffect(owner.Object, owner.Object);
            
            effectManager.AddEffect(effect);
            
            Assert.AreEqual(1, effectManager.Effects.Count);
            Assert.AreEqual(effect, effectManager.Effects[0]);
        }
        
        [Test]
        public void EffectManagerTest_RemoveEffect_HasRemovedEffect()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.ApplicationImmunityTags = Array.Empty<Tag>();
            effectAsset.ApplicationRequiredTags = Array.Empty<Tag>();
            var effect = effectAsset.ToEffect(owner.Object, owner.Object);
            
            effectManager.AddEffect(effect);
            effectManager.RemoveEffect(effect);
            
            Assert.AreEqual(0, effectManager.Effects.Count);
        }
        
        [Test]
        public void EffectManagerTest_TickAsServer_TicksEffects()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);
            var attributeSystem = new Mock<AttributeSetManager>(owner.Object);
            owner.Setup(mock => mock.IsLocalClient()).Returns(false);
            owner.Setup(mock => mock.IsServer()).Returns(true);
            owner.Setup(mock => mock.GetTime()).Returns(1);
            owner.Setup(mock => mock.AttributeSetManager).Returns(attributeSystem.Object);
            var effect = EffectUtilities.CreateDurationalEffect(owner.Object, owner.Object);
            effectManager.AddEffect(effect);
            effect.Initialise(owner.Object, owner.Object);
            effect.Activate();
            owner.Setup(mock => mock.GetTime()).Returns(5);
            effectManager.Tick();

            Assert.AreEqual(96, effect.RemainingDuration());
        }
        
        [Test]
        public void EffectManagerTest_DurationalEffect_ExpiresCorrectly()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSystem = new Mock<AttributeSetManager>(owner.Object);
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.IsLocalClient()).Returns(false);
            owner.Setup(mock => mock.IsServer()).Returns(true);
            owner.Setup(mock => mock.GetTime()).Returns(1);
            owner.Setup(mock => mock.AttributeSetManager).Returns(attributeSystem.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);
            var effect = EffectUtilities.CreateDurationalEffect(owner.Object, owner.Object);
            effectManager.AddEffect(effect);
            effect.Initialise(owner.Object, owner.Object);
            effect.Activate();
            owner.Setup(mock => mock.GetTime()).Returns(200);
            effectManager.Tick();

            Assert.AreEqual(0, effectManager.Effects.Count);
        }

        [Test]
        public void EffectManagerTest_AggregateBySource_StacksCorrectly()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);
            
            var sourceA = new Mock<IAbilitySystem>();
            var sourceB = new Mock<IAbilitySystem>();
            
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.name = "TestStackingEffect";
            effectAsset.ApplicationImmunityTags = Array.Empty<Tag>();
            effectAsset.ApplicationRequiredTags = Array.Empty<Tag>();
            effectAsset.EffectStack = new EffectStack
            {
                EffectStackType = EffectStackType.AggregateBySource,
                MaxStacks = 3
            };
            
            var effectA1 = effectAsset.ToEffect(sourceA.Object, owner.Object);
            var effectA2 = effectAsset.ToEffect(sourceA.Object, owner.Object);
            var effectB1 = effectAsset.ToEffect(sourceB.Object, owner.Object);
            
            effectManager.AddEffect(effectA1);
            effectManager.AddEffect(effectA2);
            effectManager.AddEffect(effectB1);
            
            // Should be exactly 2 effect entries (one for source A, one for source B)
            Assert.AreEqual(2, effectManager.Effects.Count);
            // Effect from SourceA should have 2 stacks
            Assert.AreEqual(2, effectManager.Effects[0].NumStacks);
            // Effect from SourceB should have 1 stack
            Assert.AreEqual(1, effectManager.Effects[1].NumStacks);
        }

        [Test]
        public void EffectManagerTest_OnEffectStacksChanged_FiresEvent()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            
            bool eventFired = false;
            effectManager.OnEffectStacksChanged += (eff, oldStacks, newStacks) =>
            {
                if (oldStacks == 1 && newStacks == 2)
                    eventFired = true;
            };

            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);
            
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.name = "TestStackingEventEffect";
            effectAsset.ApplicationImmunityTags = Array.Empty<Tag>();
            effectAsset.ApplicationRequiredTags = Array.Empty<Tag>();
            effectAsset.EffectStack = new EffectStack
            {
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 5
            };
            
            var effect1 = effectAsset.ToEffect(owner.Object, owner.Object);
            var effect2 = effectAsset.ToEffect(owner.Object, owner.Object);
            
            effectManager.AddEffect(effect1);
            effectManager.AddEffect(effect2);
            
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void EffectManagerTest_RemoveGameplayEffectsWithTags_RemovesMatchingEffects()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);

            // Poison effect
            var poisonAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            var poisonTag = new Tag("Debuff.Poison");
            poisonAsset.AssetTags = new Tag[] { poisonTag };
            var poisonEffect = poisonAsset.ToEffect(owner.Object, owner.Object);
            effectManager.AddEffect(poisonEffect);
            
            Assert.AreEqual(1, effectManager.Effects.Count);
            
            // Antidote effect
            var antidoteAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            antidoteAsset.RemoveGameplayEffectsWithTags = new Tag[] { poisonTag };
            var antidoteEffect = antidoteAsset.ToEffect(owner.Object, owner.Object);
            effectManager.AddEffect(antidoteEffect);
            
            // Antidote removes poison and adds itself
            Assert.AreEqual(1, effectManager.Effects.Count);
            Assert.AreEqual(antidoteEffect, effectManager.Effects[0]);
        }

        [Test]
        public void EffectManagerTest_OngoingRequiredTags_SuspendsAndResumesEffect()
        {
            var owner = new Mock<IAbilitySystem>();
            
            // Initialize dependencies needed for effects
            var attributeSystem = new Mock<AttributeSetManager>(owner.Object);
            owner.Setup(mock => mock.AttributeSetManager).Returns(attributeSystem.Object);
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(mock => mock.TagManager).Returns(tagManager);

            var requirementTag = new Tag("State.Stunned");
            tagManager.AddTag(requirementTag); // Apply the tag manually
            
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.OngoingRequiredTags = new Tag[] { requirementTag };
            var effect = effectAsset.ToEffect(owner.Object, owner.Object);
            
            effectManager.AddEffect(effect);
            effect.Activate();
            
            Assert.IsTrue(effect.IsActive);
            
            var removalInvoked = false;
            effectManager.OnEffectSuspended += (e) => removalInvoked = true;
            
            tagManager.RemoveTag(requirementTag);
            
            Assert.IsFalse(effect.IsActive);
            Assert.IsTrue(removalInvoked);
            
            var addInvoked = false;
            effectManager.OnEffectResumed += (e) => addInvoked = true;
            
            tagManager.AddTag(requirementTag);
            
            Assert.IsTrue(effect.IsActive);
            Assert.IsTrue(addInvoked);
        }

        [Test]
        public void EffectManagerTest_RetractPredictedEffect_FiresOnEffectRemoved()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            var effect = effectAsset.ToEffect(owner.Object, owner.Object);
            var predictionKey = new AbilitySystem.Runtime.Networking.PredictionKey() { currentKey = 1 };
            
            effectManager.AddPredictedEffect(predictionKey, effect);
            
            var removalInvoked = false;
            effectManager.OnEffectRetracted += (e) => removalInvoked = true;
            
            effectManager.RetractPredictedEffect(predictionKey);
            
            Assert.IsTrue(removalInvoked);
            Assert.AreEqual(0, effectManager.PredictedEffects.Count);
        }
    }
}