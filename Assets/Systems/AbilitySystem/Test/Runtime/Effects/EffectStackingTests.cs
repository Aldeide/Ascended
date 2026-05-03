using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

namespace AbilitySystem.Test.Runtime.Effects
{
    /// <summary>
    /// Unit tests for effect stacking, verifying aggregation types, stack limits, and overflow policies.
    /// </summary>
    public class EffectStackingTests : AbilitySystemTestBase
    {
        private EffectManager _manager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _manager = new EffectManager(Source);
        }

        /// <summary>
        /// Validates that effects with 'AggregateByTarget' stacking correctly increment 
        /// the stack count on the existing effect instance.
        /// </summary>
        [Test]
        public void EffectStackingTests_AggregateByTarget_IncrementsExistingEffectStacks()
        {
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            def.DurationType = EffectDurationType.Infinite;
            def.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 5
            };
            
            var effect1 = new Effect(def);
            effect1.Initialise(Source, Target);
            
            _manager.AddEffect(effect1);
            Assert.AreEqual(1, _manager.Effects.Count);
            Assert.AreEqual(1, _manager.Effects[0].NumStacks);
            
            var effect2 = new Effect(def);
            effect2.Initialise(Source, Target);
            _manager.AddEffect(effect2);
            
            Assert.AreEqual(1, _manager.Effects.Count);
            Assert.AreEqual(2, _manager.Effects[0].NumStacks);
        }

        /// <summary>
        /// Validates that the number of stacks cannot exceed the maximum specified in the effect definition.
        /// </summary>
        [Test]
        public void EffectStackingTests_MaxStacks_ClampsStackCountAtLimit()
        {
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            def.DurationType = EffectDurationType.Infinite;
            def.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 2
            };
            
            _manager.AddEffect(new Effect(def).WithInitialise(Source, Target));
            _manager.AddEffect(new Effect(def).WithInitialise(Source, Target));
            _manager.AddEffect(new Effect(def).WithInitialise(Source, Target));
            
            Assert.AreEqual(1, _manager.Effects.Count);
            Assert.AreEqual(2, _manager.Effects[0].NumStacks);
        }

        /// <summary>
        /// Validates that the DenyOverflow application result is returned when an effect 
        /// exceeds its stack limit and the policy specifies denial.
        /// </summary>
        [Test]
        public void EffectStackingTests_OverflowPolicyDeny_ReturnsOverflowDenyResult()
        {
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            def.DurationType = EffectDurationType.Infinite;
            def.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 1,
                EffectStackOverflowPolicy = new EffectStackOverflowPolicy { DenyOverflowApplication = true }
            };
            
            _manager.AddEffect(new Effect(def).WithInitialise(Source, Target));
            var result = _manager.AddEffect(new Effect(def).WithInitialise(Source, Target));
            
            Assert.AreEqual(EffectApplicationResult.OverflowDeny, result);
            Assert.AreEqual(1, _manager.Effects[0].NumStacks);
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
