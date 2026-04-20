using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;

namespace AbilitySystem.Runtime.Networking
{
    public interface IReplicationManager
    {
        // Outbound Events (Core -> Network)
        Action<string, float> OnNotifyClientsAttributeBaseValueChanged { get; set; }
        Action<string, float, float> OnNotifyClientsAttributeCurrentValueChanged { get; set; }
        Action<Tag, CueAction, CueData> OnNotifyClientsPlayCue { get; set; }
        Action<AbilityDefinition> OnNotifyClientAbilityGranted { get; set; }
        Action<AbilityDefinition> OnNotifyClientAbilityRemoved { get; set; }
        Action<AbilityTagSyncData> OnNotifyClientsAbilityTagsAdded { get; set; }
        Action<AbilityTagSyncData> OnNotifyClientsAbilityTagsRemoved { get; set; }
        
        public void NotifyClientsAttributeBaseValueChanged(AbilitySystem.Runtime.Attributes.Attribute attribute, float oldValue, float newValue);
        public void OnAttributeBaseValueChanged(string attributeName, float newValue);
        public void NotifyClientsAttributeCurrentValueChanged(AbilitySystem.Runtime.Attributes.Attribute attribute, float oldValue, float newValue);
        public void OnAttributeCurrentValueChanged(string attributeName, float newValue);
        public void NotifyClientsPlayCue(Tag cueTag, CueAction action, CueData data);
        public void ReceivedPlayCue(Tag cueTag, CueAction action, CueData data);
        
        // Abilities.
        public void NotifyClientAbilityGranted(AbilityDefinition abilityDefinition);
        public void NotifyClientAbilityRemoved(AbilityDefinition abilityDefinition);
        
        // Tags.
        public void NotifyClientsAbilityTagsAdded(AbilityTagSyncData abilityTags);
        public void NotifyClientsAbilityTagsRemoved(AbilityTagSyncData abilityTags);
    }
}