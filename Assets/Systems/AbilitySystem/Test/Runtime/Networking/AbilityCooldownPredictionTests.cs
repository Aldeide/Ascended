using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Test.Runtime.Networking
{
    [TestFixture]
    public class AbilityCooldownPredictionTests : AbilitySystemSyncTestBase
    {
        private EffectDefinition _cooldownEffectDef;
        private TestAbilityDefinition _testAbilityDef;

        public override void Setup()
        {
            base.Setup();

            // 1. Create Cooldown Effect
            _cooldownEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            _cooldownEffectDef.name = "CooldownEffect";
            _cooldownEffectDef.DurationType = EffectDurationType.FixedDuration;
            _cooldownEffectDef.DurationSeconds = 5f;
            
            DataManager.Effects.Add(_cooldownEffectDef.name, _cooldownEffectDef);

            // 2. Create Ability with Cooldown
            _testAbilityDef = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            _testAbilityDef.UniqueName = "TestAbility";
            _testAbilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            _testAbilityDef.Cooldown = new TestCooldown { CooldownEffect = _cooldownEffectDef };
            
            DataManager.Abilities.Add(_testAbilityDef.UniqueName, _testAbilityDef);

            // 3. Grant to both
            ServerSystem.AbilityManager.GrantAbility(_testAbilityDef);
            ClientSystem.AbilityManager.GrantAbility(_testAbilityDef);
        }

        [Test]
        public void ClientActivatesAbility_CooldownEffectIsPredictedWithKey()
        {
            ClientSystem.AbilityManager.TryActivateAbility("TestAbility");
            
            var predictedEffect = ClientSystem.EffectManager.Effects.FirstOrDefault(e => e.Definition == _cooldownEffectDef);
            var ability = ClientSystem.AbilityManager.Abilities["TestAbility"];
            Assert.IsNotNull(predictedEffect, "Cooldown effect should be applied locally on the client");
            Assert.AreEqual(ability.PredictionKey, predictedEffect.PredictionKey, "Predicted cooldown effect should have the matching PredictionKey from the ability");
            Assert.IsTrue(predictedEffect.IsPredicted(), "Cooldown effect should be marked as predicted");
        }

        [Test]
        public void ClientPredictsCooldown_ServerConfirms_EffectIsReconciled()
        {
            ClientSystem.AbilityManager.TryActivateAbility("TestAbility");
            
            Assert.AreEqual(1, ClientSystem.EffectManager.Effects.Count(e => e.Definition == _cooldownEffectDef));
            Assert.AreEqual(1, ServerSystem.EffectManager.Effects.Count(e => e.Definition == _cooldownEffectDef), "Server should have applied the cooldown");
            Assert.AreEqual(1, ClientSystem.EffectManager.Effects.Count(e => e.Definition == _cooldownEffectDef), "Client should still have only 1 cooldown effect after reconciliation");
        }

        // Helper classes for testing
        public class TestAbilityDefinition : AbilityDefinition
        {
            public override System.Type AbilityType() => typeof(TestAbility);
            public override Ability ToAbility(IAbilitySystem owner) => new TestAbility(this, owner);
        }

        // TODO(Jules): I'm sure we already have this somewhere, but I'm not sure where.
        public class TestAbility : Ability
        {
            public TestAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner) { }
            protected override void ActivateAbility(AbilityData data) { }
            public override void EndAbility() { }
        }

        [System.Serializable]
        public class TestCooldown : AbilityCooldown
        {
            public override float Calculate(IAbilitySystem owner) => 5f;
        }
    }
}
