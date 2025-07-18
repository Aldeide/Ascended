using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Networking;
using GameplayTags.Runtime;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Test.Utilities
{
    public class MockReplicationManager : IReplicationManager
    {
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
    }
}