using System;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Utilities;
using Unity.Netcode;
using UnityEditor.Presets;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public class AbilitySystemComponent : NetworkBehaviour
    {
        public AbilitySystemDefinition definition;
        public IAbilitySystem AbilitySystem { get; private set; }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public void Initialise()
        {
            AbilitySystem = new AbilitySystemManager();
            AbilitySystem.Initialise(this);
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

            AbilitySystem.EffectManager.OnEffectAdded += OnEffectAdded;
        }

        public void Update()
        {
            AbilitySystem.Tick();
        }

        public void TryActivateAbility(string abilityName)
        {
            AbilitySystem.AbilityManager.TryActivateAbility(abilityName);
        }

        [Rpc(SendTo.Server)]
        public void ServerTryActivateAbilityRpc(string abilityName, PredictionKey key)
        {
            if (!AbilitySystem.AbilityManager.TryActivateAbility(abilityName))
            {
                NotifyAbilityActivationFailedRpc(abilityName, key);
            }
        }

        [Rpc(SendTo.Owner)]
        public void NotifyAbilityActivationFailedRpc(string abilityName, PredictionKey key)
        {
            AbilitySystem.AbilityManager.EndAbility(key);
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

        public void OnEffectAdded(Effect effect)
        {
            
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerEffectAddedRpc()
        {
            
        }
    }
}