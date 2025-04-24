using System.Collections.Generic;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
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
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
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
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            var effect = effectAsset.ToEffect(owner.Object, owner.Object);
            
            effectManager.AddEffect(effect);
            effectManager.RemoveEffect(effect);
            
            Assert.AreEqual(0, effectManager.Effects.Count);
        }
        
        [Test]
        public void EffectManagerTest_TickAsServer_TicksEffects()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSystem = new Mock<AttributeSetManager>(owner.Object);
            owner.Setup(mock => mock.IsLocalClient()).Returns(false);
            owner.Setup(mock => mock.GetTime()).Returns(1);
            owner.Setup(mock => mock.AttributeSetManager).Returns(attributeSystem.Object);
            var effectManager = new EffectManager(owner.Object);
            var effect = EffectUtilities.CreateDurationalEffect();
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
            owner.Setup(mock => mock.GetTime()).Returns(1);
            owner.Setup(mock => mock.AttributeSetManager).Returns(attributeSystem.Object);
            owner.Setup(mock => mock.EffectManager).Returns(effectManager);
            var effect = EffectUtilities.CreateDurationalEffect();
            effectManager.AddEffect(effect);
            effect.Initialise(owner.Object, owner.Object);
            effect.Activate();
            owner.Setup(mock => mock.GetTime()).Returns(200);
            effectManager.Tick();

            Assert.AreEqual(0, effectManager.Effects.Count);
        }
    }
}