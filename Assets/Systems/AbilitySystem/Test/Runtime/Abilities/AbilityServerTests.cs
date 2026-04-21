using System.Linq;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using static AbilitySystem.Test.Utilities.AbilityUtilities;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityServerTests
    {
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_NotUsableByClient()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(clientAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_UsableByServer()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(serverAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_GrantsTagToServer()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(serverAbilitySystem.TagManager.HasTag(new Tag("Tag.Test")));
            Assert.IsTrue(serverAbilitySystem.TagManager.AbilityTags.Count == 1);
        }
        
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_GrantsTagToClient()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.ReplicationManager.OnNotifyClientsAbilityTagsAdded += (data) =>
            {
                clientAbilitySystem.TagManager.AddAbilityTags(data);
            };
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(clientAbilitySystem.TagManager.HasTag(new Tag("Tag.Test")));
            Assert.IsTrue(clientAbilitySystem.TagManager.AbilityTags.Count == 1);
        }
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_GrantsMultipleTagToClient()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            var tags = new[] {new Tag("Tag.Test"), new Tag("Tag.Test2")};
            abilityDefinition.ActivationOwnedTags = tags;
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.ReplicationManager.OnNotifyClientsAbilityTagsAdded += (data) =>
            {
                clientAbilitySystem.TagManager.AddAbilityTags(data);
            };
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(clientAbilitySystem.TagManager.HasTag(new Tag("Tag.Test")));
            Assert.IsTrue(clientAbilitySystem.TagManager.HasTag(new Tag("Tag.Test2")));
            Assert.IsTrue(clientAbilitySystem.TagManager.AbilityTags.Count == 2);
        }
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_RemovesTagsOnClientWhenEnded()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            var tags = new[] {new Tag("Tag.Test"), new Tag("Tag.Test2")};
            abilityDefinition.ActivationOwnedTags = tags;
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.ReplicationManager.OnNotifyClientsAbilityTagsAdded += (data) =>
            {
                clientAbilitySystem.TagManager.AddAbilityTags(data);
            };
            serverAbilitySystem.ReplicationManager.OnNotifyClientsAbilityTagsRemoved += (data) =>
            {
                clientAbilitySystem.TagManager.RemoveAbilityTags(data);
            };
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(clientAbilitySystem.TagManager.HasTag(new Tag("Tag.Test")));
            Assert.IsTrue(clientAbilitySystem.TagManager.HasTag(new Tag("Tag.Test2")));
            Assert.IsTrue(clientAbilitySystem.TagManager.AbilityTags.Count == 2);
            
            serverAbilitySystem.AbilityManager.EndAbility(abilityDefinition.UniqueName);
            
            Debug.Log(clientAbilitySystem.TagManager.AbilityTags.Count);
            Assert.IsTrue(clientAbilitySystem.TagManager.AbilityTags.Count == 0);
        }
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_GrantsEffectToClient()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            var effectDefinition = EffectUtilities.CreateDurationEffectDefinition();
            abilityDefinition.GrantedEffects = new[] {effectDefinition};
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            // Link replication for effects
            var networkData = new EffectSyncData {
                
            };
            serverAbilitySystem.ReplicationManager.OnNotifyClientsEffectAdded += (data) =>
            {
                var effect = effectDefinition.ToEffect(serverAbilitySystem, clientAbilitySystem);
                effect.ActivationTime = data.ActivationTime;
                clientAbilitySystem.EffectManager.AddEffectFromServer(effect);
                networkData = data;
            };

            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(clientAbilitySystem.EffectManager.Effects.Count == 1);
            Assert.IsTrue(clientAbilitySystem.EffectManager.Effects.Exists(e => e.Definition.name == "TestDurationEffect"));
            Assert.IsTrue(networkData.EffectName == "TestDurationEffect");
            // This ability is non-predicted.
            Assert.IsFalse(networkData.PredictionKey.IsValidKey(), "Received a valid prediction key but the ability shouldn't be predicted");
        }
        
        public void AbilityServerTests_ServerOnlyAbility_RemovesEffectsOnClientWhenEnded()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            var effectDefinition = EffectUtilities.CreateDurationEffectDefinition();
            abilityDefinition.GrantedEffects = new[] {effectDefinition};
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            // Link replication for effects
            string networkData = "";
            serverAbilitySystem.ReplicationManager.OnNotifyClientsEffectAdded += (data) =>
            {
                var effect = effectDefinition.ToEffect(serverAbilitySystem, clientAbilitySystem);
                effect.ActivationTime = data.ActivationTime;
                clientAbilitySystem.EffectManager.AddEffectFromServer(effect);
            };
            var eventDispatched = false;
            serverAbilitySystem.ReplicationManager.OnNotifyClientsEffectRemoved += (effectName) =>
            {
                clientAbilitySystem.EffectManager.RemoveEffect(effectName);
                networkData = effectName;
                eventDispatched = true;
            };

            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            serverAbilitySystem.AbilityManager.EndAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(eventDispatched, "OnNotifyClientsEffectRemoved was not dispatched.");
            Assert.IsTrue(clientAbilitySystem.EffectManager.Effects.Count == 0, "Client effectManager has an effect when it shouldn't.");
            // This ability is non-predicted.
            Assert.IsTrue(networkData == "TestDurationEffect", "The wrong effect name was passed over the network.");
        }
    }
}