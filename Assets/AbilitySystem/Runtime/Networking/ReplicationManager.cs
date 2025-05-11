using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using Unity.Netcode;

namespace AbilitySystem.Runtime.Networking
{
    public class ReplicationManager : IReplicationManager
    {
        private readonly IAbilitySystem _owner;
        private readonly AbilitySystemComponent _networkBehaviour;

        public ReplicationManager(IAbilitySystem owner)
        {
            _owner = owner;
            _networkBehaviour = owner.Component;

            _owner.AttributeSetManager.OnAnyAttributeBaseValueChanged += NotifyClientsAttributeBaseValueChanged;
            _owner.AttributeSetManager.OnAnyAttributeCurrentValueChanged +=
                NotifyClientsAttributeCurrentValueChanged;
        }

        public void NotifyClientsAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            _networkBehaviour.NotifyClientsBaseValueChangedRpc(attribute.GetName(), newValue);
        }

        public void OnAttributeBaseValueChanged(string attributeName, float newValue)
        {
            _owner.AttributeSetManager.GetAttribute(attributeName)?.SetBaseValue(newValue);
        }

        public void NotifyClientsAttributeCurrentValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            _networkBehaviour.NotifyClientsCurrentValueChangedRpc(attribute.GetName(), oldValue, newValue);
        }

        public void OnAttributeCurrentValueChanged(string attributeName, float newValue)
        {
            _owner.AttributeSetManager.GetAttribute(attributeName)?.SetCurrentValue(newValue);
        }
    }
}