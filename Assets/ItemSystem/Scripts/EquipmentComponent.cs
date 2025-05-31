using AbilitySystem.Scripts;
using ItemSystem.Runtime.Manager;
using Unity.Netcode;
using UnityEngine;

namespace ItemSystem.Scripts
{
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class EquipmentComponent : NetworkBehaviour
    {
        public EquipmentManager EquipmentManager;
        
        private AbilitySystemComponent _abilitySystemComponent;
        private InventoryComponent _inventoryComponent;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _abilitySystemComponent = GetComponent<AbilitySystemComponent>();
            _inventoryComponent = GetComponent<InventoryComponent>();
            EquipmentManager = new EquipmentManager(_abilitySystemComponent.AbilitySystem);
        }
    }
}