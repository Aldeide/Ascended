using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for server-authoritative abilities, ensuring correct permission enforcement and state replication to clients.
    /// </summary>
    public class AbilityServerTests : AbilitySystemTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            // We need a specific Client/Server split for most of these tests
            SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            
            base.SetUp();
        }

        /// <summary>
        /// Verifies that an ability with 'Server' network policy cannot be directly activated by a client.
        /// </summary>
        [Test]
        public void AbilityServerTests_ServerPolicy_CannotBeStartedByClient()
        {
            var abilityDefinition = AbilityUtilities.CreateServerAbilityDefinition();
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            bool success = Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(success, "Client should not be able to start a server-only ability");
            Assert.IsFalse(Source.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
        
        /// <summary>
        /// Verifies that a server-only ability can be correctly activated when requested on the server.
        /// </summary>
        [Test]
        public void AbilityServerTests_ServerPolicy_CanBeStartedByServer()
        {
            var abilityDefinition = AbilityUtilities.CreateServerAbilityDefinition();
            Target.AbilityManager.GrantAbility(abilityDefinition);
            
            bool success = Target.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(success, "Server should be able to start its own ability");
            Assert.IsTrue(Target.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
        
        /// <summary>
        /// Verifies that a server-authoritative ability correctly applies its 'ActivationOwnedTags' to the server's tag manager.
        /// </summary>
        [Test]
        public void AbilityServerTests_ServerActivation_GrantsTagsToServer()
        {
            var abilityDefinition = AbilityUtilities.CreateServerAbilityDefinition();
            var testTag = new Tag("Tag.Test");
            abilityDefinition.ActivationOwnedTags = new[] { testTag };
            Target.AbilityManager.GrantAbility(abilityDefinition);
            
            Target.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(Target.TagManager.HasTag(testTag), "Server should have the ability tag");
            Assert.AreEqual(1, Target.TagManager.AbilityTags.Count);
        }
        
        /// <summary>
        /// Verifies that tags granted by a server ability are correctly replicated to the client via the replication manager.
        /// </summary>
        [Test]
        public void AbilityServerTests_ServerActivation_ReplicatesTagsToClient()
        {
            var abilityDefinition = AbilityUtilities.CreateServerAbilityDefinition();
            var testTag = new Tag("Tag.Test");
            abilityDefinition.ActivationOwnedTags = new[] { testTag };
            Target.AbilityManager.GrantAbility(abilityDefinition);
            
            // Link tag replication
            Target.ReplicationManager.OnNotifyClientsAbilityTagsAdded += (data) => Source.TagManager.AddAbilityTags(data);
            
            Target.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(Source.TagManager.HasTag(testTag), "Client should have received replicated tag");
            Assert.AreEqual(1, Source.TagManager.AbilityTags.Count);
        }
        
        /// <summary>
        /// Verifies that tags replicated to a client are correctly removed once the server ability ends.
        /// </summary>
        [Test]
        public void AbilityServerTests_AbilityEnd_RemovesReplicatedTagsOnClient()
        {
            var abilityDefinition = AbilityUtilities.CreateServerAbilityDefinition();
            var tags = new[] { new Tag("Tag.Test"), new Tag("Tag.Test2") };
            abilityDefinition.ActivationOwnedTags = tags;
            Target.AbilityManager.GrantAbility(abilityDefinition);
            
            // Link replication
            Target.ReplicationManager.OnNotifyClientsAbilityTagsAdded += (data) => Source.TagManager.AddAbilityTags(data);
            Target.ReplicationManager.OnNotifyClientsAbilityTagsRemoved += (data) => Source.TagManager.RemoveAbilityTags(data);
            
            Target.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            Assert.AreEqual(2, Source.TagManager.AbilityTags.Count);
            
            Target.AbilityManager.EndAbility(abilityDefinition.UniqueName);
            
            Assert.AreEqual(0, Source.TagManager.AbilityTags.Count, "Client should have removed replicated tags");
        }
        
        /// <summary>
        /// Verifies that effects granted by a server ability are correctly replicated and added to the client's effect manager.
        /// </summary>
        [Test]
        public void AbilityServerTests_ServerActivation_ReplicatesEffectsToClient()
        {
            var abilityDefinition = AbilityUtilities.CreateServerAbilityDefinition();
            var effectDefinition = EffectUtilities.CreateDurationEffectDefinition();
            abilityDefinition.GrantedEffects = new[] { effectDefinition };
            Target.AbilityManager.GrantAbility(abilityDefinition);
            
            EffectSyncData networkData = default;
            Target.ReplicationManager.OnNotifyClientsEffectAdded += (data) =>
            {
                var effect = effectDefinition.ToEffect(Target, Source);
                effect.ActivationTime = data.ActivationTime;
                Source.EffectManager.AddEffectFromServer(effect);
                networkData = data;
            };

            Target.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.AreEqual(1, Source.EffectManager.Effects.Count);
            Assert.AreEqual("TestDurationEffect", networkData.EffectName);
            Assert.IsFalse(networkData.PredictionKey.IsValidKey(), "Non-predicted ability should not have a valid prediction key");
        }
        
        /// <summary>
        /// Verifies that effects replicated to a client are correctly removed when the server ability ends.
        /// </summary>
        [Test]
        public void AbilityServerTests_AbilityEnd_RemovesReplicatedEffectsOnClient()
        {
            var abilityDefinition = AbilityUtilities.CreateServerAbilityDefinition();
            var effectDefinition = EffectUtilities.CreateDurationEffectDefinition();
            abilityDefinition.GrantedEffects = new[] { effectDefinition };
            Target.AbilityManager.GrantAbility(abilityDefinition);
            
            Target.ReplicationManager.OnNotifyClientsEffectAdded += (data) =>
            {
                var effect = effectDefinition.ToEffect(Target, Source);
                effect.ActivationTime = data.ActivationTime;
                Source.EffectManager.AddEffectFromServer(effect);
            };

            bool removalDispatched = false;
            Target.ReplicationManager.OnNotifyClientsEffectRemoved += (effectName) =>
            {
                Source.EffectManager.RemoveEffect(effectName);
                removalDispatched = true;
            };

            Target.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            Target.AbilityManager.EndAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(removalDispatched, "Effect removal should have been replicated");
            Assert.AreEqual(0, Source.EffectManager.Effects.Count, "Client should have no active effects");
        }
    }
}