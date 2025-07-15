using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Networking
{
    /// <summary>
    /// Manages the replication of attribute and gameplay cue updates in a networked environment.
    /// </summary>
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
            if (!_owner.IsServer()) return;
            _networkBehaviour.NotifyClientsBaseValueChangedRpc(attribute.GetName(), newValue);
        }

        public void OnAttributeBaseValueChanged(string attributeName, float newValue)
        {
            _owner.AttributeSetManager.GetAttribute(attributeName)?.SetBaseValue(newValue);
        }

        public void NotifyClientsAttributeCurrentValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (!_owner.IsServer()) return;
            _networkBehaviour.NotifyClientsCurrentValueChangedRpc(attribute.GetName(), oldValue, newValue);
        }

        public void OnAttributeCurrentValueChanged(string attributeName, float newValue)
        {
            _owner.AttributeSetManager.GetAttribute(attributeName)?.SetCurrentValue(newValue);
        }

        public void NotifyClientsPlayCue(Tag cueTag, CueAction cueAction, CueData cueData)
        {
            _networkBehaviour.NotifyClientsPlayCueRpc(cueTag, cueAction, cueData);
        }

        public void ReceivedPlayCue(Tag cueTag, CueAction cueAction, CueData cueData)
        {
            Debug.Log("Received Cue: " + cueTag.Name + " / " + cueAction + " / " + cueData + " /");
            _owner.CueManager.OnCueReceived(cueTag, cueAction, cueData);
        }
    }
}