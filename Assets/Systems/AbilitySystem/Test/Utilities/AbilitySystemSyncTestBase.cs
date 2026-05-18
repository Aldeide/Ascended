using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using UnityEngine;
using NUnit.Framework;
using UnityEditor.PackageManager;

namespace AbilitySystem.Test.Utilities
{
    public abstract class AbilitySystemSyncTestBase
    {
        protected AbilitySystemManager ServerSystem;
        protected AbilitySystemManager ClientSystem;
        protected MockDataManager DataManager;

        [SetUp]
        public virtual void Setup()
        {
            DataManager = new MockDataManager();
            
            ServerSystem = new AbilitySystemManager(DataManager);
            ServerSystem.NetworkRole = new MockNetworkRole(true, true, 1);
            ServerSystem.ReplicationManager = new MockReplicationManager(ServerSystem) { DataManager = DataManager };
            
            ClientSystem = new AbilitySystemManager(DataManager);
            ClientSystem.NetworkRole = new MockNetworkRole(false, true, 1);
            ClientSystem.ReplicationManager = new MockReplicationManager(ClientSystem) { DataManager = DataManager };

            LinkSystems();
        }

        protected void LinkSystems()
        {
            var serverRepl = ServerSystem.ReplicationManager;
            var clientRepl = ClientSystem.ReplicationManager;

            // Link Attributes
            serverRepl.OnNotifyClientsAttributeBaseValueChanged += (name, val) => 
                ClientSystem.AttributeSetManager.GetAttribute(name).SetBaseValue(val);
            
            serverRepl.OnNotifyClientsAttributeCurrentValueChanged += (name, oldVal, newVal) => 
                ClientSystem.AttributeSetManager.GetAttribute(name).SetCurrentValue(newVal);

            // Link Effects
            serverRepl.OnNotifyClientsEffectAdded += (data) => 
                ClientSystem.ReplicationManager.ProcessClientEffectAdded(data);

            serverRepl.OnNotifyClientsEffectRemoved += (name) => 
                ClientSystem.ReplicationManager.ProcessClientEffectRemoved(name);

            serverRepl.OnNotifyClientsAbilityChargesChanged += (name, curr, max) =>
                ClientSystem.ReplicationManager.ProcessClientAbilityChargesChanged(name, curr, max);

            // Link Abilities
            serverRepl.OnNotifyClientAbilityGranted += (def) => 
            {
                ClientSystem.AbilityManager.GrantAbility(def);
            };

            serverRepl.OnClientActivateAbility += (name, data) => 
                ClientSystem.AbilityManager.ForceActivateAbility(name, data);

            serverRepl.OnClientEndAbility += (name) => 
            {
                if (ClientSystem.AbilityManager.Abilities.TryGetValue(name, out var ability))
                {
                    ability.TryEndAbility();
                }
            };
            
            // Link Tags.
            serverRepl.OnNotifyClientsAbilityTagsAdded += (tags) =>
            {
                ClientSystem.TagManager.AddAbilityTags(tags);
            };
            serverRepl.OnNotifyClientsAbilityTagsRemoved += (tags) =>
            {
                ClientSystem.TagManager.RemoveAbilityTags(tags);
            };

            // Client to Server Requests
            clientRepl.OnServerAbilityActivationRequested += (name, key, data) => 
                serverRepl.ProcessServerAbilityActivation(name, key, data);

            clientRepl.OnServerAbilityTerminationRequested += (name) => 
                serverRepl.ProcessServerAbilityTermination(name);
        }

        public void SyncAbilities()
        {
            foreach (var ability in ServerSystem.AbilityManager.Abilities.Values)
            {
                if (ClientSystem.AbilityManager.Abilities.TryGetValue(ability.Definition.UniqueName, out var clientAbility))
                {
                    clientAbility.SetLevel(ability.Level);
                    if (ability is ChargesAbility serverCharges && clientAbility is ChargesAbility clientCharges)
                    {
                        clientCharges.SetCharges(serverCharges.GetCurrentCharges());
                    }
                }
            }
        }

        private void SyncDataToEffect(Effect effect, EffectSyncData data)
        {
            effect.ActivationTime = data.ActivationTime;
            effect.PredictionKey = data.PredictionKey;
        }

        protected class MockNetworkRole : INetworkRole
        {
            public bool IsServer { get; }
            public bool IsClient => !IsServer;
            public bool IsHost => IsServer && IsLocalPlayer;
            public bool IsOwner => IsLocalPlayer;
            public bool IsLocalPlayer { get; }
            public bool HasAuthority { get; }
            public double ManualTime = -1;
            public double Time => ManualTime >= 0 ? ManualTime : UnityEngine.Time.timeAsDouble;
            public ulong NetworkObjectId { get; }

            public MockNetworkRole(bool isServer, bool isLocalPlayer, ulong networkObjectId)
            {
                IsServer = isServer;
                IsLocalPlayer = isLocalPlayer;
                NetworkObjectId = networkObjectId;
                HasAuthority = isServer || isLocalPlayer;
                if (ManualTime < 0) ManualTime = UnityEngine.Time.timeAsDouble;
            }

            public GameObject GetGameObjectFromNetworkId(ulong networkId) => null;
        }

        protected class MockDataManager : IDataManager
        {
            public Dictionary<string, AbilityDefinition> Abilities = new Dictionary<string, AbilityDefinition>();
            public Dictionary<string, EffectDefinition> Effects = new Dictionary<string, EffectDefinition>();

            public AbilityDefinition GetAbilityByName(string name) => Abilities.TryGetValue(name, out var def) ? def : null;
            public EffectDefinition GetEffectByName(string name) => Effects.TryGetValue(name, out var def) ? def : null;
            public CueDefinition GetCueByTag(GameplayTags.Runtime.Tag tag) => null;
            public CueDefinition GetCueByTag(string tag) => null;
        }
    }
}