using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

namespace AbilitySystem.Test.Runtime.Effects
{
    public class EffectStackingTests
    {
        [Test]
        public void EffectStacking_AggregateByTarget_IncrementsStacks()
        {
            var mockSys = AbilitySystemUtilities.CreateMockAbilitySystem();
            var manager = new EffectManager(mockSys.Object);
            
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            def.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 5
            };
            
            var effect1 = new Effect(def);
            effect1.Initialise(mockSys.Object, mockSys.Object);
            
            manager.AddEffect(effect1);
            Assert.AreEqual(1, manager.Effects.Count);
            Assert.AreEqual(1, manager.Effects[0].NumStacks);
            
            var effect2 = new Effect(def);
            effect2.Initialise(mockSys.Object, mockSys.Object);
            manager.AddEffect(effect2);
            
            Assert.AreEqual(1, manager.Effects.Count);
            Assert.AreEqual(2, manager.Effects[0].NumStacks);
        }

        [Test]
        public void EffectStacking_MaxStacks_RespectsLimit()
        {
            var mockSys = AbilitySystemUtilities.CreateMockAbilitySystem();
            var manager = new EffectManager(mockSys.Object);
            
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            def.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 2
            };
            
            manager.AddEffect(new Effect(def).WithInitialise(mockSys.Object, mockSys.Object));
            manager.AddEffect(new Effect(def).WithInitialise(mockSys.Object, mockSys.Object));
            var result = manager.AddEffect(new Effect(def).WithInitialise(mockSys.Object, mockSys.Object));
            
            Assert.AreEqual(1, manager.Effects.Count);
            Assert.AreEqual(2, manager.Effects[0].NumStacks);
        }

        [Test]
        public void EffectStacking_DenyOverflow_ReturnsFailure()
        {
            var mockSys = AbilitySystemUtilities.CreateMockAbilitySystem();
            var manager = new EffectManager(mockSys.Object);
            
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            def.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 1,
                EffectStackOverflowPolicy = new EffectStackOverflowPolicy { DenyOverflowApplication = true }
            };
            
            manager.AddEffect(new Effect(def).WithInitialise(mockSys.Object, mockSys.Object));
            var result = manager.AddEffect(new Effect(def).WithInitialise(mockSys.Object, mockSys.Object));
            
            Assert.AreEqual(EffectApplicationResult.OverflowDeny, result);
            Assert.AreEqual(1, manager.Effects[0].NumStacks);
        }
    }

    public static class EffectTestExtensions
    {
        public static Effect WithInitialise(this Effect effect, AbilitySystem.Runtime.Core.IAbilitySystem source, AbilitySystem.Runtime.Core.IAbilitySystem target)
        {
            effect.Initialise(source, target);
            return effect;
        }
    }
}
