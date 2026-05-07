using System.Collections.Generic;
using Item.Runtime;
using Unity.Netcode;

namespace Systems.Item.Runtime.Networking
{
    /// <summary>
    /// Full picture of an actor's item state. Sent to a late-joining client
    /// so it can rebuild inventory + equipment without watching every prior delta.
    /// </summary>
    public struct SerialisedItemSnapshot : INetworkSerializable
    {
        public Dictionary<string, int> Inventory; // itemName -> count
        public Dictionary<string, SerialisedEquipment> Equipment; // slotTag -> equipment

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // Inventory
            int inventoryCount = 0;
            if (serializer.IsWriter)
            {
                inventoryCount = Inventory?.Count ?? 0;
            }
            serializer.SerializeValue(ref inventoryCount);

            if (serializer.IsReader)
            {
                Inventory = new Dictionary<string, int>(inventoryCount);
                for (var i = 0; i < inventoryCount; i++)
                {
                    string key = default;
                    int value = default;
                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);
                    Inventory.Add(key, value);
                }
            }
            else if (Inventory != null)
            {
                foreach (var pair in Inventory)
                {
                    var key = pair.Key;
                    var value = pair.Value;
                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);
                }
            }

            // Equipment
            int equipmentCount = 0;
            if (serializer.IsWriter)
            {
                equipmentCount = Equipment?.Count ?? 0;
            }
            serializer.SerializeValue(ref equipmentCount);

            if (serializer.IsReader)
            {
                Equipment = new Dictionary<string, SerialisedEquipment>(equipmentCount);
                for (var i = 0; i < equipmentCount; i++)
                {
                    string key = default;
                    SerialisedEquipment value = default;
                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);
                    Equipment.Add(key, value);
                }
            }
            else if (Equipment != null)
            {
                foreach (var pair in Equipment)
                {
                    var key = pair.Key;
                    var value = pair.Value;
                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);
                }
            }
        }
    }
}
