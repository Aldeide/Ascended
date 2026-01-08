using System;
using GameplayTags.Runtime;
using Unity.Netcode;

namespace AbilitySystem.Runtime.Cues
{
    public struct ActiveCueState : INetworkSerializable, IEquatable<ActiveCueState>
    {
        public char[] CueTag;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CueTag);
        }

        public bool Equals(ActiveCueState other)
        {
            return CueTag.Equals(other.CueTag);
        }

        public override int GetHashCode()
        {
            return CueTag.GetHashCode();
        }
    }

}