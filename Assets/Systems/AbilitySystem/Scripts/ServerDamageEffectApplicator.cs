using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Scripts
{
    public class ServerDamageEffectApplicator : NetworkBehaviour
    {
        public EffectDefinition EffectDefinition;
        public float DamageAmount;
        
        // Cache the Tag instantiation to avoid GC allocation and string parsing
        // overhead during high-frequency physics callbacks like OnTriggerEnter.
        private static readonly Tag DamageTag = new Tag("Data.Effect.Damage");

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            var abilitySystem = other.GetComponent<AbilitySystemComponent>();
            if (!abilitySystem) return;
            var effect = EffectDefinition.ToEffect(abilitySystem.AbilitySystem, abilitySystem.AbilitySystem);
            effect.SetSetByCallerMagnitude(DamageTag, DamageAmount);
            abilitySystem.AbilitySystem.EffectManager.AddEffect(effect);
        }
    }
}
