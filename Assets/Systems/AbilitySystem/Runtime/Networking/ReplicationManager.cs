using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
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
        public Action<AbilitySystemComponent.AbilityTagSyncData> OnNotifyClientsAbilityTagsAdded { get; set; }

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
        public void NotifyClientsAbilityTagsAdded(AbilitySystemComponent.AbilityTagSyncData abilityTags)
        {
            OnNotifyClientsAbilityTagsAdded?.Invoke(abilityTags);
        }
    }
}