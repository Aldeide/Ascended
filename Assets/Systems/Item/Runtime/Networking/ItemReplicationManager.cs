using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTags.Runtime;
using Item.Runtime;
using Item.Runtime.Database;
using Item.Runtime.Definition;
using Item.Runtime.Interface.Core;
using Item.Runtime.Manager;
using Item.Runtime.Modifiers;

namespace Systems.Item.Runtime.Networking
{
    /// <summary>
    /// Default implementation of IItemReplicationManager. Mirrors the AbilitySystem
    /// ReplicationManager: owns the inbound/outbound delegate fan-out and contains
    /// the Process... methods that mutate local state on a client without
    /// re-firing the replication path.
    /// </summary>
    public class ItemReplicationManager : IItemReplicationManager
    {
        private readonly Func<bool> _isServer;
        private readonly Func<bool> _isClient;

        private IInventoryManager _inventoryManager;
        private EquipmentManager _equipmentManager;

        public Action<string, int> OnNotifyClientItemAdded { get; set; }
        public Action<string, int> OnNotifyClientItemRemoved { get; set; }
        public Action<Tag, SerialisedEquipment> OnNotifyClientEquipmentEquipped { get; set; }
        public Action<Tag> OnNotifyClientEquipmentUnequipped { get; set; }
        public Action<Tag, int> OnNotifyClientEquipmentUpgraded { get; set; }
        public Action<Tag, Tag, SerialisedModifier> OnNotifyClientModAdded { get; set; }
        public Action<Tag, Tag> OnNotifyClientModRemoved { get; set; }
        public Action<SerialisedItemSnapshot> OnNotifyClientSnapshot { get; set; }

        public Action<Tag, string> OnServerEquipRequested { get; set; }
        public Action<Tag> OnServerUnequipRequested { get; set; }
        public Action<Tag, Tag, string> OnServerAddModRequested { get; set; }
        public Action<Tag, Tag> OnServerRemoveModRequested { get; set; }
        public Action<Tag> OnServerUpgradeRequested { get; set; }
        public Action<Tag, SerialisedEquipment> OnNotifyClientEquipmentDenied { get; set; }

        public ItemReplicationManager(Func<bool> isServer, Func<bool> isClient)
        {
            _isServer = isServer ?? (() => false);
            _isClient = isClient ?? (() => false);
        }

        /// <summary>
        /// Late-binds the managers this replicator drives. Required before any
        /// Process... call; the InventoryManager / EquipmentManager constructors
        /// take this replicator as a dependency, so the wiring is two-step.
        /// </summary>
        public void Bind(IInventoryManager inventoryManager, EquipmentManager equipmentManager)
        {
            _inventoryManager = inventoryManager;
            _equipmentManager = equipmentManager;
        }

        public bool IsServer() => _isServer();
        public bool IsClient() => _isClient();

        // --- Server -> Client outbound ---
        public void NotifyClientItemAdded(string itemName, int amount)
        {
            if (!IsServer()) return;
            OnNotifyClientItemAdded?.Invoke(itemName, amount);
        }

        public void NotifyClientItemRemoved(string itemName, int amount)
        {
            if (!IsServer()) return;
            OnNotifyClientItemRemoved?.Invoke(itemName, amount);
        }

        public void NotifyClientEquipmentEquipped(Tag slot, SerialisedEquipment equipment)
        {
            if (!IsServer()) return;
            OnNotifyClientEquipmentEquipped?.Invoke(slot, equipment);
        }

        public void NotifyClientEquipmentUnequipped(Tag slot)
        {
            if (!IsServer()) return;
            OnNotifyClientEquipmentUnequipped?.Invoke(slot);
        }

        public void NotifyClientEquipmentUpgraded(Tag slot, int newLevel)
        {
            if (!IsServer()) return;
            OnNotifyClientEquipmentUpgraded?.Invoke(slot, newLevel);
        }

        public void NotifyClientModAdded(Tag slot, Tag modSlot, SerialisedModifier modifier)
        {
            if (!IsServer()) return;
            OnNotifyClientModAdded?.Invoke(slot, modSlot, modifier);
        }

        public void NotifyClientModRemoved(Tag slot, Tag modSlot)
        {
            if (!IsServer()) return;
            OnNotifyClientModRemoved?.Invoke(slot, modSlot);
        }

        public void NotifyClientSnapshot(SerialisedItemSnapshot snapshot)
        {
            if (!IsServer()) return;
            OnNotifyClientSnapshot?.Invoke(snapshot);
        }

        // --- Client -> Server request forwarding ---
        public void RequestEquip(Tag slot, string equipmentName) => OnServerEquipRequested?.Invoke(slot, equipmentName);
        public void RequestUnequip(Tag slot) => OnServerUnequipRequested?.Invoke(slot);
        public void RequestAddMod(Tag slot, Tag modSlot, string modName) => OnServerAddModRequested?.Invoke(slot, modSlot, modName);
        public void RequestRemoveMod(Tag slot, Tag modSlot) => OnServerRemoveModRequested?.Invoke(slot, modSlot);
        public void RequestUpgrade(Tag slot) => OnServerUpgradeRequested?.Invoke(slot);

