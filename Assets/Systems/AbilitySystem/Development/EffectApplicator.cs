using System;
using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Components;
using Systems.AbilitySystem.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Development
{
    public class EffectApplicator : MonoBehaviour
    {
        public EffectAsset effectAsset;
        private void OnTriggerEnter(Collider other)
        {
            AbilitySystemComponent asc = other.GetComponent<AbilitySystemComponent>();
            if (!asc) return;
            asc.AddEffect(asc, new EffectSpec(new Effect(effectAsset)));
        }
    }
}