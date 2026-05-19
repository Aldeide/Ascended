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
        // Cache Tag instantiation to prevent GC allocation in high-frequency physics callbacks
        private static readonly Tag DamageTag = new Tag("Data.Effect.Damage");

        public EffectDefinition EffectDefinition;
        public float DamageAmount;

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