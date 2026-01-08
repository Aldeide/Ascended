using System;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystemDevelopment.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class SendCueFromServer : NetworkBehaviour
    {
        public CueDefinition Cue;
        public CueAction CueAction;

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;
            var abilitySystem = other.GetComponent<AbilitySystemComponent>();
            if (abilitySystem)
            {
                abilitySystem.NotifyClientsPlayCueRpc(Cue.CueTag, CueAction, null);
            }
        }
    }
}