using System;
using AbilitySystem.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    /// Represents data used for handling gameplay cues, including manipulation and transmission over the network.
    public class CueData : INetworkSerializable, IData
    {
        public Vector3[] VectorData = Array.Empty<Vector3>();
        public float Magnitude;
        public Vector3 Normal;
        public ulong SourceId;
        public ulong TargetId;
        public PredictionKey PredictionKey;

        public Vector3 GetVector3Data(int index)
        {
            return VectorData[index];
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref VectorData);
            serializer.SerializeValue(ref Magnitude);
            serializer.SerializeValue(ref Normal);
            serializer.SerializeValue(ref SourceId);
            serializer.SerializeValue(ref TargetId);
            PredictionKey.NetworkSerialize(serializer);
        }
    }
}