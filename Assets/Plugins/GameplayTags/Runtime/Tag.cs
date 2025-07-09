using System;
using System.Linq;
using Unity.Netcode;

namespace GameplayTags.Runtime
{
    [Serializable]
    public struct Tag : INetworkSerializable, IEquatable<Tag>
    {
        public string Name { get; set; }
        public int HashCode { get; set; }
        public string[] AncestorsNames { get; set; }
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
        }

        public bool Equals(Tag other)
        {
            return HashCode == other.HashCode;
        }
    }
}