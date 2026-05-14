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
    public abstract class AbilitySystemInteractionSyncTestBase : INetworkSystemRegistry
    {
        protected AbilitySystemManager SourceServer;
        protected AbilitySystemManager SourceClient;
        protected AbilitySystemManager TargetServer;
        protected AbilitySystemManager TargetClient;
        
        protected InteractionMockDataManager DataManager;
        protected Dictionary<ulong, AbilitySystemManager> NetworkRegistry = new();

        [SetUp]
        public virtual void Setup()
        {
            DataManager = new InteractionMockDataManager();
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

        public void TickAll()
        {
            SourceServer.Tick();
            SourceClient.Tick();
            TargetServer.Tick();
            TargetClient.Tick();
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

            serverRepl.OnNotifyClientAbilityRemoved += (def) => 
                client.AbilityManager.RemoveAbility(def.UniqueName);

            serverRepl.OnClientActivateAbility += (name, data) => 
                client.AbilityManager.ForceActivateAbility(name, data);

            serverRepl.OnClientEndAbility += (name) => 
                client.AbilityManager.ForceEndAbility(name);

            serverRepl.OnNotifyClientsAbilityChargesChanged += (name, curr, max) =>
                client.ReplicationManager.ProcessClientAbilityChargesChanged(name, curr, max);

            // Link Cues
            serverRepl.OnNotifyClientsPlayCue += (tag, act, data) =>
                client.ReplicationManager.ReceivedPlayCue(tag, act, data);

            // Link Tags
            serverRepl.OnNotifyClientsAbilityTagsAdded += (tags) =>
                client.TagManager.AddAbilityTags(tags);

            serverRepl.OnNotifyClientsAbilityTagsRemoved += (tags) =>
                client.TagManager.RemoveAbilityTags(tags);

            serverRepl.OnNotifyClientsTagAdded += (tag) =>
                client.TagManager.AddTag(tag);

            serverRepl.OnNotifyClientsTagRemoved += (tag) =>
                client.TagManager.RemoveTag(tag);

            // Client to Server Requests
            clientRepl.OnServerAbilityActivationRequested += (name, key, data) => 
                serverRepl.ProcessServerAbilityActivation(name, key, data);

            clientRepl.OnServerAbilityUnpredictedActivationRequested += (name, data) =>
                serverRepl.ProcessServerAbilityUnpredictedActivation(name, data);

            clientRepl.OnServerAbilityTerminationRequested += (name) => 
                serverRepl.ProcessServerAbilityTermination(name);
            
            clientRepl.OnAbilityActivationResponded += (key, success) =>
            {
                if (success) clientRepl.ProcessAbilityActivationConfirmed(key);
                else clientRepl.ProcessAbilityActivationDenied("", key); // Name is not strictly used in current implementation
            };
        }

        public IAbilitySystem GetSystemFromNetworkId(ulong networkId)
        {
            return NetworkRegistry.TryGetValue(networkId, out var system) ? system : null;
        }
    }
}
