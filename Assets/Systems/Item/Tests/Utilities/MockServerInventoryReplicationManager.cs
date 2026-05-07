using System;
using GameplayTags.Runtime;
using Item.Runtime;
using Item.Runtime.Modifiers;
using Systems.Item.Runtime.Networking;

namespace Systems.Item.Tests.Utilities
{
    /// <summary>
    /// Server-side mock used by tests that don't need to verify outbound delegate
    /// invocation but do need IsServer() to be true. For verification, prefer
    /// <c>Mock&lt;IItemReplicationManager&gt;</c> via Moq.
    /// </summary>
    public class MockServerInventoryReplicationManager : IItemReplicationManager
    {
        public bool IsServer() => true;
        public bool IsClient() => false;

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

        public void NotifyClientItemAdded(string itemName, int amount) => OnNotifyClientItemAdded?.Invoke(itemName, amount);
        public void NotifyClientItemRemoved(string itemName, int amount) => OnNotifyClientItemRemoved?.Invoke(itemName, amount);
        public void NotifyClientEquipmentEquipped(Tag slot, SerialisedEquipment equipment) => OnNotifyClientEquipmentEquipped?.Invoke(slot, equipment);
        public void NotifyClientEquipmentUnequipped(Tag slot) => OnNotifyClientEquipmentUnequipped?.Invoke(slot);
        public void NotifyClientEquipmentUpgraded(Tag slot, int newLevel) => OnNotifyClientEquipmentUpgraded?.Invoke(slot, newLevel);
        public void NotifyClientModAdded(Tag slot, Tag modSlot, SerialisedModifier modifier) => OnNotifyClientModAdded?.Invoke(slot, modSlot, modifier);
        public void NotifyClientModRemoved(Tag slot, Tag modSlot) => OnNotifyClientModRemoved?.Invoke(slot, modSlot);
        public void NotifyClientSnapshot(SerialisedItemSnapshot snapshot) => OnNotifyClientSnapshot?.Invoke(snapshot);

        public void RequestEquip(Tag slot, string equipmentName) => OnServerEquipRequested?.Invoke(slot, equipmentName);
        public void RequestUnequip(Tag slot) => OnServerUnequipRequested?.Invoke(slot);
        public void RequestAddMod(Tag slot, Tag modSlot, string modName) => OnServerAddModRequested?.Invoke(slot, modSlot, modName);
        public void RequestRemoveMod(Tag slot, Tag modSlot) => OnServerRemoveModRequested?.Invoke(slot, modSlot);
        public void RequestUpgrade(Tag slot) => OnServerUpgradeRequested?.Invoke(slot);

        public void ProcessClientItemAdded(string itemName, int amount) => throw new NotImplementedException();
        public void ProcessClientItemRemoved(string itemName, int amount) => throw new NotImplementedException();
        public void ProcessClientEquipmentEquipped(Tag slot, SerialisedEquipment equipment) => throw new NotImplementedException();
        public void ProcessClientEquipmentUnequipped(Tag slot) => throw new NotImplementedException();
        public void ProcessClientEquipmentUpgraded(Tag slot, int newLevel) => throw new NotImplementedException();
        public void ProcessClientModAdded(Tag slot, Tag modSlot, SerialisedModifier modifier) => throw new NotImplementedException();
        public void ProcessClientModRemoved(Tag slot, Tag modSlot) => throw new NotImplementedException();
        public void ProcessClientSnapshot(SerialisedItemSnapshot snapshot) => throw new NotImplementedException();
        public void ProcessServerEquipRequested(Tag slot, string equipmentName) => throw new NotImplementedException();
        public void ProcessServerUnequipRequested(Tag slot) => throw new NotImplementedException();
        public void ProcessServerAddModRequested(Tag slot, Tag modSlot, string modName) => throw new NotImplementedException();
        public void ProcessServerRemoveModRequested(Tag slot, Tag modSlot) => throw new NotImplementedException();
        public void ProcessServerUpgradeRequested(Tag slot) => throw new NotImplementedException();
    }
}
