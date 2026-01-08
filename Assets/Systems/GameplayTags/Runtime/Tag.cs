using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace GameplayTags.Runtime
{
    [Serializable]
    public struct Tag : INetworkSerializable, IEquatable<Tag>
    {
        [field: SerializeField]
        public string Name { get; set; }
        [field: SerializeField]
        public int HashCode { get; set; }
        [field: SerializeField]
        public string[] AncestorsNames { get; set; }
        [field: SerializeField]
        public int[] AncestorsHashCodes { get; set; }

        public Tag(string name)
        {
            Name = name;
            HashCode = name.GetHashCode();
            var tags = name.Split(new char[] {'.'});
            AncestorsNames = new string[tags.Length - 1];
            AncestorsHashCodes = new int[tags.Length - 1];
            var i = 0;
            var ancestorTag = "";
            while (i < tags.Length - 1)
            {
                ancestorTag += tags[i];
                AncestorsHashCodes[i] = ancestorTag.GetHashCode();
                AncestorsNames[i] = ancestorTag;
                ancestorTag += ".";
                i++;
            }
        }
        
        public bool HasTag(Tag otherGameplayTag)
        {
            return Name.Contains(otherGameplayTag.Name);
        }
        
        public bool IsAncestorOf(Tag other)
        {
            return other.AncestorsHashCodes.Contains(HashCode);
        }
        
        public static bool operator ==(Tag x, Tag y)
        {
            return x.HashCode == y.HashCode;
        }

        public static bool operator !=(Tag x, Tag y)
        {
            return x.HashCode != y.HashCode;
        }
        
        public override bool Equals(object obj)
        {
            return obj is Tag tag && this == tag;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            var value = Name;
            serializer.SerializeValue(ref value);
            if (!serializer.IsReader) return;
            // This logic is essentially a parameter-less constructor that
            // re-initializes the object based on the Name we just received.
            Name = value;
            if (string.IsNullOrEmpty(Name))
            {
                HashCode = 0;
                AncestorsNames = Array.Empty<string>();
                AncestorsHashCodes = Array.Empty<int>();
            }
            else
            {
                HashCode = Name.GetHashCode();
                var tags = Name.Split('.');
                AncestorsNames = new string[tags.Length - 1];
                AncestorsHashCodes = new int[tags.Length - 1];
                var i = 0;
                var ancestorTag = "";
                while (i < tags.Length - 1)
                {
                    ancestorTag += tags[i];
                    AncestorsHashCodes[i] = ancestorTag.GetHashCode();
                    AncestorsNames[i] = ancestorTag;
                    ancestorTag += ".";
                    i++;
                }
            }
        }
        
        public bool Equals(Tag other)
        {
            return HashCode == other.HashCode;
        }
    }
}