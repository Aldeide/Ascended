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
    public class AbilitySystemComponent : NetworkBehaviour
    {
        [FormerlySerializedAs("definition")] public AbilitySystemDefinition Definition;
        public IAbilitySystem AbilitySystem { get; private set; }
        public Action OnAbilitySystemInitialised;
        public bool IsInitialized => AbilitySystem != null;
        private CueManagerComponent _cueManagerComponent;
        
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
                    
                    var sourceAsc = effect.Source as AbilitySystemManager;
                    if (sourceAsc != null && sourceAsc.Component != null)
                        data.SourceId = sourceAsc.Component.NetworkObjectId;
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
            
            AbilitySystem = new AbilitySystemManager(this);
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
            if (!AbilitySystem.AbilityManager.ServerTryActivateAbilityWithKey(abilityName, key, data))
            {
                NotifyAbilityActivationFailedRpc(abilityName, key);
            }
        }
        
        [Rpc(SendTo.Server)]
        public void ServerTryEndAbilityRpc(string abilityName)
        {
            AbilitySystem.AbilityManager.EndAbility(abilityName);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyAbilityActivationFailedRpc(string abilityName, PredictionKey key)
        {
            AbilitySystem.AbilityManager.EndAbility(key);
            AbilitySystem.EffectManager.RetractPredictedEffect(key);
        }

        public void EndAbility(string abilityName)
        {
            AbilitySystem.AbilityManager.EndAbility(abilityName);
        }

        public void ApplyEffect(EffectDefinition effectDefinition)
        {
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
                var sourceAsc = effect.Source as AbilitySystemManager;
                ulong sourceId = sourceAsc != null && sourceAsc.Component != null ? sourceAsc.Component.NetworkObjectId : NetworkObjectId;

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
            AbilitySystem.EffectManager.ReconcilePredictedEffect(key);
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
    }
}