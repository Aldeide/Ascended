using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public abstract class AbilitySystemInteractionSyncTestBase
    {
        protected AbilitySystemManager SourceServer;
        protected AbilitySystemManager SourceClient;
        protected AbilitySystemManager TargetServer;
        protected AbilitySystemManager TargetClient;
        
        protected MockDataManager DataManager;
        protected Dictionary<ulong, AbilitySystemManager> NetworkRegistry = new();

        [SetUp]
        public virtual void Setup()
        {
            DataManager = new MockDataManager();
            NetworkRegistry.Clear();

            // Setup Source
            SourceServer = CreateSystem(true, true, 100);
            SourceClient = CreateSystem(false, true, 100);
            
            // Setup Target
            TargetServer = CreateSystem(true, false, 200);
            TargetClient = CreateSystem(false, false, 200);

            LinkPair(SourceServer, SourceClient);
            LinkPair(TargetServer, TargetClient);
        }

        private AbilitySystemManager CreateSystem(bool isServer, bool isLocalPlayer, ulong networkId)
        {
            var manager = new AbilitySystemManager(DataManager);
            manager.NetworkRole = new InteractionMockNetworkRole(isServer, isLocalPlayer, networkId, this);
            manager.ReplicationManager = new MockReplicationManager(manager) { DataManager = DataManager };
            
            if (isServer)
            {
                NetworkRegistry[networkId] = manager;
            }
            
            return manager;
        }

        protected void LinkPair(AbilitySystemManager server, AbilitySystemManager client)
        {
            var serverRepl = server.ReplicationManager;
            var clientRepl = client.ReplicationManager;

            // Link Attributes
            serverRepl.OnNotifyClientsAttributeBaseValueChanged += (name, val) => 
                client.AttributeSetManager.GetAttribute(name)?.SetBaseValue(val);
            
            serverRepl.OnNotifyClientsAttributeCurrentValueChanged += (name, oldVal, newVal) => 
                client.AttributeSetManager.GetAttribute(name)?.SetCurrentValue(newVal);

            // Link Effects
            serverRepl.OnNotifyClientsEffectAdded += (data) => 
                client.ReplicationManager.ProcessClientEffectAdded(data);

            serverRepl.OnNotifyClientsEffectRemoved += (name) => 
                client.ReplicationManager.ProcessClientEffectRemoved(name);

            // Link Abilities
            serverRepl.OnNotifyClientAbilityGranted += (def) => 
                client.AbilityManager.GrantAbility(def);

            serverRepl.OnClientActivateAbility += (name, data) => 
                client.AbilityManager.ForceActivateAbility(name, data);

            serverRepl.OnClientEndAbility += (name) => 
                client.AbilityManager.ForceEndAbility(name);

            // Client to Server Requests
            clientRepl.OnServerAbilityActivationRequested += (name, key, data) => 
                serverRepl.ProcessServerAbilityActivation(name, key, data);

            clientRepl.OnServerAbilityTerminationRequested += (name) => 
                serverRepl.ProcessServerAbilityTermination(name);
        }

        protected class InteractionMockNetworkRole : INetworkRole
        {
            public bool IsServer { get; }
            public bool IsClient => !IsServer;
            public bool IsHost => IsServer && IsLocalPlayer;
            public bool IsOwner => IsLocalPlayer;
            public bool IsLocalPlayer { get; }
            public bool HasAuthority { get; }
            public double Time => UnityEngine.Time.timeAsDouble;
            public ulong NetworkObjectId { get; }
            
            private readonly AbilitySystemInteractionSyncTestBase _owner;

            public InteractionMockNetworkRole(bool isServer, bool isLocalPlayer, ulong networkObjectId, AbilitySystemInteractionSyncTestBase owner)
            {
                IsServer = isServer;
                IsLocalPlayer = isLocalPlayer;
                NetworkObjectId = networkObjectId;
                HasAuthority = isServer || isLocalPlayer;
                _owner = owner;
            }

            public GameObject GetGameObjectFromNetworkId(ulong networkId)
            {
                // In tests, we don't usually have GameObjects, but we can return null 
                // or a dummy if needed. The important part is being able to find the AbilitySystem.
                return null;
            }
            
            // Helper for testing to resolve the system directly
            public IAbilitySystem GetSystemFromNetworkId(ulong networkId)
            {
                return _owner.NetworkRegistry.TryGetValue(networkId, out var system) ? system : null;
            }
        }

        protected class MockDataManager : IDataManager
        {
            public Dictionary<string, AbilityDefinition> Abilities = new();
            public Dictionary<string, EffectDefinition> Effects = new();

            public AbilityDefinition GetAbilityByName(string name) => Abilities.TryGetValue(name, out var def) ? def : null;
            public EffectDefinition GetEffectByName(string name) => Effects.TryGetValue(name, out var def) ? def : null;
            public CueDefinition GetCueByTag(GameplayTags.Runtime.Tag tag) => null;
            public CueDefinition GetCueByTag(string tag) => null;
        }
    }
}
