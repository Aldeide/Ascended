using System;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using static AbilitySystem.Test.Utilities.EffectUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using static AbilitySystem.Test.Utilities.AbilityUtilities;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Effects
{
    public class EffectDefinitionTests
    {
        [Test]
        public void EffectDefinitionTests_PeriodicDefinition_ReturnsIsPeriodicTrue()
        {
            var abilitySystem = CreateMockServerAbilitySystem();
            var effectDefinition = CreateDurationEffectDefinition();
            effectDefinition.Period = 1.0f;

            Assert.IsTrue(effectDefinition.IsPeriodic());
        }
        
        [Test]
        public void EffectDefinitionTests_ZeroPeriod_ReturnsIsPeriodicFalse()
        {
            var effectDefinition = CreateDurationEffectDefinition();
            effectDefinition.Period = 0.0f;

            Assert.IsFalse(effectDefinition.IsPeriodic());
        }
        
        [Test]
        public void EffectDefinitionTests_InstantEffect_ReturnsIsPeriodicFalse()
        {
            var effectDefinition = CreateInstantEffectDefinition();

            Assert.IsFalse(effectDefinition.IsPeriodic());
        }
        
        [Test]
        public void EffectDefinitionTests_DurationEffect_ReturnsDurationalTrue()
        {
            var effectDefinition = CreateDurationEffectDefinition();

            Assert.IsTrue(effectDefinition.IsDurationalPolicy());
        }
        
        [Test]
        public void EffectDefinitionTests_InstantEffect_ReturnsDurationalFalse()
        {
            var effectDefinition = CreateInstantEffectDefinition();

            Assert.IsFalse(effectDefinition.IsDurationalPolicy());
        }
    }
}