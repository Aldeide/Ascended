using System;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.Targeting
{
    /// <summary>
    /// Base interface for all types of targeting data sent from client to server.
    /// </summary>
    public interface ITargetData : INetworkSerializable
    {
        // Add common properties if needed
    }

    [Serializable]
    public struct TargetDataLocation : ITargetData
    {
        public Vector3 Position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
        }
    }

    [Serializable]
    public struct TargetDataActor : ITargetData
    {
        public ulong NetworkObjectId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref NetworkObjectId);
        }
    }

    [Serializable]
    public struct TargetDataHitResult : ITargetData
    {
        public Vector3 Position;
        public Vector3 Normal;
        public ulong NetworkObjectId;
        public int ColliderIndex;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Normal);
            serializer.SerializeValue(ref NetworkObjectId);
            serializer.SerializeValue(ref ColliderIndex);
        }
    }
}
