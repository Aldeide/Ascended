using AbilitySystem.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    /// Represents data used for handling gameplay cues, including manipulation and transmission over the network.
    public struct CueData : INetworkSerializable, IData
    {
        public Vector3[] VectorData;

        public Vector3 GetVector3Data(int index)
        {
            return VectorData[index];
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref VectorData);
        }
    }
}