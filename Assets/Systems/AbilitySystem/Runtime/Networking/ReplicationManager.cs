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

        public Action<string, PredictionKey, AbilityData> OnServerAbilityActivationRequested { get; set; }
        public Action<string, AbilityData> OnServerAbilityUnpredictedActivationRequested { get; set; }
        public Action<string> OnServerAbilityTerminationRequested { get; set; }
        public Action<PredictionKey, bool> OnAbilityActivationResponded { get; set; }
        public Action<string, AbilityData> OnClientActivateAbility { get; set; }
        public Action<string> OnClientEndAbility { get; set; }

        public IDataManager DataManager { get; set; }

        public ReplicationManager(IAbilitySystem owner)
        {
            _owner = owner;

            _owner.AttributeSetManager.OnAnyAttributeBaseValueChanged += NotifyClientsAttributeBaseValueChanged;
            _owner.AttributeSetManager.OnAnyAttributeCurrentValueChanged +=
                NotifyClientsAttributeCurrentValueChanged;
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

            OnNotifyClientsEffectAdded?.Invoke(data);
        }

        public void NotifyClientsEffectRemoved(Effect effect)
        {
            if (!_owner.IsServer()) return;
            OnNotifyClientsEffectRemoved?.Invoke(effect.Definition.name);
        }

        // --- Ability Networking ---

        public void RequestAbilityActivation(string name, PredictionKey key, AbilityData data)
        {
            OnServerAbilityActivationRequested?.Invoke(name, key, data);
        }

        public void RequestAbilityActivationUnpredicted(string name, AbilityData data)
        {
            OnServerAbilityUnpredictedActivationRequested?.Invoke(name, data);
        }

        public void RequestAbilityTermination(string name)
        {
            OnServerAbilityTerminationRequested?.Invoke(name);
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
    }
}