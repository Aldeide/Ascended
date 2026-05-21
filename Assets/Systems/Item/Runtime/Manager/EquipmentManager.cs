using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using GameplayTags.Runtime;
using Item.Runtime;
using Item.Runtime.Database;
using Item.Runtime.Definition;
using Item.Runtime.Interface.Core;
using Item.Runtime.Modifiers;
using Systems.Item.Runtime.Networking;

namespace Item.Runtime.Manager
{
    /// <summary>
    /// Responsible for managing Equipable Items. Handles the storage,
    /// granting, activation, deactivation, and lifecycle of abilities, while integrating
    /// with the associated <see cref="IAbilitySystem"/> owner.
    /// </summary>
    public class EquipmentManager
    {
        private readonly IAbilitySystem _owner;
        private readonly IInventoryManager _inventoryManager;
        private readonly IItemReplicationManager _replicationManager;

        private EquipmentManagerDefinition _definition;

        private readonly Dictionary<Tag, Equipment> _equipment = new();

        public event Action OnEquipmentChanged;

        public EquipmentManager(
            IAbilitySystem owner,
            IInventoryManager inventoryManager,
            EquipmentManagerDefinition definition,
            IItemReplicationManager replicationManager = null,
            StartingEquipmentDefinition startingEquipment = null)
        {
            _owner = owner;
            _inventoryManager = inventoryManager;
            _replicationManager = replicationManager;
            _definition = definition;
            foreach (var slot in _definition.EquipmentSlots)
            {
                _equipment.TryAdd(slot, null);
            }

            IEnumerable<EquipmentDefinition> initialEquipment = startingEquipment != null
                ? startingEquipment.StartingEquipment
                : _definition.Equipment;

            if (initialEquipment != null)
            {
                foreach (var equipmentDefinition in initialEquipment)
                {
                    if (equipmentDefinition == null) continue;
                    var slot = equipmentDefinition.EquipmentSlot;
                    if (!_equipment.ContainsKey(slot)) continue;
                    var equipment = new Equipment(_inventoryManager, equipmentDefinition);
                    _equipment[slot] = equipment;
                    AttachServerReplicationHooks(slot, equipment);
                    _equipment[slot].Equip();
                }
            }
        }

        /// <summary>
        /// Wires an Equipment instance's mod/upgrade events to the replication
        /// manager so server-authoritative changes are pushed to clients. Only
        /// called on the server-side path; client replays use Apply... methods.
        /// </summary>
        private void AttachServerReplicationHooks(Tag slot, Equipment equipment)
        {
            if (_replicationManager == null) return;
            equipment.OnModAdded += (modSlot, mod) =>
            {
                if (!_replicationManager.IsServer()) return;
                _replicationManager.NotifyClientModAdded(slot, modSlot, mod.ToSerializedModifier());
            };
            equipment.OnModRemoved += (modSlot, _) =>
            {
                if (!_replicationManager.IsServer()) return;
                _replicationManager.NotifyClientModRemoved(slot, modSlot);
            };
            equipment.OnUpgraded += newLevel =>
            {
                if (!_replicationManager.IsServer()) return;
                _replicationManager.NotifyClientEquipmentUpgraded(slot, newLevel);
            };
        }

        // Executed on the server.
        public void Equip(Tag slotName, EquipmentDefinition item)
        {
            if (!_owner.IsServer()) return;
            if (!_equipment.ContainsKey(slotName)) return;
            var equipment = new Equipment(_inventoryManager, item);
            _equipment[slotName] = equipment;
            AttachServerReplicationHooks(slotName, equipment);
            equipment.Equip();

            if (_replicationManager != null && _replicationManager.IsServer())
            {
                _replicationManager.NotifyClientEquipmentEquipped(slotName, equipment.ToSerializedEquipment());
            }

            OnEquipmentChanged?.Invoke();
        }

        public void Unequip(Tag slotName)
        {
            if (!_equipment.TryGetValue(slotName, out var equipment) || equipment == null) return;
            equipment.Unequip();
            _equipment[slotName] = null;

            if (_replicationManager != null && _replicationManager.IsServer())
            {
                _replicationManager.NotifyClientEquipmentUnequipped(slotName);
            }

            OnEquipmentChanged?.Invoke();
        }

        public IAbilitySystem GetOwner()
        {
            return _owner;
        }

        public Dictionary<Tag, Equipment> GetEquipment()
        {
            return _equipment;
        }

        // --- Client-side appliers (called from ItemReplicationManager.Process...) ---

        /// <summary>
        /// Replays an Equipped event coming from the server on a client. Rebuilds
        /// the local Equipment instance from the serialized payload and calls Equip()
        /// so granted abilities/effects flow through the local AbilitySystem.
        /// </summary>
        public void ApplyEquipped(Tag slotName, SerialisedEquipment serialised)
        {
            if (!_equipment.ContainsKey(slotName)) return;
            var equipment = new Equipment(_inventoryManager, serialised);
            _equipment[slotName] = equipment;
            equipment.Equip();
            OnEquipmentChanged?.Invoke();
        }

        public void ApplyUnequipped(Tag slotName)
        {
            if (!_equipment.TryGetValue(slotName, out var equipment) || equipment == null) return;
            equipment.Unequip();
            _equipment[slotName] = null;
            OnEquipmentChanged?.Invoke();
        }

        public void ApplyUpgraded(Tag slotName, int newLevel)
        {
            if (!_equipment.TryGetValue(slotName, out var equipment) || equipment == null) return;
            equipment.Level = newLevel;
            OnEquipmentChanged?.Invoke();
        }

        public void ApplyModAdded(Tag slotName, Tag modSlot, SerialisedModifier serialised)
        {
            if (!_equipment.TryGetValue(slotName, out var equipment) || equipment == null) return;
            var mod = new Modifier(_inventoryManager, serialised);
            equipment.AddMod(modSlot, mod);
            OnEquipmentChanged?.Invoke();
        }

        public void ApplyModRemoved(Tag slotName, Tag modSlot)
        {
            if (!_equipment.TryGetValue(slotName, out var equipment) || equipment == null) return;
            if (!equipment.Mods.TryGetValue(modSlot, out var mod) || mod == null) return;
            equipment.RemoveMod(modSlot, mod);
            OnEquipmentChanged?.Invoke();
        }

        // --- Snapshot for late-join clients ---

        public SerialisedItemSnapshot CaptureSnapshot()
        {
            var snapshot = new SerialisedItemSnapshot
            {
                Inventory = new Dictionary<string, int>(),
                Equipment = new Dictionary<string, SerialisedEquipment>()
            };

            if (_inventoryManager?.Items != null)
            {
                foreach (var item in _inventoryManager.Items)
                {
                    if (item == null) continue;
                    if (snapshot.Inventory.ContainsKey(item.Name))
                        snapshot.Inventory[item.Name]++;
                    else
                        snapshot.Inventory[item.Name] = 1;
                }
            }

            foreach (var pair in _equipment)
            {
                if (pair.Value == null) continue;
                snapshot.Equipment[pair.Key.Name] = pair.Value.ToSerializedEquipment();
            }

            return snapshot;
        }
    }
}
