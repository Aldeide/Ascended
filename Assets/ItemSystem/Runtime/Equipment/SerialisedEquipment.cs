using System.Collections.Generic;
using ItemSystem.Runtime.Modifiers;
using Unity.Netcode;

namespace ItemSystem.Runtime
{
    public struct SerialisedEquipment : INetworkSerializable
    {
        public string Name;
        public int Level;
        public Dictionary<string, SerialisedModifier> Modifiers;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Level);
            var modifiersCount = 0;
            if (serializer.IsWriter)
            {
                modifiersCount = Modifiers?.Count ?? 0;
            }

            serializer.SerializeValue(ref modifiersCount);

            if (serializer.IsReader)
            {
                Modifiers = new Dictionary<string, SerialisedModifier>(modifiersCount);
                for (var i = 0; i < modifiersCount; i++)
                {
                    string key = default;
                    SerialisedModifier value = default;

                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);

                    Modifiers.Add(key, value);
                }
            }
            else if (Modifiers != null)
            {
                foreach (var pair in Modifiers)
                {
                    string key = pair.Key;
                    SerialisedModifier value = pair.Value;
                    serializer.SerializeValue(ref key);
                    serializer.SerializeValue(ref value);
                }
            }
        }
    }
}