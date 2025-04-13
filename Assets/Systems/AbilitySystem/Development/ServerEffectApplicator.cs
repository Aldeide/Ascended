using FishNet.Object;
using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Components;
using Systems.AbilitySystem.Effects;
using UnityEngine;

namespace Systems.Development
{
    public class ServerEffectApplicator : NetworkBehaviour
    {
        public EffectAsset effectAsset;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServerInitialized) return;
            Debug.Log("OnTriggerEnter Server Side");
            AbilitySystemComponent asc = other.GetComponent<AbilitySystemComponent>();
            if (!asc) return;
            asc.AddEffect(new EffectSpec(new Effect(effectAsset)));
        }
    }
}