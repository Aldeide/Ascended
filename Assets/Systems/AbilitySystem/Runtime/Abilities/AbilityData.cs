using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public struct AbilityData : INetworkSerializable
    {
        public Vector3 MuzzlePosition;
        public Vector3 TargetPosition;
        public Targeting.TargetDataHandle TargetData;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref MuzzlePosition);
            serializer.SerializeValue(ref TargetPosition);
            TargetData.NetworkSerialize(serializer);
        }
    }
}