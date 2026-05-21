using System;
using AbilitySystem.Scripts;
using Item.Runtime.Definition;
using Item.Runtime.Interface.Core;
using Item.Runtime.Manager;
using Systems.Item.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Item.Scripts
{
    /// <summary>
    /// NetworkBehaviour bridge between the local InventoryManager and Netcode RPCs.
    /// Owns the IItemReplicationManager shared with EquipmentComponent on the same
    /// actor. Subscribes to the replication manager's outbound delegates and forwards
    /// them as RPCs; inbound RPCs route back into Process... methods on the manager.
    /// </summary>
    public class InventoryComponent : NetworkBehaviour
    {
        [SerializeField] private StartingItemsDefinition startingItems;

        public StartingItemsDefinition StartingItems
        {
            get => startingItems;
            set => startingItems = value;
        }

        public IInventoryManager InventoryManager { get; set; }
        public IItemReplicationManager ReplicationManager { get; set; }
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
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            UnsubscribeReplication();
        }

        public void Initialise()
        {
            if (InventoryManager != null) return;

            if (ReplicationManager == null)
            {
                ReplicationManager = new ItemReplicationManager(() => IsServer, () => IsClient);
            }
            InventoryManager = new InventoryManager(_abilitySystemComponent.AbilitySystem, ReplicationManager);
            // EquipmentComponent will rebind the replication manager once it builds its EquipmentManager.
            if (ReplicationManager is ItemReplicationManager concrete)
            {
                concrete.Bind(InventoryManager, null);
            }

            SubscribeReplication();

            if (ReplicationManager.IsServer() && startingItems != null && startingItems.StartingItems != null)
            {
                foreach (var entry in startingItems.StartingItems)
                {
                    if (entry.Item != null)
                    {
                        InventoryManager.AddItem(entry.Item.Name, entry.Quantity);
                    }
                }
            }

            OnInventoryInitialised?.Invoke();
        }

        private void SubscribeReplication()
        {
            ReplicationManager.OnNotifyClientItemAdded = (name, amount) => NotifyOwnerAddItemRpc(name, amount);
            ReplicationManager.OnNotifyClientItemRemoved = (name, amount) => NotifyOwnerRemoveItemRpc(name, amount);
        }

        private void UnsubscribeReplication()
        {
            if (ReplicationManager == null) return;
            ReplicationManager.OnNotifyClientItemAdded = null;
            ReplicationManager.OnNotifyClientItemRemoved = null;
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerAddItemRpc(string itemName, int amount)
        {
            if (!IsOwner || IsServer) return;
            ReplicationManager?.ProcessClientItemAdded(itemName, amount);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerRemoveItemRpc(string itemName, int amount)
        {
            if (!IsOwner || IsServer) return;
            ReplicationManager?.ProcessClientItemRemoved(itemName, amount);
        }
    }
}
