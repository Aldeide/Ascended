using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class StunAbilityTests : AbilitySystemInteractionSyncTestBase
    {
        private StunAbilityDefinition _stunAbilityDef;
        private EffectDefinition _stunEffectDef;
        private StunTestAbilityDefinition _activeAbilityDef;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Setup Stun Effect
            _stunEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            _stunEffectDef.name = "StunEffect";
            _stunEffectDef.GrantedTags = new[] { new Tag("Status.Debuff.Stun") };
            _stunEffectDef.DurationType = EffectDurationType.Infinite;
            DataManager.Effects[_stunEffectDef.name] = _stunEffectDef;

            // Setup Stun Ability
            _stunAbilityDef = ScriptableObject.CreateInstance<StunAbilityDefinition>();
            _stunAbilityDef.UniqueName = "StunAbility";
            _stunAbilityDef.GrantedEffects = new[] { _stunEffectDef };
            _stunAbilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            _stunAbilityDef.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            DataManager.Abilities[_stunAbilityDef.UniqueName] = _stunAbilityDef;

            // Setup an "Active" ability to be cancelled
            _activeAbilityDef = ScriptableObject.CreateInstance<StunTestAbilityDefinition>();
            _activeAbilityDef.UniqueName = "ActiveAbility";
            _activeAbilityDef.AssetTags = new[] { new Tag("Ability.Active") };
            _activeAbilityDef.NetworkPolicy = AbilityNetworkPolicy.Server;
            DataManager.Abilities[_activeAbilityDef.UniqueName] = _activeAbilityDef;

            // Grant abilities
            SourceServer.AbilityManager.GrantAbility(_stunAbilityDef);
            SourceClient.AbilityManager.GrantAbility(_stunAbilityDef);
            TargetServer.AbilityManager.GrantAbility(_activeAbilityDef);
            TargetClient.AbilityManager.GrantAbility(_activeAbilityDef);
        }

        [Test]
        public void StunAbility_CancelsTargetActiveAbilities()
        {
            TargetServer.AbilityManager.TryActivateAbility("ActiveAbility");
            TickAll();
            Assert.IsTrue(TargetServer.AbilityManager.Abilities["ActiveAbility"].IsActive, "Target ability should be active before stun on the server.");
            var targetData = new TargetDataHandle();
            targetData.Data.Add(new TargetDataActor { NetworkObjectId = TargetServer.NetworkRole.NetworkObjectId });
            var abilityData = new AbilityData { TargetData = targetData };
            
            SourceServer.AbilityManager.TryActivateAbility("StunAbility", abilityData);
            
            Assert.IsFalse(TargetServer.AbilityManager.Abilities["ActiveAbility"].IsActive, "Target ability should be cancelled by stun");
        }

        [Test]
        public void StunAbility_AppliesStunEffectToTarget()
        {
            // Arrange
            var targetData = new TargetDataHandle();
            targetData.Data.Add(new TargetDataActor { NetworkObjectId = TargetServer.NetworkRole.NetworkObjectId });
            var abilityData = new AbilityData { TargetData = targetData };

            // Act
            SourceClient.AbilityManager.TryActivateAbility("StunAbility", abilityData);

            // Assert
            Assert.IsTrue(TargetServer.EffectManager.Effects.Any(e => e.Definition == _stunEffectDef), "Target should have the stun effect applied on the server.");
            Assert.IsTrue(TargetClient.EffectManager.Effects.Any(e => e.Definition == _stunEffectDef), "Target should have the stun effect applied on the client.");
        }
        
        [Test]
        public void StunAbility_AppliesStunTagToTarget()
        {
            // Arrange
            var targetData = new TargetDataHandle();
            targetData.Data.Add(new TargetDataActor { NetworkObjectId = TargetServer.NetworkRole.NetworkObjectId });
            var abilityData = new AbilityData { TargetData = targetData };

            // Act
            SourceClient.AbilityManager.TryActivateAbility("StunAbility", abilityData);

            // Assert
            Assert.IsTrue(TargetServer.TagManager.HasTag(new Tag("Status.Debug.Stun")), "Target should have the stun tag applied on the server.");
            Assert.IsTrue(TargetClient.TagManager.HasTag(new Tag("Status.Debug.Stun")), "Target should have the stun tag applied on the client.");
        }

        [Test]
        public void StunAbility_ReplicatesToClients()
        {
            // Arrange
            var targetData = new TargetDataHandle();
            targetData.Data.Add(new TargetDataActor { NetworkObjectId = TargetServer.NetworkRole.NetworkObjectId });
            var abilityData = new AbilityData { TargetData = targetData };

            // Act
            SourceServer.AbilityManager.TryActivateAbility("StunAbility", abilityData);

            // Assert: Effect should replicate to target client
            Assert.IsTrue(TargetClient.EffectManager.Effects.Any(e => e.Definition == _stunEffectDef), "Stun effect should replicate to Target Client");
        }

        [Test]
        public void StunAbility_ClientPrediction_AppliesEffectLocally()
        {
            // Arrange
            var targetData = new TargetDataHandle();
            targetData.Data.Add(new TargetDataActor { NetworkObjectId = TargetServer.NetworkRole.NetworkObjectId });
            var abilityData = new AbilityData { TargetData = targetData };

            // Act: Client activates
            SourceClient.AbilityManager.TryActivateAbility("StunAbility", abilityData);

            // Assert: Effect should be on the target (replicated or predicted if mock allows)
            // In this mock setup, it should be on TargetClient due to replication link.
            Assert.IsTrue(TargetClient.EffectManager.Effects.Any(e => e.Definition == _stunEffectDef), "Target Client should have the effect applied");
        }
    }

    // Helper for testing (Moved outside to avoid potential serialization issues with nested classes)
    public class StunTestAbilityDefinition : AbilityDefinition
    {
        public override System.Type AbilityType() => typeof(StunTestAbility);
        public override Ability ToAbility(IAbilitySystem owner) => new StunTestAbility(this, owner);
    }

    public class StunTestAbility : Ability
    {
        public StunTestAbility(AbilityDefinition definition, IAbilitySystem owner) : base(definition, owner) { }
        protected override void ActivateAbility(AbilityData data) { }
        public override void EndAbility() { }
    }
}