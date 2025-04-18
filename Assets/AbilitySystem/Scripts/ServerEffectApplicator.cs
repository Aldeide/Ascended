using System;
using AbilitySystem.Runtime.Effects;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public class ServerEffectApplicator : NetworkBehaviour
    {
        public EffectDefinition effectDefinition;

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            var abilitySystem = other.GetComponent<AbilitySystemComponent>();
            if (!abilitySystem) return;
            
            abilitySystem.ApplyEffect(effectDefinition);
        }
    }
}