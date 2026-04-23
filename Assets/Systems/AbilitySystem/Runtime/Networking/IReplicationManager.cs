using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
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
        Action<EffectSyncData> OnNotifyClientsEffectAdded { get; set; }
        Action<string> OnNotifyClientsEffectRemoved { get; set; }

        // --- Ability Networking (New) ---
        
        // Outbound to Component
        Action<string, PredictionKey, AbilityData> OnServerAbilityActivationRequested { get; set; }
        Action<string, AbilityData> OnServerAbilityUnpredictedActivationRequested { get; set; }
        Action<string> OnServerAbilityTerminationRequested { get; set; }
        Action<PredictionKey, bool> OnAbilityActivationResponded { get; set; }
        Action<string, AbilityData> OnClientActivateAbility { get; set; }
        Action<string> OnClientEndAbility { get; set; }

        IDataManager DataManager { get; set; }
        
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

        // Effects.
        public void NotifyClientsEffectAdded(Effect effect);
        public void NotifyClientsEffectRemoved(Effect effect);

        // --- Network Processing (Inbound from Component/Managers) ---
        
        // From AbilityManager (outbound)
        void RequestAbilityActivation(string name, PredictionKey key, AbilityData data);
        void RequestAbilityActivationUnpredicted(string name, AbilityData data);
        void RequestAbilityTermination(string name);
        void RequestClientActivateAbility(string name, AbilityData data);
        void RequestClientEndAbility(string name);

        // From Component (inbound from RPC)
        void ProcessServerAbilityActivation(string name, PredictionKey key, AbilityData data);
        void ProcessServerAbilityUnpredictedActivation(string name, AbilityData data);
        void ProcessServerAbilityTermination(string name);
        
        void ProcessAbilityActivationConfirmed(PredictionKey key);
        void ProcessAbilityActivationDenied(string name, PredictionKey key);
        void ProcessClientActivateAbility(string name, AbilityData data);
        void ProcessClientEndAbility(string name);
    }
}