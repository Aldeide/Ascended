using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using Item.Runtime.Definition;
using Item.Runtime.Interface.Core;
using Item.Runtime.Manager;
using Unity.Netcode;
using UnityEngine;

namespace Item.Scripts
{
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class EquipmentComponent : NetworkBehaviour
    {
        public EquipmentManagerDefinition EquipmentManagerDefinition;

        public EquipmentManager EquipmentManager => _equipmentManager;
        
        private EquipmentManager _equipmentManager;
        private IInventoryManager _inventoryManager;
        private AbilitySystemComponent _abilitySystemComponent;

        private InventoryComponent _inventoryComponent;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Only the client and the server should initialise this. Observers shouldn't have any information on
            // another player's equipment.
            if (!IsServer && !IsClient) return;

            _abilitySystemComponent = GetComponent<AbilitySystemComponent>();
            _inventoryComponent = GetComponent<InventoryComponent>();
            
            if (_inventoryComponent != null)
            {
                _inventoryManager = _inventoryComponent.InventoryManager;
            }

            // We need both the AbilitySystem and the InventoryManager to be initialized.
            // We subscribe to both and check in Initialise() if we have everything.
            _abilitySystemComponent.OnAbilitySystemInitialised += Initialise;
            if (_inventoryComponent != null)
            {
                _inventoryComponent.OnInventoryInitialised += Initialise;
            }

            Initialise();
        }

        public void Initialise()
        {
            if (_equipmentManager != null) return;
            if (!_abilitySystemComponent.IsInitialized) return;
            
            // Ensure we have an inventory manager if we didn't find it yet
            if (_inventoryManager == null && _inventoryComponent != null)
            {
                _inventoryManager = _inventoryComponent.InventoryManager;
            }

            if (_inventoryManager == null || _inventoryManager.GetOwner() == null)
            {
                // Wait for inventory manager to be fully ready with its owner
                return;
            }

            _equipmentManager = new EquipmentManager(_abilitySystemComponent.AbilitySystem, _inventoryManager,
                EquipmentManagerDefinition);
        }

        [Rpc(SendTo.Server)]
        public void EquipItemRpc(Tag slotName /*, EquipmentDefinition equipmentDefinition*/)
        {
            if (!IsClient) return;
            // TODO: EquipmentDefinition probably isn't serialisable. Need to defined a serialised version that also
            // contains mods to send over the network.
            //_equipmentManager.Equip(slotName, equipmentDefinition);
        }
    }
}