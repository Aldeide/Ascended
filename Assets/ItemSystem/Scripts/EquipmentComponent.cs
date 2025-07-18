using AbilitySystem.Scripts;
using Assets.ItemSystem.Scripts;
using GameplayTags.Runtime;
using ItemSystem.Runtime.Definition;
using ItemSystem.Runtime.Manager;
using Unity.Netcode;
using UnityEngine;

namespace ItemSystem.Scripts
{
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class EquipmentComponent : NetworkBehaviour
    {
        public EquipmentManagerDefinition EquipmentManagerDefinition;
        
        private EquipmentManager _equipmentManager;
        private AbilitySystemComponent _abilitySystemComponent;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Only the client and the server should initialise this. Observers shouldn't have any information on
            // another player's equipment.
            if (!IsServer && !IsClient) return;
            
            _abilitySystemComponent = GetComponent<AbilitySystemComponent>();
            // TODO: Figure out correct initialisation order (or maybe relying on an event is okay).
            if (_abilitySystemComponent.IsInitialized)
            {
                Initialise();
            }
            else
            {
                _abilitySystemComponent.OnAbilitySystemInitialised += Initialise;
            }
        }

        public void Initialise()
        {
            _equipmentManager = new EquipmentManager(_abilitySystemComponent.AbilitySystem, EquipmentManagerDefinition);
        }

        [Rpc(SendTo.Server)]
        public void EquipItem(Tag slotName, EquipmentDefinition equipmentDefinition)
        {
            if (!IsClient) return;
            _equipmentManager.Equip(slotName, equipmentDefinition);
        }
    }
}