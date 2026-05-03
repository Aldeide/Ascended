using System;
using AbilitySystem.Scripts;
using Item.Runtime.Interface.Core;
using Item.Runtime.Manager;
using Systems.Item.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Item.Scripts
{
    public class InventoryComponent : NetworkBehaviour
    {
        public IInventoryManager InventoryManager { get; set; }
        public ReplicationManager ReplicationManager { get; set; }
        public Action OnInventoryInitialised;
        
        private AbilitySystemComponent _abilitySystemComponent;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            _abilitySystemComponent = GetComponent<AbilitySystemComponent>();
            
            if (_abilitySystemComponent.IsInitialized)
            {
                Initialise();
            }
            else
            {
                _abilitySystemComponent.OnAbilitySystemInitialised += Initialise;
            }

            if (IsServer)
            {
                // TODO: register events if any.
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                // TODO: unregister events if any.
            }
        }
        
        private void Initialise()
        {
            if (InventoryManager != null) return;
            
            ReplicationManager = new ReplicationManager(this);
            InventoryManager = new InventoryManager(_abilitySystemComponent.AbilitySystem, ReplicationManager);
            OnInventoryInitialised?.Invoke();
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerAddItemRpc(string itemName, int amount)
        {
            if (!IsOwner) return;
            InventoryManager.AddItem(itemName, amount);
        }
    }
}