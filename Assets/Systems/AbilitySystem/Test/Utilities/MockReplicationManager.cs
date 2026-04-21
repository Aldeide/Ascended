using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Test.Utilities
{
    public class MockReplicationManager : IReplicationManager
    {
        public Action<string, float> OnNotifyClientsAttributeBaseValueChanged { get; set; }
        public Action<string, float, float> OnNotifyClientsAttributeCurrentValueChanged { get; set; }
        public Action<Tag, CueAction, CueData> OnNotifyClientsPlayCue { get; set; }
        public Action<AbilityDefinition> OnNotifyClientAbilityGranted { get; set; }
        public Action<AbilityDefinition> OnNotifyClientAbilityRemoved { get; set; }
        public Action<AbilityTagSyncData> OnNotifyClientsAbilityTagsAdded { get; set; }
        public Action<AbilityTagSyncData> OnNotifyClientsAbilityTagsRemoved { get; set; }
        public Action<EffectSyncData> OnNotifyClientsEffectAdded { get; set; }
        public Action<string> OnNotifyClientsEffectRemoved { get; set; }
        public Action<string[]> OnNotifyClientsSyncTags { get; set; }

        public Action<Attribute, float, float> NotifyClientsAttributeBaseValueChangedCallback { get; set; }

        private IAbilitySystem _owner;
        
        public MockReplicationManager(IAbilitySystem abilitySystem)
        {
            _owner = abilitySystem;
            if (_owner.IsServer())
            {
                _owner.AttributeSetManager.OnAnyAttributeBaseValueChanged += NotifyClientsAttributeBaseValueChanged;
            }
        }
        public void NotifyClientsAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            NotifyClientsAttributeBaseValueChangedCallback?.Invoke(attribute, oldValue, newValue);
        }

        public void OnAttributeBaseValueChanged(string attributeName, float newValue)
        {
            
        }

        public void NotifyClientsAttributeCurrentValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            throw new NotImplementedException();
        }

        public void OnAttributeCurrentValueChanged(string attributeName, float newValue)
        {
            throw new NotImplementedException();
        }

        public void NotifyClientsPlayCue(Tag cueTag, CueAction action, CueData data)
        {
            throw new NotImplementedException();
        }

        public void ReceivedPlayCue(Tag cueTag, CueAction action, CueData data)
        {
            throw new NotImplementedException();
        }

        public void NotifyClientAbilityGranted(AbilityDefinition abilityDefinition)
        {
            throw new NotImplementedException();
        }

        public void NotifyClientAbilityRemoved(AbilityDefinition abilityDefinition)
        {
            throw new NotImplementedException();
        }

        public void NotifyClientsAbilityTagsAdded(AbilityTagSyncData abilityTags)
        {
            OnNotifyClientsAbilityTagsAdded?.Invoke(abilityTags);
        }
        
        public void NotifyClientsAbilityTagsRemoved(AbilityTagSyncData abilityTags)
        {
            OnNotifyClientsAbilityTagsRemoved?.Invoke(abilityTags);
        }

        public void NotifyClientsAbilityTagsAdded(Tuple<string, Tag[]> abilityTags)
        {
            throw new NotImplementedException();
        }

        public void NotifyClientsSyncTags(string[] tagNames)
        {
            throw new NotImplementedException();
        }

        public void NotifyClientsEffectAdded(Effect effect)
        {
            var data = new EffectSyncData
            {
                EffectName = effect.Definition.name,
                ActivationTime = effect.ActivationTime,
                PredictionKey = effect.PredictionKey
            };
            
            if (effect.Source != null && effect.Source.NetworkRole != null)
                data.SourceId = effect.Source.NetworkRole.NetworkObjectId;
            else
                data.SourceId = 0;
                
            OnNotifyClientsEffectAdded?.Invoke(data);
        }

        public void NotifyClientsEffectRemoved(Effect effect)
        {
            OnNotifyClientsEffectRemoved?.Invoke(effect.Definition.name);
        }
    }
}