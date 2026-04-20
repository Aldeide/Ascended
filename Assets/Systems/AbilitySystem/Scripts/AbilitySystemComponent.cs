using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Utilities;
using GameplayTags.Runtime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Scripts
{
    public class AbilitySystemComponent : NetworkBehaviour, INetworkRole
    {
        [FormerlySerializedAs("definition")] public AbilitySystemDefinition Definition;
        public IAbilitySystem AbilitySystem { get; private set; }
        public Action OnAbilitySystemInitialised;
        public bool IsInitialized => AbilitySystem != null;
        private CueManagerComponent _cueManagerComponent;
        
        public double Time => NetworkManager != null ? NetworkManager.ServerTime.Time : UnityEngine.Time.time;
        
        public struct AttributeSyncData : INetworkSerializable
        {
            public string AttributeName;
            public float BaseValue;
            public float CurrentValue;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref AttributeName);
                serializer.SerializeValue(ref BaseValue);
                serializer.SerializeValue(ref CurrentValue);
            }
        }

        public struct EffectSyncData : INetworkSerializable
        {
            public string EffectName;
            public float ActivationTime;
            public ulong SourceId;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref EffectName);
                serializer.SerializeValue(ref ActivationTime);
                serializer.SerializeValue(ref SourceId);
            }
        }

        public struct AbilityTagSyncData : INetworkSerializable
        {
            public string AbilityUniqueName;
            public Tag[] Tags;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref AbilityUniqueName);
                serializer.SerializeValue(ref Tags);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialise();
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnected;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            // When a new client connects, we need to catch them up with the current state.
            // Prepare an RPC parameter to target only the newly connected client.
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };

            // Sync Cues
            var activeCues = AbilitySystem.CueManager.GetActiveCues();
            if (activeCues.Count > 0)
            {
                var cueTags = new Tag[activeCues.Count];
                var cueDatas = new CueData[activeCues.Count];
                int i = 0;
                foreach (var cue in activeCues)
                {
                    cueTags[i] = cue.Key;
                    cueDatas[i] = cue.Value;
                    i++;
                }
                AddCuesBatchClientRpc(cueTags, cueDatas, clientRpcParams);
            }

            // Sync Attributes
            var snapshot = AbilitySystem.AttributeSetManager.Snapshot();
            if (snapshot.Count > 0)
            {
                var attributeSyncData = new AttributeSyncData[snapshot.Count];
                int j = 0;
                foreach (var kvp in snapshot)
                {
                    attributeSyncData[j] = new AttributeSyncData
                    {
                        AttributeName = kvp.Key,
                        BaseValue = kvp.Value.BaseValue,
                        CurrentValue = kvp.Value.CurrentValue
                    };
                    j++;
                }
                SyncAttributesClientRpc(attributeSyncData, clientRpcParams);
            }

            // Sync Effects
            var activeEffects = AbilitySystem.EffectManager.GetActiveEffects();
            if (activeEffects.Count > 0)
            {
                var effectSyncData = new EffectSyncData[activeEffects.Count];
                for (int k = 0; k < activeEffects.Count; k++)
                {
                    var effect = activeEffects[k];
                    var data = new EffectSyncData
                    {
                        EffectName = effect.Definition.name,
                        ActivationTime = effect.ActivationTime
                    };
                    
                    if (effect.Source != null && effect.Source.NetworkRole != null)
                        data.SourceId = effect.Source.NetworkRole.NetworkObjectId;
                    else
                        data.SourceId = NetworkObjectId;
                        
                    effectSyncData[k] = data;
                }
                SyncEffectsClientRpc(effectSyncData, clientRpcParams);
            }
        }

        public void Initialise()
        {
            _cueManagerComponent = GetComponent<CueManagerComponent>();
            
            var abilitySystemManager = new AbilitySystemManager();
            abilitySystemManager.NetworkRole = this;
            AbilitySystem = abilitySystemManager;

            abilitySystemManager.ReplicationManager.OnNotifyClientsAttributeBaseValueChanged += (attr, val) => NotifyClientsBaseValueChangedRpc(attr, val);
            abilitySystemManager.ReplicationManager.OnNotifyClientsAttributeCurrentValueChanged += (attr, old, val) => NotifyClientsCurrentValueChangedRpc(attr, old, val);
            abilitySystemManager.ReplicationManager.OnNotifyClientsPlayCue += (tag, act, data) => NotifyClientsPlayCueRpc(tag, act, data);
            abilitySystemManager.ReplicationManager.OnNotifyClientAbilityGranted += (def) => NotifyClientAbilityGrantedRpc(def.UniqueName);
            abilitySystemManager.ReplicationManager.OnNotifyClientAbilityRemoved += (def) => NotifyClientAbilityRemovedRpc(def.UniqueName);
            
            abilitySystemManager.ReplicationManager.OnNotifyClientsAbilityTagsAdded += (tags) => NotifyClientsAbilityTagsAddedRpc(tags);
            
            abilitySystemManager.AbilityManager.OnServerTryActivateAbilityRequested += (name, key, data) => ServerTryActivateAbilityRpc(name, key, data);
            abilitySystemManager.AbilityManager.OnServerTryEndAbilityRequested += (name) => ServerTryEndAbilityRpc(name);
            abilitySystemManager.OnPlayCueRequested += (tag, data, pred) =>
            {
                // If it's predicted and we are the owner, play it locally immediately.
                if (pred && IsOwner)
                {
                    AbilitySystem.CueManager.OnCueReceived(new Tag(tag), CueAction.Execute, data);
                    _cueManagerComponent.PlayCue(tag, data);
                }
                
                // Always send to everyone (including self, to be filtered by pred check in RPC body).
                ObserversPlayCueWithDataRpc(tag, data, pred);
            };
            foreach (var attributeSet in Definition.AttributeSets)
            {
                var type = ReflectionUtil.GetAttributeSetType(attributeSet);
                var set = Activator.CreateInstance(type, AbilitySystem) as AttributeSet;
                AbilitySystem.AttributeSetManager.AddAttributeSet(type, set);
            }

            foreach (var ability in Definition.BaseAbilities)
            {
                AbilitySystem.AbilityManager.GrantAbility(ability);
            }

            AbilitySystem.AttributeSetManager.OnAnyAttributeBaseValueChanged += OnAttributeBaseValueChanged;
            AbilitySystem.AttributeSetManager.OnAnyAttributeCurrentValueChanged += OnAttributeBaseCurrentChanged;
            AbilitySystem.EffectManager.OnEffectAdded += OnEffectAdded;
            AbilitySystem.EffectManager.OnEffectRemoved += OnEffectRemoved;
            
            OnAbilitySystemInitialised?.Invoke();
        }

        public void Update()
        {
            AbilitySystem.Tick();
        }
        
        public void OnAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (IsServer && ! IsHost)
            {
                NotifyClientsBaseValueChangedRpc(attribute.GetName(), newValue);
            }
        }
        
        public void OnAttributeBaseCurrentChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (IsServer && ! IsHost)
            {
                NotifyClientsCurrentValueChangedRpc(attribute.GetName(), oldValue, newValue);
            }
        }

        [Rpc(SendTo.NotServer)]
        public void NotifyClientsBaseValueChangedRpc(string attributeName, float newValue)
        {
            AbilitySystem.ReplicationManager.OnAttributeBaseValueChanged(attributeName, newValue);
        }
        
        [Rpc(SendTo.NotServer)]
        public void NotifyClientsCurrentValueChangedRpc(string attributeName, float oldValue, float newValue)
        {
            AbilitySystem.AttributeSetManager.GetAttribute(attributeName)?.SetCurrentValue(newValue);
        }

        public void TryActivateAbility(string abilityName, AbilityData data = new())
        {
            AbilitySystem.AbilityManager.TryActivateAbility(abilityName, data);
        }

        [Rpc(SendTo.Server)]
        public void ServerTryActivateAbilityRpc(string abilityName, PredictionKey key, AbilityData data)
        {
            if (AbilitySystem.AbilityManager.ServerTryActivateAbilityWithKey(abilityName, key, data))
            {
                NotifyAbilityActivationSucceededRpc(key);
            }
            else
            {
                Debug.Log("Predicted ability failed: " + abilityName);
                NotifyAbilityActivationFailedRpc(abilityName, key);
            }
        }
        
        [Rpc(SendTo.Server)]
        public void ServerTryEndAbilityRpc(string abilityName)
        {
            AbilitySystem.AbilityManager.EndAbility(abilityName);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyAbilityActivationSucceededRpc(PredictionKey key)
        {
            AbilitySystem.AbilityManager.NotifyServerResponse(key, true);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyAbilityActivationFailedRpc(string abilityName, PredictionKey key)
        {
            AbilitySystem.AbilityManager.NotifyServerResponse(key, false);
        }

        public void EndAbility(string abilityName)
        {
            AbilitySystem.AbilityManager.EndAbility(abilityName);
        }

        public void ApplyEffect(EffectDefinition effectDefinition)
        {
            if (effectDefinition.IsInstant())
            {
                ExecuteEffect(effectDefinition, AbilitySystem);
                return;
            }
            var effect = effectDefinition.ToEffect(AbilitySystem, AbilitySystem);
            effect.Activate();
            AbilitySystem.EffectManager.AddEffect(effect);
        }

        public void ExecuteEffect(EffectDefinition effectDefinition, IAbilitySystem source)
        {
            var effect = effectDefinition.ToEffect(source, AbilitySystem);
            effect.Execute();
        }

        public void OnEffectAdded(Effect effect)
        {
            if (IsServer && !IsHost)
            {
                ulong sourceId = effect.Source != null && effect.Source.NetworkRole != null ? effect.Source.NetworkRole.NetworkObjectId : NetworkObjectId;

                if (effect.PredictionKey.IsValidKey())
                {
                    NotifyOwnerEffectAddedRpc(effect.PredictionKey, effect.Definition.name, effect.ActivationTime, sourceId);
                    return;
                }
                NotifyOwnerEffectAddedRpc(effect.Definition.name, effect.ActivationTime, sourceId);
            }
        }
        
        public void OnEffectRemoved(Effect effect)
        {
            if (IsServer && !IsHost)
            {
                NotifyOwnerEffectRemovedRpc(effect.Definition.name);
            }
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectAddedRpc(string effectName, float applicationTime, ulong sourceId)
        {
            if (IsServer) return;
            var effectDefinition = DataLibrary.Instance.GetEffectByName(effectName);
            
            IAbilitySystem source = AbilitySystem;
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(sourceId, out var networkObj))
            {
                if (networkObj.TryGetComponent<AbilitySystemComponent>(out var asc))
                {
                    source = asc.AbilitySystem;
                }
            }

            var effect = effectDefinition.ToEffect(source, AbilitySystem);
            effect.ActivationTime = applicationTime;
            AbilitySystem.EffectManager.AddEffectFromServer(effect);
        }
        
        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectAddedRpc(PredictionKey key,string effectName, float applicationTime, ulong sourceId)
        {
            if (IsServer) return;
            var effectDefinition = DataLibrary.Instance.GetEffectByName(effectName);
            
            IAbilitySystem source = AbilitySystem;
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(sourceId, out var networkObj))
            {
                if (networkObj.TryGetComponent<AbilitySystemComponent>(out var asc))
                {
                    source = asc.AbilitySystem;
                }
            }
            
            var effect = effectDefinition.ToEffect(source, AbilitySystem);
            effect.ActivationTime = applicationTime;
            AbilitySystem.AbilityManager.NotifyServerResponse(key, true);
            AbilitySystem.EffectManager.ReconcilePredictedEffect(key, effect);
        }
        
        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectRemovedRpc(string effectName)
        {
            AbilitySystem.EffectManager.RemoveEffect(effectName);
        }

        [Rpc(SendTo.Everyone)]
        public void ObserversPlayCueRpc(string cueTag, CueData data, bool isPredicted = false)
        {
            if (isPredicted && IsOwner && !IsServer) return;
            var gameplayTag = new Tag(cueTag);
            AbilitySystem.CueManager.OnCueReceived(gameplayTag, CueAction.Execute, data);
            _cueManagerComponent.PlayCue(cueTag);
        }
        
        [Rpc(SendTo.Everyone)]
        public void ObserversPlayCueWithDataRpc(string cueTag, CueData data, bool isPredicted = false)
        {
            if (isPredicted && IsOwner && !IsServer) return;
            _cueManagerComponent.PlayCue(cueTag, data);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        public void NotifyClientsPlayCueRpc(Tag cueTag, CueAction cueAction, CueData cueData)
        {
            AbilitySystem.ReplicationManager.ReceivedPlayCue(cueTag, cueAction, cueData);
        }

        [ClientRpc]
        public void AddCuesClientRpc(Tag cueTag, CueData cueData = default, ClientRpcParams clientRpcParams = default)
        {
            var cueDefinition = DataLibrary.Instance.GetCueByTag(cueTag);
            AbilitySystem.CueManager.AddCue(cueDefinition, cueData);
        }

        [ClientRpc]
        public void AddCuesBatchClientRpc(Tag[] cueTags, CueData[] cueDatas, ClientRpcParams clientRpcParams = default)
        {
            for (int i = 0; i < cueTags.Length; i++)
            {
                var cueDefinition = DataLibrary.Instance.GetCueByTag(cueTags[i]);
                AbilitySystem.CueManager.AddCue(cueDefinition, cueDatas[i]);
            }
        }

        [ClientRpc]
        public void SyncAttributesClientRpc(AttributeSyncData[] syncData, ClientRpcParams clientRpcParams = default)
        {
            foreach (var data in syncData)
            {
                var attribute = AbilitySystem.AttributeSetManager.GetAttribute(data.AttributeName);
                if (attribute != null)
                {
                    attribute.SetBaseValue(data.BaseValue);
                    attribute.SetCurrentValue(data.CurrentValue);
                }
            }
        }

        [ClientRpc]
        public void SyncEffectsClientRpc(EffectSyncData[] syncData, ClientRpcParams clientRpcParams = default)
        {
            foreach (var data in syncData)
            {
                var effectDefinition = DataLibrary.Instance.GetEffectByName(data.EffectName);
                if (effectDefinition == null) continue;
                
                IAbilitySystem source = AbilitySystem;
                if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(data.SourceId, out var networkObj))
                {
                    if (networkObj.TryGetComponent<AbilitySystemComponent>(out var asc))
                    {
                        source = asc.AbilitySystem;
                    }
                }
                
                var effect = effectDefinition.ToEffect(source, AbilitySystem);
                effect.ActivationTime = data.ActivationTime;
                AbilitySystem.EffectManager.AddEffectFromServer(effect);
            }
        }

        [Rpc(SendTo.NotServer)]
        public void NotifyClientAbilityGrantedRpc(string abilityName)
        {
            var abilityDefinition = DataLibrary.Instance.GetAbilityByName(abilityName);
            AbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
        }
        
        [Rpc(SendTo.NotServer)]
        public void NotifyClientAbilityRemovedRpc(string abilityName)
        {
            AbilitySystem.AbilityManager.RemoveAbility(abilityName);
        }
        
        [Rpc(SendTo.NotServer)]
        public void NotifyClientsAbilityTagsAddedRpc(AbilityTagSyncData tags)
        {
            AbilitySystem.TagManager.AddAbilityTags(tags);
        }
    }
}