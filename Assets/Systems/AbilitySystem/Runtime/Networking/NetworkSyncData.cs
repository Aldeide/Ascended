using GameplayTags.Runtime;
using Unity.Netcode;

namespace AbilitySystem.Runtime.Networking
{
    public struct AttributeSyncData : INetworkSerializable
    {
        public string AttributeName;
        public float BaseValue;
        public float CurrentValue;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AttributeName);
            serializer.SerializeValue(ref BaseValue);
            serializer.SerializeValue(ref CurrentValue);
        }
    }

    public struct EffectSyncData : INetworkSerializable
    {
        public string EffectName;
        public float ActivationTime;
        public ulong SourceId;
        public PredictionKey PredictionKey;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref EffectName);
            serializer.SerializeValue(ref ActivationTime);
            serializer.SerializeValue(ref SourceId);
            serializer.SerializeValue(ref PredictionKey);
        }
    }

    public struct AbilityTagSyncData : INetworkSerializable
    {
        public string AbilityUniqueName;
        public Tag[] Tags;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AbilityUniqueName);
            serializer.SerializeValue(ref Tags);
        }
    }
}