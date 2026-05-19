using System;
using System.Linq;
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
        public IAbilitySystem AbilitySystem { get; internal set; }
        public Action OnAbilitySystemInitialised;
        public bool IsInitialized => AbilitySystem != null;
        private CueManagerComponent _cueManagerComponent;

        public string ServerDebugString { get; private set; }
        private float _lastDebugRequestTime;

        public void RequestUpdateFromServer()
        {
            if (IsServer)
            {
                ServerDebugString = CalculateFullDebugInfo();
                return;
            }

            if (UnityEngine.Time.time - _lastDebugRequestTime < 0.5f) return;
            _lastDebugRequestTime = UnityEngine.Time.time;
            RequestDebugDataServerRpc();
        }

        [Rpc(SendTo.Server)]
        public void RequestDebugDataServerRpc(RpcParams rpcParams = default)
        {
            var debugInfo = CalculateFullDebugInfo();
            NotifyDebugDataClientRpc(debugInfo, rpcParams.Receive.SenderClientId);
        }

        [Rpc(SendTo.Everyone)]
        public void NotifyDebugDataClientRpc(string debugInfo, ulong targetId)
        {
            if (NetworkManager.LocalClientId != targetId) return;
            ServerDebugString = debugInfo;
        }

        public string CalculateFullDebugInfo()
        {
            if (AbilitySystem == null) return "No Ability System";
            var output = "--- Attributes ---\n" + AbilitySystem.AttributeSetManager.DebugString() + "\n\n";
            output += "--- Effects ---\n" + AbilitySystem.EffectManager.DebugString() + "\n\n";
            output += "--- Abilities ---\n" + AbilitySystem.AbilityManager.DebugString() + "\n\n";
            output += "--- Tags ---\n" + AbilitySystem.TagManager.DebugString() + "\n";
            return output;
        }
        
        public double Time => NetworkManager != null ? NetworkManager.ServerTime.Time : UnityEngine.Time.time;

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
                var i = 0;
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
            if (activeEffects.Count <= 0) return;
            var effectSyncData = new EffectSyncData[activeEffects.Count];
            for (var k = 0; k < activeEffects.Count; k++)
            {
                var effect = activeEffects[k];
                var data = new EffectSyncData
                {
                    EffectName = effect.Definition.name,
                    ActivationTime = effect.ActivationTime,
                    PredictionKey = effect.PredictionKey,
                    Level = effect.Level,
                    NumStacks = effect.NumStacks
                };

                if (effect.SetByCallerTagMagnitudes.Count > 0)
                {
                    data.SetByCallerTags = effect.SetByCallerTagMagnitudes.Keys.ToArray();
                    data.SetByCallerValues = effect.SetByCallerTagMagnitudes.Values.ToArray();
                }
                    
                if (effect.Source != null && effect.Source.NetworkRole != null)
                    data.SourceId = effect.Source.NetworkRole.NetworkObjectId;
                else
                    data.SourceId = NetworkObjectId;
                        
                effectSyncData[k] = data;
            }
            SyncEffectsClientRpc(effectSyncData, clientRpcParams);
        }

        public void Initialise()
        {
            if (IsInitialized) return;
            _cueManagerComponent = GetComponent<CueManagerComponent>();
            
            var abilitySystemManager = new AbilitySystemManager(DataLibrary.Instance);
            abilitySystemManager.NetworkRole = this;
            AbilitySystem = abilitySystemManager;

            var repl = AbilitySystem.ReplicationManager;

            repl.OnNotifyClientsAttributeBaseValueChanged += (attr, val) => NotifyClientsBaseValueChangedRpc(attr, val);
            repl.OnNotifyClientsAttributeCurrentValueChanged += (attr, old, val) => NotifyClientsCurrentValueChangedRpc(attr, old, val);
            repl.OnNotifyClientsPlayCue += (tag, act, data) => NotifyClientsPlayCueRpc(tag, act, data);
            repl.OnNotifyClientAbilityGranted += (def) => NotifyClientAbilityGrantedRpc(def.UniqueName);
            repl.OnNotifyClientAbilityRemoved += (def) => NotifyClientAbilityRemovedRpc(def.UniqueName);

            repl.OnNotifyClientsAbilityChargesChanged +=
                (name, current, max) => NotifyClientsAbilityChargesChangedRpc(name, current, max);
            repl.OnNotifyClientsAbilityTagsAdded += (tags) => NotifyClientsAbilityTagsAddedRpc(tags);
            repl.OnNotifyClientsAbilityTagsRemoved += (tags) => NotifyClientsAbilityTagsRemovedRpc(tags);
            repl.OnNotifyClientsEffectAdded += (data) => NotifyOwnerEffectAddedRpc(data);
            repl.OnNotifyClientsEffectRemoved += (name) => NotifyOwnerEffectRemovedRpc(name);
            
            // New Ability Networking bindings
            repl.OnServerAbilityActivationRequested += (name, key, data) => ServerTryActivateAbilityRpc(name, key, data);
            repl.OnServerAbilityUnpredictedActivationRequested += (name, data) => ServerTryActivateUnpredictedAbilityRpc(name, data);
            repl.OnServerAbilityTerminationRequested += (name) => ServerTryEndAbilityRpc(name);
            repl.OnServerAbilityBatchRequested += (batch) => ServerAbilityBatchRpc(batch);
            repl.OnAbilityActivationResponded += (key, success) =>
            {
                if (success) NotifyAbilityActivationSucceededRpc(key);
                else NotifyAbilityActivationFailedRpc("", key);
            };
            repl.OnClientActivateAbility += (name, data) => NotifyOwnerActivateAbilityRpc(name, data);
            repl.OnClientEndAbility += (name) => NotifyOwnerEndAbilityRpc(name);
            repl.OnServerSyncKeyReceived += (name, key) => ServerSendSyncKeyRpc(name, key);
            repl.OnClientSyncKeyConfirmed += (name, key) => ClientConfirmSyncKeyRpc(name, key);
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
            
            OnAbilitySystemInitialised?.Invoke();
        }

        public void Update()
        {
            AbilitySystem.Tick();
            AbilitySystem.AttributeSetManager.UpdateAttributesJobified();
        }
        
        public void OnAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (IsServer)
            {
                NotifyClientsBaseValueChangedRpc(attribute.GetName(), newValue);
            }
        }
        
        public void OnAttributeBaseCurrentChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (IsServer)
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
        public void ServerTryActivateAbilityRpc(string abilityName, PredictionKey key, AbilityData data, RpcParams rpcParams = default)
        {
            AbilitySystem.ReplicationManager.ProcessServerAbilityActivation(abilityName, key, data);
        }
        
        [Rpc(SendTo.Server)]
        public void ServerTryActivateUnpredictedAbilityRpc(string abilityName, AbilityData data, RpcParams rpcParams = default)
        {
            AbilitySystem.ReplicationManager.ProcessServerAbilityUnpredictedActivation(abilityName, data);
        }
        
        [Rpc(SendTo.Server)]
        public void ServerTryEndAbilityRpc(string abilityName, RpcParams rpcParams = default)
        {
            AbilitySystem.ReplicationManager.ProcessServerAbilityTermination(abilityName);
        }

        [Rpc(SendTo.Server)]
        public void ServerAbilityBatchRpc(AbilityBatchData batch, RpcParams rpcParams = default)
        {
            AbilitySystem.ReplicationManager.ProcessServerAbilityBatch(batch);
        }

        [Rpc(SendTo.Server)]
        public void ServerSendSyncKeyRpc(string abilityName, PredictionKey key, RpcParams rpcParams = default)
        {
            AbilitySystem.ReplicationManager.ProcessServerSyncKey(abilityName, key);
        }

        [Rpc(SendTo.Owner)]
        public void ClientConfirmSyncKeyRpc(string abilityName, PredictionKey key)
        {
            AbilitySystem.ReplicationManager.ProcessClientSyncKeyConfirmed(abilityName, key);
        }

        public void AbilityLocalInputPressed(string abilityName)
        {
            AbilitySystem.AbilityManager.AbilityLocalInputPressed(abilityName);
        }

        public void AbilityLocalInputReleased(string abilityName)
        {
            AbilitySystem.AbilityManager.AbilityLocalInputReleased(abilityName);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerActivateAbilityRpc(string abilityName, AbilityData data)
        {
            NotifyOwnerActivateAbilityInternal(abilityName, data);
        }

        public void NotifyOwnerActivateAbilityInternal(string abilityName, AbilityData data)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            AbilitySystem.ReplicationManager.ProcessClientActivateAbility(abilityName, data);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEndAbilityRpc(string abilityName)
        {
            NotifyOwnerEndAbilityInternal(abilityName);
        }

        public void NotifyOwnerEndAbilityInternal(string abilityName)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            AbilitySystem.ReplicationManager.ProcessClientEndAbility(abilityName);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyAbilityActivationSucceededRpc(PredictionKey key)
        {
            NotifyAbilityActivationSucceededInternal(key);
        }

        public void NotifyAbilityActivationSucceededInternal(PredictionKey key)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            AbilitySystem.ReplicationManager.ProcessAbilityActivationConfirmed(key);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyAbilityActivationFailedRpc(string abilityName, PredictionKey key)
        {
            NotifyAbilityActivationFailedInternal(abilityName, key);
        }

        public void NotifyAbilityActivationFailedInternal(string abilityName, PredictionKey key)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            AbilitySystem.ReplicationManager.ProcessAbilityActivationDenied(abilityName, key);
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
            // Instant effects aren't predicted and are only run on the server.
            if (!IsServer) return;
            var effect = effectDefinition.ToEffect(source, AbilitySystem);
            effect.Execute();
        }

        // These are now handled via ReplicationManager bridge in Initialise()

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectAddedRpc(EffectSyncData data)
        {
            NotifyOwnerEffectAddedInternal(data);
        }

        public void NotifyOwnerEffectAddedInternal(EffectSyncData data)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            var effectDefinition = DataLibrary.Instance.GetEffectByName(data.EffectName);
            if (effectDefinition == null) return;
            
            IAbilitySystem source = FindSource(data.SourceId);
            var effect = effectDefinition.ToEffect(source, AbilitySystem);
            ApplySyncDataToEffect(effect, data);

            if (data.PredictionKey.IsValidKey())
            {
                AbilitySystem.AbilityManager.NotifyServerResponse(data.PredictionKey, true);
                AbilitySystem.EffectManager.ReconcilePredictedEffect(data.PredictionKey, effect);
            }
            else
            {
                AbilitySystem.EffectManager.AddEffectFromServer(effect);
            }
        }

        private void ApplySyncDataToEffect(Effect effect, EffectSyncData data)
        {
            effect.ActivationTime = data.ActivationTime;
            effect.Level = data.Level;
            effect.NumStacks = data.NumStacks;

            if (data.SetByCallerTags != null && data.SetByCallerValues != null)
            {
                for (int i = 0; i < data.SetByCallerTags.Length; i++)
                {
                    if (i < data.SetByCallerValues.Length)
                    {
                        effect.SetSetByCallerMagnitude(data.SetByCallerTags[i], data.SetByCallerValues[i]);
                    }
                }
            }
        }

        private IAbilitySystem FindSource(ulong sourceId)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(sourceId, out var networkObj))
            {
                if (networkObj.TryGetComponent<AbilitySystemComponent>(out var asc))
                {
                    return asc.AbilitySystem;
                }
            }
            return AbilitySystem;
        }
        
        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectRemovedRpc(string effectName)
        {
            NotifyOwnerEffectRemovedInternal(effectName);
        }

        public void NotifyOwnerEffectRemovedInternal(string effectName)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            AbilitySystem.EffectManager.RemoveEffect(effectName);
        }

        [Rpc(SendTo.Everyone)]
        public void ObserversPlayCueRpc(string cueTag, CueData data, bool isPredicted = false)
        {
            // If it's a predicted cue and I am the owner, I already played it locally in OnPlayCueRequested.
            if (isPredicted && IsOwner) return;
            
            var gameplayTag = new Tag(cueTag);
            AbilitySystem.CueManager.OnCueReceived(gameplayTag, CueAction.Execute, data);
            _cueManagerComponent.PlayCue(cueTag);
        }
        
        [Rpc(SendTo.Everyone)]
        public void ObserversPlayCueWithDataRpc(string cueTag, CueData data, bool isPredicted = false)
        {
            // If it's a predicted cue and I am the owner, I already played it locally in OnPlayCueRequested.
            if (isPredicted && IsOwner) return;
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
            AddCuesClientInternal(cueTag, cueData);
        }

        public void AddCuesClientInternal(Tag cueTag, CueData cueData = default)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            var cueDefinition = DataLibrary.Instance.GetCueByTag(cueTag);
            AbilitySystem.CueManager.AddCue(cueDefinition, cueData);
        }

        [ClientRpc]
        public void AddCuesBatchClientRpc(Tag[] cueTags, CueData[] cueDatas, ClientRpcParams clientRpcParams = default)
        {
            AddCuesBatchClientInternal(cueTags, cueDatas);
        }

        public void AddCuesBatchClientInternal(Tag[] cueTags, CueData[] cueDatas)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            for (var i = 0; i < cueTags.Length; i++)
            {
                var cueDefinition = DataLibrary.Instance.GetCueByTag(cueTags[i]);
                AbilitySystem.CueManager.AddCue(cueDefinition, cueDatas[i]);
            }
        }

        [ClientRpc]
        public void SyncAttributesClientRpc(AttributeSyncData[] syncData, ClientRpcParams clientRpcParams = default)
        {
            SyncAttributesClientInternal(syncData);
        }

        public void SyncAttributesClientInternal(AttributeSyncData[] syncData)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            foreach (var data in syncData)
            {
                var attribute = AbilitySystem.AttributeSetManager.GetAttribute(data.AttributeName);
                if (attribute == null) continue;
                attribute.SetBaseValue(data.BaseValue);
                attribute.SetCurrentValue(data.CurrentValue);
            }
        }

        [ClientRpc]
        public void SyncEffectsClientRpc(EffectSyncData[] syncData, ClientRpcParams clientRpcParams = default)
        {
            SyncEffectsClientInternal(syncData);
        }

        public void SyncEffectsClientInternal(EffectSyncData[] syncData)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            foreach (var data in syncData)
            {
                var effectDefinition = DataLibrary.Instance.GetEffectByName(data.EffectName);
                if (effectDefinition == null) continue;
                
                IAbilitySystem source = FindSource(data.SourceId);
                var effect = effectDefinition.ToEffect(source, AbilitySystem);
                ApplySyncDataToEffect(effect, data);
                AbilitySystem.EffectManager.AddEffectFromServer(effect);
            }
        }

        [Rpc(SendTo.NotServer)]
        public void NotifyClientAbilityGrantedRpc(string abilityName)
        {
            NotifyClientAbilityGrantedInternal(abilityName);
        }

        public void NotifyClientAbilityGrantedInternal(string abilityName)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            var abilityDefinition = DataLibrary.Instance.GetAbilityByName(abilityName);
            AbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
        }
        
        [Rpc(SendTo.NotServer)]
        public void NotifyClientAbilityRemovedRpc(string abilityName)
        {
            NotifyClientAbilityRemovedInternal(abilityName);
        }

        public void NotifyClientAbilityRemovedInternal(string abilityName)
        {
            if (AbilitySystem == null || AbilitySystem.IsServer()) return;
            AbilitySystem.AbilityManager.RemoveAbility(abilityName);
        }

        [Rpc(SendTo.NotServer)]
        public void NotifyClientsAbilityChargesChangedRpc(string abilityName, int current, int max)
        {
            AbilitySystem.ReplicationManager.ProcessClientAbilityChargesChanged(abilityName, current, max);
        }
        
        [Rpc(SendTo.NotServer)]
        public void NotifyClientsAbilityTagsAddedRpc(AbilityTagSyncData tags)
        {
            AbilitySystem.TagManager.AddAbilityTags(tags);
        }
        
        [Rpc(SendTo.NotServer)]
        public void NotifyClientsAbilityTagsRemovedRpc(AbilityTagSyncData tags)
        {
            AbilitySystem.TagManager.RemoveAbilityTags(tags);
        }

        public GameObject GetGameObjectFromNetworkId(ulong networkId)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkId, out var networkObj))
            {
                return networkObj.gameObject;
            }
            return null;
        }
    }
}