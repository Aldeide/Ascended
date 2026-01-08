using Item.Runtime.Interface.Core;
using Unity.Netcode;

namespace Item.Runtime.Modifiers
{
    public struct SerialisedModifier : INetworkSerializable
    {
        public string Name;
        public int Level;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Level);
        }
    }
}