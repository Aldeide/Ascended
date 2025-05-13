using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    public class CueData : INetworkSerializable, IData
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