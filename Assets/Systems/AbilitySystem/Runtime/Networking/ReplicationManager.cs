using System;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using UnityEngine;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Runtime.Networking
{
    /// <summary>
    /// Manages the replication of attribute and gameplay cue updates in a networked environment.
    /// </summary>
    public class ReplicationManager : IReplicationManager
    {
        private readonly IAbilitySystem _owner;

        public Action<string, float> OnNotifyClientsAttributeBaseValueChanged { get; set; }
        public Action<string, float, float> OnNotifyClientsAttributeCurrentValueChanged { get; set; }
        public Action<Tag, CueAction, CueData> OnNotifyClientsPlayCue { get; set; }
        public Action<AbilityDefinition> OnNotifyClientAbilityGranted { get; set; }
        public Action<AbilityDefinition> OnNotifyClientAbilityRemoved { get; set; }
        public Action<AbilityTagSyncData> OnNotifyClientsAbilityTagsAdded { get; set; }
        public Action<AbilityTagSyncData> OnNotifyClientsAbilityTagsRemoved { get; set; }
        public Action<EffectSyncData> OnNotifyClientsEffectAdded { get; set; }
        public Action<string> OnNotifyClientsEffectRemoved { get; set; }
        public Action<string, int, int> OnNotifyClientsAbilityChargesChanged { get; set; }
        public Action<Tag> OnNotifyClientsTagAdded { get; set; }
        public Action<Tag> OnNotifyClientsTagRemoved { get; set; }

        public Action<string, PredictionKey, AbilityData> OnServerAbilityActivationRequested { get; set; }
        public Action<string, AbilityData> OnServerAbilityUnpredictedActivationRequested { get; set; }
        public Action<string> OnServerAbilityTerminationRequested { get; set; }
        public Action<AbilityBatchData> OnServerAbilityBatchRequested { get; set; }
        public Action<PredictionKey, bool> OnAbilityActivationResponded { get; set; }
        public Action<string, AbilityData> OnClientActivateAbility { get; set; }
        public Action<string> OnClientEndAbility { get; set; }
        public Action<string, PredictionKey> OnServerSyncKeyReceived { get; set; }
        public Action<string, PredictionKey> OnClientSyncKeyConfirmed { get; set; }

        public IDataManager DataManager { get; set; }

        public ReplicationManager(IAbilitySystem owner)
        {
            _owner = owner;

            _owner.AttributeSetManager.OnAnyAttributeBaseValueChanged += NotifyClientsAttributeBaseValueChanged;
            _owner.AttributeSetManager.OnAnyAttributeCurrentValueChanged +=
                NotifyClientsAttributeCurrentValueChanged;
            _owner.EffectManager.OnEffectStacksChanged += HandleEffectStacksChanged;
        }

        private void HandleEffectStacksChanged(Effect effect, int oldStacks, int newStacks)
        {
            if (!_owner.IsServer()) return;
            NotifyClientsEffectAdded(effect);
        }

        public void NotifyClientsAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (!_owner.IsServer()) return;
            OnNotifyClientsAttributeBaseValueChanged?.Invoke(attribute.GetName(), newValue);
        }

        public void OnAttributeBaseValueChanged(string attributeName, float newValue)
        {
            _owner.AttributeSetManager.GetAttribute(attributeName)?.SetBaseValue(newValue);
        }

        public void NotifyClientsAttributeCurrentValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (!_owner.IsServer()) return;
            OnNotifyClientsAttributeCurrentValueChanged?.Invoke(attribute.GetName(), oldValue, newValue);
        }

        public void OnAttributeCurrentValueChanged(string attributeName, float newValue)
        {
            _owner.AttributeSetManager.GetAttribute(attributeName)?.SetCurrentValue(newValue);
        }

        public void NotifyClientsPlayCue(Tag cueTag, CueAction cueAction, CueData cueData)
        {
            OnNotifyClientsPlayCue?.Invoke(cueTag, cueAction, cueData);
        }

        public void ReceivedPlayCue(Tag cueTag, CueAction cueAction, CueData cueData)
        {
            Debug.Log("Received Cue: " + cueTag.Name + " / " + cueAction + " / " + cueData + " /");
            _owner.CueManager.OnCueReceived(cueTag, cueAction, cueData);
        }

        // Abilities.
        public void NotifyClientAbilityGranted(AbilityDefinition abilityDefinition)
        {
            OnNotifyClientAbilityGranted?.Invoke(abilityDefinition);
        }

        public void NotifyClientAbilityRemoved(AbilityDefinition abilityDefinition)
        {
            if (!_owner.IsServer()) return;
            OnNotifyClientAbilityRemoved?.Invoke(abilityDefinition);
        }


        // Tags
        public void NotifyClientsAbilityTagsAdded(AbilityTagSyncData abilityTags)
        {
            OnNotifyClientsAbilityTagsAdded?.Invoke(abilityTags);
        }

        public void NotifyClientsAbilityTagsRemoved(AbilityTagSyncData abilityTags)
        {
            OnNotifyClientsAbilityTagsRemoved?.Invoke(abilityTags);
        }

        public void NotifyClientsTagAdded(Tag tag)
        {
            OnNotifyClientsTagAdded?.Invoke(tag);
        }

        public void NotifyClientsTagRemoved(Tag tag)
        {
            OnNotifyClientsTagRemoved?.Invoke(tag);
        }

        public void NotifyClientsEffectAdded(Effect effect)
        {
            if (!_owner.IsServer()) return;

            var data = new EffectSyncData
            {
                EffectName = effect.Definition.name,
                ActivationTime = effect.ActivationTime,
                PredictionKey = effect.PredictionKey,
                Level = effect.Level,
                NumStacks = effect.NumStacks
            };

            // Set By Caller sync
            if (effect.SetByCallerTagMagnitudes.Count > 0)
            {
                data.SetByCallerTags = effect.SetByCallerTagMagnitudes.Keys.ToArray();
                data.SetByCallerValues = effect.SetByCallerTagMagnitudes.Values.ToArray();
            }

            if (effect.Source != null && effect.Source.NetworkRole != null)
                data.SourceId = effect.Source.NetworkRole.NetworkObjectId;
            else
                data.SourceId = _owner.NetworkRole.NetworkObjectId;

            Debug.Log($"[ReplicationManager] NotifyClientsEffectAdded: {data.EffectName} on server={_owner.IsServer()}");
            OnNotifyClientsEffectAdded?.Invoke(data);
        }

        public void NotifyClientsEffectRemoved(Effect effect)
        {
            if (!_owner.IsServer()) return;
            OnNotifyClientsEffectRemoved?.Invoke(effect.Definition.name);
        }

        public void NotifyClientsAbilityChargesChanged(string abilityName, int current, int max)
        {
            if (!_owner.IsServer()) return;
            OnNotifyClientsAbilityChargesChanged?.Invoke(abilityName, current, max);
        }

        public void ProcessClientAbilityChargesChanged(string abilityName, int current, int max)
        {
            Debug.Log($"[ReplicationManager] ProcessClientAbilityChargesChanged for {abilityName}: {current}/{max} on server={_owner.IsServer()}");
            if (_owner.AbilityManager.Abilities.TryGetValue(abilityName, out var ability) && ability is ChargesAbility chargesAbility)
            {
                chargesAbility.SetCharges(current, max);
            }
        }

        public void ProcessClientEffectAdded(EffectSyncData data)
        {
            Debug.Log($"[ReplicationManager] ProcessClientEffectAdded: {data.EffectName} on server={_owner.IsServer()}");
            var def = DataManager.GetEffectByName(data.EffectName);
            if (def == null) 
            {
                Debug.LogWarning($"[ReplicationManager] Could not find effect definition for {data.EffectName}");
                return;
            }
            
            // source resolution might be tricky in mocks, but usually _owner is safe if source not found
            var effect = def.ToEffect(_owner, _owner); 
            effect.ActivationTime = data.ActivationTime;
            effect.PredictionKey = data.PredictionKey;
            effect.Level = data.Level;
            effect.NumStacks = data.NumStacks;
            
            if (data.SetByCallerTags != null)
            {
                for (int i = 0; i < data.SetByCallerTags.Length; i++)
                    effect.SetSetByCallerMagnitude(data.SetByCallerTags[i], data.SetByCallerValues[i]);
            }
            
            _owner.EffectManager.AddEffectFromServer(effect);
        }

        public void ProcessClientEffectRemoved(string effectName)
        {
            _owner.EffectManager.RemoveEffect(effectName);
        }

        // --- Ability Networking ---

        public bool IsBatching { get; private set; }
        private AbilityBatchData _currentBatch;

        public void BeginBatch()
        {
            IsBatching = true;
            _currentBatch = new AbilityBatchData();
        }

        public void EndBatch()
        {
            if (IsBatching)
            {
                IsBatching = false;
                if (!string.IsNullOrEmpty(_currentBatch.AbilityName))
                {
                    OnServerAbilityBatchRequested?.Invoke(_currentBatch);
                }
            }
        }

        public void RequestAbilityActivation(string name, PredictionKey key, AbilityData data)
        {
            Debug.Log($"[ReplicationManager] RequestAbilityActivation for {name} on server={_owner.IsServer()}. Has listener: {OnServerAbilityActivationRequested != null}");
            if (IsBatching)
            {
                _currentBatch.AbilityName = name;
                _currentBatch.PredictionKey = key;
                _currentBatch.ActivationData = data;
            }
            else
            {
                OnServerAbilityActivationRequested?.Invoke(name, key, data);
            }
        }

        public void RequestAbilityActivationUnpredicted(string name, AbilityData data)
        {
            OnServerAbilityUnpredictedActivationRequested?.Invoke(name, data);
        }

        public void RequestAbilityTermination(string name)
        {
            if (IsBatching && _currentBatch.AbilityName == name)
            {
                _currentBatch.EndAbilityImmediately = true;
            }
            else
            {
                OnServerAbilityTerminationRequested?.Invoke(name);
            }
        }

        public void ProcessServerAbilityBatch(AbilityBatchData batch)
        {
            ProcessServerAbilityActivation(batch.AbilityName, batch.PredictionKey, batch.ActivationData);
            if (batch.EndAbilityImmediately)
            {
                _owner.AbilityManager.EndAbility(batch.AbilityName);
            }
        }

        public void RequestClientActivateAbility(string name, AbilityData data)
        {
            OnClientActivateAbility?.Invoke(name, data);
        }

        public void RequestClientEndAbility(string name)
        {
            OnClientEndAbility?.Invoke(name);
        }

        public void ProcessServerAbilityActivation(string name, PredictionKey key, AbilityData data)
        {
            Debug.Log($"[ReplicationManager] ProcessServerAbilityActivation for {name} on server={_owner.IsServer()}");
            if (!_owner.IsServer()) return;

            if (!_owner.AbilityManager.Abilities.TryGetValue(name, out var ability) ||
                !AbilityManager.HasAuthorityToActivate(ability, true))
            {
                OnAbilityActivationResponded?.Invoke(key, false);
                return;
            }

            if (_owner.AbilityManager.ServerTryActivateAbilityWithKey(name, key, data))
            {
                OnAbilityActivationResponded?.Invoke(key, true);
            }
            else
            {
                OnAbilityActivationResponded?.Invoke(key, false);
            }
        }

        public void ProcessServerAbilityUnpredictedActivation(string name, AbilityData data)
        {
            if (!_owner.IsServer()) return;
            if (!_owner.AbilityManager.Abilities.TryGetValue(name, out var ability)) return;
            if (!AbilityManager.HasAuthorityToActivate(ability, true)) return;

            _owner.AbilityManager.TryActivateAbility(name, data);
        }

        public void ProcessServerAbilityTermination(string name)
        {
            if (!_owner.IsServer()) return;
            _owner.AbilityManager.EndAbility(name);
        }

        public void ProcessAbilityActivationConfirmed(PredictionKey key)
        {
            _owner.AbilityManager.NotifyServerResponse(key, true);
        }

        public void ProcessAbilityActivationDenied(string name, PredictionKey key)
        {
            _owner.AbilityManager.NotifyServerResponse(key, false);
        }

        public void ProcessClientActivateAbility(string name, AbilityData data)
        {
            _owner.AbilityManager.ForceActivateAbility(name, data);
        }

        public void ProcessClientEndAbility(string name)
        {
            _owner.AbilityManager.ForceEndAbility(name);
        }

        public void SendSyncKey(string abilityName, PredictionKey key)
        {
            OnServerSyncKeyReceived?.Invoke(abilityName, key);
        }

        public void ConfirmSyncKey(string abilityName, PredictionKey key)
        {
            OnClientSyncKeyConfirmed?.Invoke(abilityName, key);
        }

        public void ProcessServerSyncKey(string abilityName, PredictionKey key)
        {
            if (!_owner.IsServer()) return;
            if (_owner.AbilityManager.Abilities.TryGetValue(abilityName, out var ability))
            {
                ability.QueueSyncKey(key);
            }
        }

        public void ProcessClientSyncKeyConfirmed(string abilityName, PredictionKey key)
        {
            if (_owner.IsServer()) return;
            OnClientSyncKeyConfirmed?.Invoke(abilityName, key);
        }
    }
}