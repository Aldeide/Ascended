using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    public class CueData : INetworkSerializable
    {
        public Vector3 position;
        public Vector3 normal;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref normal);
        }
    }
}