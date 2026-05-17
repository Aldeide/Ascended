using AbilitySystem.Runtime.Abilities;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    /// <summary>
    /// ServerAbilityGranter is a class derived from NetworkBehaviour that provides functionality for granting abilities
    /// to entities in a server-authoritative manner utilizing Unity Netcode.
    /// </summary>
    /// <remarks>
    /// This class operates as a server-side component. It is primarily responsible for associating an ability
    /// defined in the <see cref="AbilityDefinition"/> with game objects controlled on the server.
    /// </remarks>
    public class ServerAbilityGranter: NetworkBehaviour
    {
        public AbilityDefinition AbilityDefinition;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;
            var abilitySystem = other.GetComponent<AbilitySystemComponent>();
            if (!abilitySystem) return;
            abilitySystem.AbilitySystem.AbilityManager.GrantAbility(AbilityDefinition);
        }
    }
}