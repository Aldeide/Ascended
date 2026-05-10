using System;
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
        public int Level;
        public int NumStacks;
        public Tag[] SetByCallerTags;
        public float[] SetByCallerValues;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref EffectName);
            serializer.SerializeValue(ref ActivationTime);
            serializer.SerializeValue(ref SourceId);
            serializer.SerializeValue(ref PredictionKey);
            serializer.SerializeValue(ref Level);
            serializer.SerializeValue(ref NumStacks);

            // Array serialization requires non-null arrays in some NGO versions or specific contexts
            if (!serializer.IsReader)
            {
                SetByCallerTags ??= Array.Empty<Tag>();
                SetByCallerValues ??= Array.Empty<float>();
            }

            serializer.SerializeValue(ref SetByCallerTags);
            serializer.SerializeValue(ref SetByCallerValues);
        }
    }

    public struct AbilityTagSyncData : INetworkSerializable
    {
        public string AbilityUniqueName;
        public Tag[] Tags;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AbilityUniqueName);

            if (!serializer.IsReader)
            {
                Tags ??= Array.Empty<Tag>();
            }

            serializer.SerializeValue(ref Tags);
        }
    }
}