        // --- Inbound: client applies authoritative state ---
        public void ProcessClientItemAdded(string itemName, int amount)
        {
            if (IsServer() || _inventoryManager == null) return;
            _inventoryManager.AddItem(itemName, amount);
        }

        public void ProcessClientItemRemoved(string itemName, int amount)
        {
            if (IsServer() || _inventoryManager == null) return;
            var def = _inventoryManager.GetItemDefinition(itemName);
            if (def == null) return;
            var costs = new Dictionary<ItemDefinition, int> { { def, amount } };
            _inventoryManager.ConsumeItems(costs);
        }

        public void ProcessClientEquipmentEquipped(Tag slot, SerialisedEquipment serialised)
        {
            if (IsServer() || _equipmentManager == null) return;
            _equipmentManager.ApplyEquipped(slot, serialised);
        }

        public void ProcessClientEquipmentUnequipped(Tag slot)
        {
            if (IsServer() || _equipmentManager == null) return;
            _equipmentManager.ApplyUnequipped(slot);
        }

        public void ProcessClientEquipmentUpgraded(Tag slot, int newLevel)
        {
            if (IsServer() || _equipmentManager == null) return;
            _equipmentManager.ApplyUpgraded(slot, newLevel);
        }

        public void ProcessClientModAdded(Tag slot, Tag modSlot, SerialisedModifier serialised)
        {
            if (IsServer() || _equipmentManager == null) return;
            _equipmentManager.ApplyModAdded(slot, modSlot, serialised);
        }

        public void ProcessClientModRemoved(Tag slot, Tag modSlot)
        {
            if (IsServer() || _equipmentManager == null) return;
            _equipmentManager.ApplyModRemoved(slot, modSlot);
        }

        public void ProcessClientSnapshot(SerialisedItemSnapshot snapshot)
        {
            if (IsServer() || _equipmentManager == null || _inventoryManager == null) return;

            // Reset and rebuild inventory.
            _inventoryManager.Items.Clear();
            if (snapshot.Inventory != null)
            {
                foreach (var pair in snapshot.Inventory)
                {
                    _inventoryManager.AddItem(pair.Key, pair.Value);
                }
            }

            // Reset and rebuild equipment.
            if (snapshot.Equipment != null)
            {
                foreach (var pair in snapshot.Equipment)
                {
                    _equipmentManager.ApplyEquipped(new Tag(pair.Key), pair.Value);
                }
            }
        }

        // --- Inbound: server processes client requests (with prediction) ---
        public void ProcessServerEquipRequested(Tag slot, string equipmentName)
        {
            if (!IsServer() || _equipmentManager == null) return;
            var def = ItemLibrary.Instance.GetItemByName(equipmentName) as EquipmentDefinition;
            if (def == null)
            {
                // Deny: send back the current authoritative state for that slot.
                DenyEquipForSlot(slot);
                return;
            }
            _equipmentManager.Equip(slot, def);
        }

        public void ProcessServerUnequipRequested(Tag slot)
        {
            if (!IsServer() || _equipmentManager == null) return;
            _equipmentManager.Unequip(slot);
        }

        public void ProcessServerAddModRequested(Tag slot, Tag modSlot, string modName)
        {
            if (!IsServer() || _equipmentManager == null) return;
            var equipment = _equipmentManager.GetEquipment().TryGetValue(slot, out var eq) ? eq : null;
            if (equipment == null) { DenyEquipForSlot(slot); return; }
            var def = ItemLibrary.Instance.GetItemByName(modName) as ModifierDefinition;
            if (def == null) { DenyEquipForSlot(slot); return; }
            equipment.AddMod(modSlot, new Modifier(def, _inventoryManager));
        }

        public void ProcessServerRemoveModRequested(Tag slot, Tag modSlot)
        {
            if (!IsServer() || _equipmentManager == null) return;
            var equipment = _equipmentManager.GetEquipment().TryGetValue(slot, out var eq) ? eq : null;
            if (equipment == null || !equipment.Mods.TryGetValue(modSlot, out var mod) || mod == null) return;
            equipment.RemoveMod(modSlot, mod);
        }

        public void ProcessServerUpgradeRequested(Tag slot)
        {
            if (!IsServer() || _equipmentManager == null) return;
            var equipment = _equipmentManager.GetEquipment().TryGetValue(slot, out var eq) ? eq : null;
            equipment?.Upgrade();
        }

        private void DenyEquipForSlot(Tag slot)
        {
            if (_equipmentManager == null) return;
            var equipment = _equipmentManager.GetEquipment().TryGetValue(slot, out var eq) ? eq : null;
            var serialised = equipment != null ? equipment.ToSerializedEquipment() : default;
            OnNotifyClientEquipmentDenied?.Invoke(slot, serialised);
        }
    }
}
