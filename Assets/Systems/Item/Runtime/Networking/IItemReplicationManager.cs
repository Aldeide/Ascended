using System;
using GameplayTags.Runtime;
using Item.Runtime;
using Item.Runtime.Modifiers;

namespace Systems.Item.Runtime.Networking
{
    /// <summary>
    /// Replication contract for the Item system. Mirrors the AbilitySystem's
    /// IReplicationManager pattern: managers raise mutation events, this manager
    /// fans them out via Action delegates, and the NetworkBehaviour wires those
    /// delegates to RPCs. Inbound RPCs land back in the Process... methods,
    /// which apply state locally without triggering a re-replication loop.
    /// </summary>
    public interface IItemReplicationManager
    {
        // --- Network role ---
        bool IsServer();
        bool IsClient();

        // --- Outbound (Server -> Client) ---
        // Inventory
        Action<string, int> OnNotifyClientItemAdded { get; set; }
        Action<string, int> OnNotifyClientItemRemoved { get; set; }

        // Equipment
        Action<Tag, SerialisedEquipment> OnNotifyClientEquipmentEquipped { get; set; }
        Action<Tag> OnNotifyClientEquipmentUnequipped { get; set; }
        Action<Tag, int> OnNotifyClientEquipmentUpgraded { get; set; }
        Action<Tag, Tag, SerialisedModifier> OnNotifyClientModAdded { get; set; }
        Action<Tag, Tag> OnNotifyClientModRemoved { get; set; }

        // Late-join snapshot
        Action<SerialisedItemSnapshot> OnNotifyClientSnapshot { get; set; }

        // --- Outbound (Client -> Server) — prediction requests ---
        Action<Tag, string> OnServerEquipRequested { get; set; }
        Action<Tag> OnServerUnequipRequested { get; set; }
        Action<Tag, Tag, string> OnServerAddModRequested { get; set; }
        Action<Tag, Tag> OnServerRemoveModRequested { get; set; }
        Action<Tag> OnServerUpgradeRequested { get; set; }

        // Server -> Client denial (prediction rollback)
        Action<Tag, SerialisedEquipment> OnNotifyClientEquipmentDenied { get; set; }

        // --- Server-side notifications (called by managers) ---
        void NotifyClientItemAdded(string itemName, int amount);
        void NotifyClientItemRemoved(string itemName, int amount);
        void NotifyClientEquipmentEquipped(Tag slot, SerialisedEquipment equipment);
        void NotifyClientEquipmentUnequipped(Tag slot);
        void NotifyClientEquipmentUpgraded(Tag slot, int newLevel);
        void NotifyClientModAdded(Tag slot, Tag modSlot, SerialisedModifier modifier);
        void NotifyClientModRemoved(Tag slot, Tag modSlot);
        void NotifyClientSnapshot(SerialisedItemSnapshot snapshot);

        // --- Server-side request forwarding (called by client managers) ---
        void RequestEquip(Tag slot, string equipmentName);
        void RequestUnequip(Tag slot);
        void RequestAddMod(Tag slot, Tag modSlot, string modName);
        void RequestRemoveMod(Tag slot, Tag modSlot);
        void RequestUpgrade(Tag slot);

        // --- Inbound processors (called by NetworkBehaviour from RPCs) ---
        void ProcessClientItemAdded(string itemName, int amount);
        void ProcessClientItemRemoved(string itemName, int amount);
        void ProcessClientEquipmentEquipped(Tag slot, SerialisedEquipment equipment);
        void ProcessClientEquipmentUnequipped(Tag slot);
        void ProcessClientEquipmentUpgraded(Tag slot, int newLevel);
        void ProcessClientModAdded(Tag slot, Tag modSlot, SerialisedModifier modifier);
        void ProcessClientModRemoved(Tag slot, Tag modSlot);
        void ProcessClientSnapshot(SerialisedItemSnapshot snapshot);

        void ProcessServerEquipRequested(Tag slot, string equipmentName);
        void ProcessServerUnequipRequested(Tag slot);
        void ProcessServerAddModRequested(Tag slot, Tag modSlot, string modName);
        void ProcessServerRemoveModRequested(Tag slot, Tag modSlot);
        void ProcessServerUpgradeRequested(Tag slot);
    }
}
