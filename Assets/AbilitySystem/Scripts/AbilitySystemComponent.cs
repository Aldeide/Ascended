using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Utilities;
using Unity.Netcode;
using UnityEditor.Presets;
using UnityEngine;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Scripts
{
    public class AbilitySystemComponent : NetworkBehaviour
    {
        public AbilitySystemDefinition definition;
        public IAbilitySystem AbilitySystem { get; private set; }

        private EffectDefinitionLibrary _effectLibrary;
        private CueManagerComponent _cueManagerComponent;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialise();
        }

        public void Initialise()
        {
            _effectLibrary = GameObject.Find("DataManager").GetComponent<EffectDefinitionLibrary>();
            _cueManagerComponent = GetComponent<CueManagerComponent>();
            
            AbilitySystem = new AbilitySystemManager(this);
            foreach (var attributeSet in definition.attributeSets)
            {
                Type type = ReflectionUtil.GetAttributeSetType(attributeSet);
                var set = Activator.CreateInstance(type, AbilitySystem) as AttributeSet;
                AbilitySystem.AttributeSetManager.AddAttributeSet(type, set);
            }

            foreach (var ability in definition.baseAbilities)
            {
                AbilitySystem.AbilityManager.GrantAbility(ability);
            }

            AbilitySystem.AttributeSetManager.OnAnyAttributeBaseValueChanged += OnAttributeBaseValueChanged;
            AbilitySystem.AttributeSetManager.OnAnyAttributeCurrentValueChanged += OnAttributeBaseCurrentChanged;
            AbilitySystem.EffectManager.OnEffectAdded += OnEffectAdded;
            AbilitySystem.EffectManager.OnEffectRemoved += OnEffectRemoved;
        }

        public void Update()
        {
            AbilitySystem.Tick();
        }
        
        public void OnAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (IsServer && ! IsHost)
            {
                NotifyClientsBaseValueChangedRpc(attribute.GetName(), newValue);
            }
        }
        
        public void OnAttributeBaseCurrentChanged(Attribute attribute, float oldValue, float newValue)
        {
            if (IsServer && ! IsHost)
            {
                NotifyClientsCurrentValueChangedRpc(attribute.GetName(), oldValue, newValue);
            }
        }

        [Rpc(SendTo.NotServer)]
        public void NotifyClientsBaseValueChangedRpc(string attributeName, float newValue)
        {
            AbilitySystem.ReplicationManager.OnAttributeBaseValueChanged(attributeName, newValue);
        }
        
        [Rpc(SendTo.NotServer)]
        public void NotifyClientsCurrentValueChangedRpc(string attributeName, float oldValue, float newValue)
        {
            AbilitySystem.AttributeSetManager.GetAttribute(attributeName).SetCurrentValue(newValue);
        }

        public void TryActivateAbility(string abilityName, AbilityData data = new())
        {
            AbilitySystem.AbilityManager.TryActivateAbility(abilityName, data);
        }

        [Rpc(SendTo.Server)]
        public void ServerTryActivateAbilityRpc(string abilityName, PredictionKey key, AbilityData data)
        {
            if (!AbilitySystem.AbilityManager.ServerTryActivateAbilityWithKey(abilityName, key, data))
            {
                NotifyAbilityActivationFailedRpc(abilityName, key);
            }
        }
        
        [Rpc(SendTo.Server)]
        public void ServerTryEndAbilityRpc(string abilityName)
        {
            AbilitySystem.AbilityManager.EndAbility(abilityName);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyAbilityActivationFailedRpc(string abilityName, PredictionKey key)
        {
            AbilitySystem.AbilityManager.EndAbility(key);
            AbilitySystem.EffectManager.RetractPredictedEffect(key);
        }

        public void EndAbility(string abilityName)
        {
            AbilitySystem.AbilityManager.EndAbility(abilityName);
        }

        public void ApplyEffect(EffectDefinition effectDefinition)
        {
            var effect = effectDefinition.ToEffect(AbilitySystem, AbilitySystem);
            effect.Activate();
            AbilitySystem.EffectManager.AddEffect(effect);
        }

        public void ExecuteEffect(EffectDefinition effectDefinition, IAbilitySystem source)
        {
            Debug.Log("Executing effect");
            var effect = effectDefinition.ToEffect(source, AbilitySystem);
            effect.Execute();
        }

        public void OnEffectAdded(Effect effect)
        {
            if (IsServer && !IsHost)
            {
                if (effect.PredictionKey.IsValidKey())
                {
                    NotifyOwnerEffectAddedRpc(effect.PredictionKey, effect.Definition.name, effect.ActivationTime);
                    return;
                }
                NotifyOwnerEffectAddedRpc(effect.Definition.name, effect.ActivationTime);
            }
        }
        
        public void OnEffectRemoved(Effect effect)
        {
            if (IsServer && !IsHost)
            {
                NotifyOwnerEffectRemovedRpc(effect.Definition.name);
            }
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectAddedRpc(string effectName, float applicationTime)
        {
            if (IsServer) return;
            var effectDefinition = _effectLibrary.GetEffectByName(effectName);
            // TODO: find an identifier to identify the abilitysystem and source player.
            var effect = effectDefinition.ToEffect(AbilitySystem, AbilitySystem);
            effect.ActivationTime = applicationTime;
            AbilitySystem.EffectManager.AddEffectFromServer(effect);
        }
        
        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectAddedRpc(PredictionKey key,string effectName, float applicationTime)
        {
            if (IsServer) return;
            var effectDefinition = _effectLibrary.GetEffectByName(effectName);
            // TODO: find an identifier to identify the abilitysystem and source player.
            var effect = effectDefinition.ToEffect(AbilitySystem, AbilitySystem);
            effect.ActivationTime = applicationTime;
            AbilitySystem.EffectManager.ReconcilePredictedEffect(key);
        }
        
        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectRemovedRpc(string effectName)
        {
            AbilitySystem.EffectManager.RemoveEffect(effectName);
        }

        // TODO: only send to observers if cue is predicted.
        [Rpc(SendTo.Everyone)]
        public void ObserversPlayCueRpc(string cueTag)
        {
            _cueManagerComponent.PlayCue(cueTag);
        }
        
        // TODO: only send to observers if cue is predicted.
        [Rpc(SendTo.Everyone)]
        public void ObserversPlayCueWithDataRpc(string cueTag, CueData data)
        {
            _cueManagerComponent.PlayCue(cueTag, data);
        }
    }
}