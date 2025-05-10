using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using UnityEngine;
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
    }
}