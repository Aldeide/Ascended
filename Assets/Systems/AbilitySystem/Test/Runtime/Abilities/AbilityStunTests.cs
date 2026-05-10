using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for the StunAbility, verifying its ability to apply effects to targets, cancel active abilities, and respect immunities.
    /// </summary>
    public class AbilityStunTests : AbilitySystemTestBase
    {
        private StunAbilityDefinition _stunAbilityDef;
        private TestAbilityDefinition _mainAbilityDef;
        private GameObject _targetGameObject;
        private IAbilitySystem _targetServer;

        [SetUp]
        public override void SetUp()
        {
            // For stun tests, we specifically need a Source Server and a Target Server
            SourceMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            _targetServer = Target;

            // Setup Target NetworkObjectId
            Mock.Get(_targetServer.NetworkRole).SetupGet(nr => nr.NetworkObjectId).Returns(2);

            _targetGameObject = new GameObject("TargetActor");
            var targetComponent = _targetGameObject.AddComponent<DummyAbilitySystemComponent>();
            targetComponent.MockSystem = _targetServer;

            // Source system needs to be able to resolve the target object
            SourceMock.Setup(x => x.GetGameObjectFromNetworkId(2)).Returns(_targetGameObject);
            
            _stunAbilityDef = ScriptableObject.CreateInstance<StunAbilityDefinition>();
            _stunAbilityDef.UniqueName = "StunAbility";
            _stunAbilityDef.NetworkPolicy = AbilityNetworkPolicy.Server;
            _stunAbilityDef.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            _stunAbilityDef.CancelAbilityTags = new[] { new Tag("Ability.Active") };

            _mainAbilityDef = AbilityUtilities.CreatePredictedAbilityDefinition();
            _mainAbilityDef.UniqueName = "MainAbility";
            _mainAbilityDef.AssetTags = new[] { new Tag("Ability.Active") };
            
            base.SetUp();
        }

        [TearDown]
        public new void TearDown()
        {
            if (_targetGameObject != null)
            {
                Object.DestroyImmediate(_targetGameObject);
            }
        }

        /// <summary>
        /// Verifies that a stun ability correctly applies its associated effect to a target actor.
        /// </summary>
        [Test]
        public void AbilityStunTests_Activation_AppliesStunEffectToTarget()
        {
            var stunEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            stunEffectDef.DurationType = EffectDurationType.FixedDuration;
            stunEffectDef.DurationSeconds = 5f;
            stunEffectDef.GrantedTags = new[] { new Tag("Status.Stun") };
            _stunAbilityDef.GrantedEffects = new[] { stunEffectDef };

            Source.AbilityManager.GrantAbility(_stunAbilityDef);

            var targetData = new TargetDataHandle();
            targetData.Add(new TargetDataActor { NetworkObjectId = 2 });
            var abilityData = new AbilityData { TargetData = targetData };

            bool success = Source.AbilityManager.TryActivateAbility("StunAbility", abilityData);
            
            Assert.IsTrue(success, "Stun ability should have activated");
            Assert.IsTrue(_targetServer.TagManager.HasTag(new Tag("Status.Stun")), "Target should have received the stun tag");
        }

        /// <summary>
        /// Verifies that applying a stun ability to a target correctly cancels any active abilities on that target that possess blocked tags.
        /// </summary>
        [Test]
        public void AbilityStunTests_StunTarget_CancelsActiveAbilitiesOnTarget()
        {
            // 1. Setup target with an active ability
            _targetServer.AbilityManager.GrantAbility(_mainAbilityDef);
            _targetServer.AbilityManager.TryActivateAbility("MainAbility");
            var activeAbility = _targetServer.AbilityManager.Abilities["MainAbility"];
            Assert.IsTrue(activeAbility.IsActive);

            // 2. Setup Stun Ability
            var stunEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            stunEffectDef.DurationType = EffectDurationType.FixedDuration;
            stunEffectDef.DurationSeconds = 5f;
            stunEffectDef.GrantedTags = new[] { new Tag("Status.Stun") };
            _stunAbilityDef.GrantedEffects = new[] { stunEffectDef };

            Source.AbilityManager.GrantAbility(_stunAbilityDef);

            // 3. Source activates Stun on Target
            var targetData = new TargetDataHandle();
            targetData.Add(new TargetDataActor { NetworkObjectId = 2 });
            var abilityData = new AbilityData { TargetData = targetData };

            Source.AbilityManager.TryActivateAbility("StunAbility", abilityData);

            // 4. Verification
            Assert.IsFalse(activeAbility.IsActive, "Active ability on target should have been cancelled by stun");
            Assert.IsTrue(_targetServer.TagManager.HasTag(new Tag("Status.Stun")), "Target should have received stun tag");
        }

        /// <summary>
        /// Verifies that a target with stun immunity correctly blocks the application of stun effects from the ability.
        /// </summary>
        [Test]
        public void AbilityStunTests_ImmuneTarget_DoesNotReceiveStunEffect()
        {
            var stunEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            stunEffectDef.DurationType = EffectDurationType.FixedDuration;
            stunEffectDef.DurationSeconds = 5f;
            stunEffectDef.GrantedTags = new[] { new Tag("Status.Stun") };
            stunEffectDef.ApplicationImmunityTags = new[] { new Tag("Status.Immune.Stun") };
            _stunAbilityDef.GrantedEffects = new[] { stunEffectDef };

            // Apply immunity to target
            _targetServer.TagManager.AddTag(new Tag("Status.Immune.Stun"));

            Source.AbilityManager.GrantAbility(_stunAbilityDef);

            var targetData = new TargetDataHandle();
            targetData.Add(new TargetDataActor { NetworkObjectId = 2 });
            var abilityData = new AbilityData { TargetData = targetData };

            Source.AbilityManager.TryActivateAbility("StunAbility", abilityData);

            Assert.IsFalse(_targetServer.TagManager.HasTag(new Tag("Status.Stun")), "Immune target should not have received stun tag");
        }
    }
}