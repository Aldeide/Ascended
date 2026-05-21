using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using Item.Runtime;
using Item.Runtime.Definition;
using Item.Runtime.Interface.Core;
using Item.Runtime.Manager;
using Item.Runtime.Modifiers;
using Systems.Item.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Item.Scripts
{
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class EquipmentComponent : NetworkBehaviour
    {
        public EquipmentManagerDefinition EquipmentManagerDefinition;
        public StartingEquipmentDefinition StartingEquipment;

        public EquipmentManager EquipmentManager => _equipmentManager;

        private EquipmentManager _equipmentManager;
        private IInventoryManager _inventoryManager;
        private IItemReplicationManager _replicationManager;
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
                _replicationManager = _inventoryComponent.ReplicationManager;
            }

            _abilitySystemComponent.OnAbilitySystemInitialised += Initialise;
            if (_inventoryComponent != null)
            {
                _inventoryComponent.OnInventoryInitialised += Initialise;
            }

            Initialise();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (_abilitySystemComponent != null)
            {
                _abilitySystemComponent.OnAbilitySystemInitialised -= Initialise;
            }
            if (_inventoryComponent != null)
            {
                _inventoryComponent.OnInventoryInitialised -= Initialise;
            }
            UnsubscribeReplication();
        }

        public void Initialise()
        {
            if (_equipmentManager != null) return;
            if (!_abilitySystemComponent.IsInitialized) return;

            if (_inventoryManager == null && _inventoryComponent != null)
            {
                _inventoryManager = _inventoryComponent.InventoryManager;
                _replicationManager = _inventoryComponent.ReplicationManager;
            }

            if (_inventoryManager == null || _inventoryManager.GetOwner() == null)
            {
                return;
            }

            _equipmentManager = new EquipmentManager(_abilitySystemComponent.AbilitySystem, _inventoryManager,
                EquipmentManagerDefinition, _replicationManager, StartingEquipment);

            // Bind back-reference so the replication manager's Process methods can mutate the equipment.
            if (_replicationManager is ItemReplicationManager concrete)
            {
                concrete.Bind(_inventoryManager, _equipmentManager);
            }

            SubscribeReplication();

            // Server pushes a snapshot to the late-joining owner so it boots with the
            // current equipment + inventory state.
            if (IsServer)
            {
                NotifyOwnerSnapshotRpc(_equipmentManager.CaptureSnapshot());
            }
            else if (IsClient && IsOwner)
            {
                // Client requests the snapshot once both managers are initialised.
                RequestSnapshotRpc();
            }
        }

        private void SubscribeReplication()
        {
            if (_replicationManager == null) return;

            _replicationManager.OnNotifyClientEquipmentEquipped = (slot, eq) => NotifyOwnerEquipRpc(slot, eq);
            _replicationManager.OnNotifyClientEquipmentUnequipped = slot => NotifyOwnerUnequipRpc(slot);
            _replicationManager.OnNotifyClientEquipmentUpgraded = (slot, level) => NotifyOwnerUpgradedRpc(slot, level);
            _replicationManager.OnNotifyClientModAdded = (slot, modSlot, mod) => NotifyOwnerModAddedRpc(slot, modSlot, mod);
            _replicationManager.OnNotifyClientModRemoved = (slot, modSlot) => NotifyOwnerModRemovedRpc(slot, modSlot);
            _replicationManager.OnNotifyClientSnapshot = snapshot => NotifyOwnerSnapshotRpc(snapshot);
            _replicationManager.OnNotifyClientEquipmentDenied = (slot, eq) => NotifyOwnerEquipDeniedRpc(slot, eq);

            _replicationManager.OnServerEquipRequested = (slot, name) => RequestEquipRpc(slot, name);
            _replicationManager.OnServerUnequipRequested = slot => RequestUnequipRpc(slot);
            _replicationManager.OnServerAddModRequested = (slot, modSlot, name) => RequestAddModRpc(slot, modSlot, name);
            _replicationManager.OnServerRemoveModRequested = (slot, modSlot) => RequestRemoveModRpc(slot, modSlot);
            _replicationManager.OnServerUpgradeRequested = slot => RequestUpgradeRpc(slot);
        }

        private void UnsubscribeReplication()
        {
            if (_replicationManager == null) return;

            _replicationManager.OnNotifyClientEquipmentEquipped = null;
            _replicationManager.OnNotifyClientEquipmentUnequipped = null;
            _replicationManager.OnNotifyClientEquipmentUpgraded = null;
            _replicationManager.OnNotifyClientModAdded = null;
            _replicationManager.OnNotifyClientModRemoved = null;
            _replicationManager.OnNotifyClientSnapshot = null;
            _replicationManager.OnNotifyClientEquipmentDenied = null;

            _replicationManager.OnServerEquipRequested = null;
            _replicationManager.OnServerUnequipRequested = null;
            _replicationManager.OnServerAddModRequested = null;
            _replicationManager.OnServerRemoveModRequested = null;
            _replicationManager.OnServerUpgradeRequested = null;
        }

        // --- Server -> Owner client RPCs ---

        [Rpc(SendTo.Owner)]
        private void NotifyOwnerEquipRpc(Tag slot, SerialisedEquipment equipment)
        {
            if (!IsOwner || IsServer) return;
            _replicationManager?.ProcessClientEquipmentEquipped(slot, equipment);
        }

        [Rpc(SendTo.Owner)]
        private void NotifyOwnerUnequipRpc(Tag slot)
        {
            if (!IsOwner || IsServer) return;
            _replicationManager?.ProcessClientEquipmentUnequipped(slot);
        }

        [Rpc(SendTo.Owner)]
        private void NotifyOwnerUpgradedRpc(Tag slot, int newLevel)
        {
            if (!IsOwner || IsServer) return;
            _replicationManager?.ProcessClientEquipmentUpgraded(slot, newLevel);
        }

        [Rpc(SendTo.Owner)]
        private void NotifyOwnerModAddedRpc(Tag slot, Tag modSlot, SerialisedModifier modifier)
        {
            if (!IsOwner || IsServer) return;
            _replicationManager?.ProcessClientModAdded(slot, modSlot, modifier);
        }

        [Rpc(SendTo.Owner)]
        private void NotifyOwnerModRemovedRpc(Tag slot, Tag modSlot)
        {
            if (!IsOwner || IsServer) return;
            _replicationManager?.ProcessClientModRemoved(slot, modSlot);
        }

        [Rpc(SendTo.Owner)]
        private void NotifyOwnerSnapshotRpc(SerialisedItemSnapshot snapshot)
        {
            if (!IsOwner || IsServer) return;
            _replicationManager?.ProcessClientSnapshot(snapshot);
        }

        [Rpc(SendTo.Owner)]
        private void NotifyOwnerEquipDeniedRpc(Tag slot, SerialisedEquipment equipment)
        {
            if (!IsOwner || IsServer) return;
            // On denial, restore the authoritative state.
            _replicationManager?.ProcessClientEquipmentEquipped(slot, equipment);
        }

        // --- Owner client -> Server RPCs (predicted requests) ---

        [Rpc(SendTo.Server)]
        private void RequestEquipRpc(Tag slot, string equipmentName)
        {
            _replicationManager?.ProcessServerEquipRequested(slot, equipmentName);
        }

        [Rpc(SendTo.Server)]
        private void RequestUnequipRpc(Tag slot)
        {
            _replicationManager?.ProcessServerUnequipRequested(slot);
        }

        [Rpc(SendTo.Server)]
        private void RequestAddModRpc(Tag slot, Tag modSlot, string modName)
        {
            _replicationManager?.ProcessServerAddModRequested(slot, modSlot, modName);
        }

        [Rpc(SendTo.Server)]
        private void RequestRemoveModRpc(Tag slot, Tag modSlot)
        {
            _replicationManager?.ProcessServerRemoveModRequested(slot, modSlot);
        }

        [Rpc(SendTo.Server)]
        private void RequestUpgradeRpc(Tag slot)
        {
            _replicationManager?.ProcessServerUpgradeRequested(slot);
        }

        [Rpc(SendTo.Server)]
        private void RequestSnapshotRpc()
        {
            if (_equipmentManager != null)
            {
                NotifyOwnerSnapshotRpc(_equipmentManager.CaptureSnapshot());
            }
        }
    }
}
