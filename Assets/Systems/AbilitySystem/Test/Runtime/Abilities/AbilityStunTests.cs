using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Abilities.StunAbility;
using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Events;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    [TestFixture]
    public class AbilityStunTests
    {
        private Mock<IAbilitySystem> _sourceClient;
        private Mock<IAbilitySystem> _sourceServer;
        private Mock<IAbilitySystem> _targetClient;
        private Mock<IAbilitySystem> _targetServer;

        private StunAbilityDefinition _stunAbilityDef;
        private TestAbilityDefinition _mainAbilityDef;
        private GameObject _targetGameObject;

        [SetUp]
        public void Setup()
        {
            _sourceClient = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            _sourceServer = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            AbilitySystemUtilities.LinkAbilitySystems(_sourceClient, _sourceServer);

            _targetClient = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            _targetServer = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            AbilitySystemUtilities.LinkAbilitySystems(_targetClient, _targetServer);

            // Setup Target NetworkObjectId
            Mock.Get(_targetServer.Object.NetworkRole).SetupGet(nr => nr.NetworkObjectId).Returns(2);
            Mock.Get(_targetClient.Object.NetworkRole).SetupGet(nr => nr.NetworkObjectId).Returns(2);

            _targetGameObject = new GameObject("Target");
            var targetComponent = _targetGameObject.AddComponent<DummyAbilitySystemComponent>();
            targetComponent.MockSystem = _targetServer.Object;

            // Source systems need to be able to resolve the target object
            _sourceServer.Setup(x => x.GetGameObjectFromNetworkId(2)).Returns(_targetGameObject);
            _sourceClient.Setup(x => x.GetGameObjectFromNetworkId(2)).Returns(_targetGameObject);
            
            _stunAbilityDef = ScriptableObject.CreateInstance<StunAbilityDefinition>();
            _stunAbilityDef.UniqueName = "StunAbility";
            _stunAbilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            _stunAbilityDef.CancelAbilityTags = new Tag[] { new Tag("Ability.Active") };

            _mainAbilityDef = AbilityUtilities.CreatePredictedAbilityDefinition();
            _mainAbilityDef.UniqueName = "MainAbility";
            _mainAbilityDef.AssetTags = new Tag[] { new Tag("Ability.Active") };
        }



        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_targetGameObject);
        }



        [Test]
        public void StunAbility_RealImplementation_AppliesEffectToTarget()
        {
            // 1. Setup Stun Effect
            var stunEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            stunEffectDef.DurationType = EffectDurationType.FixedDuration;
            stunEffectDef.DurationSeconds = 5f;
            stunEffectDef.GrantedTags = new Tag[] { new Tag("Status.Stun") };
            _stunAbilityDef.GrantedEffects = new[] { stunEffectDef };

            // 2. Setup mock resolution on Source Server
            ulong targetId = 12345;
            _sourceServer.Setup(x => x.GetGameObjectFromNetworkId(targetId)).Returns(_targetGameObject);

            // 3. Grant ability
            _sourceServer.Object.AbilityManager.GrantAbility(_stunAbilityDef);

            // 4. Prepare TargetData
            var targetData = new TargetDataHandle();
            targetData.Add(new TargetDataActor { NetworkObjectId = targetId });
            var abilityData = new AbilityData { TargetData = targetData };

            // 5. Activate on source server
            bool success = _sourceServer.Object.AbilityManager.TryActivateAbility(_stunAbilityDef.UniqueName, abilityData);
            Assert.IsTrue(success);

            // 6. Verification: Target should have the stun tag
            Assert.IsTrue(_targetServer.Object.TagManager.HasTag(new Tag("Status.Stun")));
        }

        [Test]
        public void StunAbility_Cancellation_CancelsActiveAbilityOnTarget()
        {
            // 1. Setup target with an active ability that has the "Ability.Active" tag
            _mainAbilityDef.AssetTags = new[] { new Tag("Ability.Active") };
            _targetServer.Object.AbilityManager.GrantAbility(_mainAbilityDef);
            _targetServer.Object.AbilityManager.TryActivateAbility(_mainAbilityDef.UniqueName);
            var activeAbility = _targetServer.Object.AbilityManager.Abilities[_mainAbilityDef.UniqueName];
            Assert.IsTrue(activeAbility.IsActive);

            // 2. Setup Stun Ability with effects
            var stunEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            stunEffectDef.DurationType = EffectDurationType.FixedDuration;
            stunEffectDef.DurationSeconds = 5f;
            stunEffectDef.GrantedTags = new Tag[] { new Tag("Status.Stun") };
            _stunAbilityDef.GrantedEffects = new[] { stunEffectDef };

            // 3. Source activates Stun Ability on Target
            // NetworkObjectId 2 is the target (from Setup)
            var targetData = new TargetDataHandle();
            targetData.Add(new TargetDataActor { NetworkObjectId = 2 });
            var abilityData = new AbilityData { TargetData = targetData };

            _sourceServer.Object.AbilityManager.GrantAbility(_stunAbilityDef);
            _sourceServer.Object.AbilityManager.TryActivateAbility(_stunAbilityDef.UniqueName, abilityData);

            // 4. Verification: Main ability on target should be cancelled by StunAbility's explicit call
            Assert.IsFalse(activeAbility.IsActive, "Active ability on target should be cancelled by StunAbility");
            Assert.IsTrue(_targetServer.Object.TagManager.HasTag(new Tag("Status.Stun")), "Target should have Stun tag from the ability's granted effect");
        }

        [Test]
        public void StunAbility_Immunity_BlocksOnServer()
        {
            // 1. Setup Stun Effect with immunity
            var stunEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            stunEffectDef.DurationType = EffectDurationType.FixedDuration;
            stunEffectDef.DurationSeconds = 5f;
            stunEffectDef.GrantedTags = new Tag[] { new Tag("Status.Stun") };
            stunEffectDef.ApplicationImmunityTags = new Tag[] { new Tag("Status.Immune.Stun") };
            _stunAbilityDef.GrantedEffects = new[] { stunEffectDef };

            // 2. Give target immunity
            _targetServer.Object.TagManager.AddTag(new Tag("Status.Immune.Stun"));

            // 3. Source attempts to stun target
            ulong targetId = 12345;
            _sourceServer.Setup(x => x.GetGameObjectFromNetworkId(targetId)).Returns(_targetGameObject);
            _sourceServer.Object.AbilityManager.GrantAbility(_stunAbilityDef);

            var targetData = new TargetDataHandle();
            targetData.Add(new TargetDataActor { NetworkObjectId = targetId });
            var abilityData = new AbilityData { TargetData = targetData };

            _sourceServer.Object.AbilityManager.TryActivateAbility(_stunAbilityDef.UniqueName, abilityData);

            // 4. Verification: Target should NOT have the stun tag
            Assert.IsFalse(_targetServer.Object.TagManager.HasTag(new Tag("Status.Stun")));
        }
    }
}