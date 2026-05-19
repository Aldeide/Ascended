using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Effects
{
    public class MockApplicationRequirement : EffectApplicationRequirement
    {
        public bool AllowApplication;

        public override bool CanApplyEffect(IAbilitySystem target, IAbilitySystem source, Effect effect)
        {
            return AllowApplication;
        }
    }

    public class EffectApplicationRequirementTests : AbilitySystemTestBase
    {

        [Test]
        public void ApplyEffect_WhenRequirementFails_ReturnsFailedCustomRequirement()
        {
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            var requirement = ScriptableObject.CreateInstance<MockApplicationRequirement>();
            requirement.AllowApplication = false;
            def.ApplicationRequirements = new[] { requirement };

            var effect = new Effect(def);
            effect.Initialise(Source, Target);
            var manager = new EffectManager(Target);

            var result = manager.AddEffect(effect);

            Assert.AreEqual(EffectApplicationResult.FailedCustomRequirement, result);
        }

        [Test]
        public void ApplyEffect_WhenRequirementPasses_ReturnsSuccess()
        {
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            var requirement = ScriptableObject.CreateInstance<MockApplicationRequirement>();
            requirement.AllowApplication = true;
            def.ApplicationRequirements = new[] { requirement };

            var effect = new Effect(def);
            effect.Initialise(Source, Target);
            var manager = new EffectManager(Target);

            var result = manager.AddEffect(effect);

            Assert.AreEqual(EffectApplicationResult.Success, result);
        }
    }
}
