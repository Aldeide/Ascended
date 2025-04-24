using AbilitySystem.Runtime.Core;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    // Component that handles the execution of cues to run cosmetics (vfx, audio, animation, ...).
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class CueManagerComponent : NetworkBehaviour
    {
        private IAbilitySystem _abilitySystem;
        public void Start()
        {
            _abilitySystem = GetComponent<AbilitySystemComponent>().AbilitySystem;
            // TODO: subscribe to tags being added or removed.
        }
    }
}