using System;
using System.Linq;
using UnityEngine;

namespace Systems.AbilitySystem.Tags
{
    [Serializable]
    public struct GameplayTag
    {
        [SerializeField] private string name;
        [SerializeField] private int hashCode;
        [SerializeField] private string[] ancestorsNames;
        [SerializeField] private int[] ancestorsHashCodes;
        
        public GameplayTag(string name)
        {
            this.name = name;
            hashCode = name.GetHashCode();
            var tags = name.Split('.');
            ancestorsNames = new string[tags.Length - 1];
            ancestorsHashCodes = new int[tags.Length - 1];
            var i = 0;
            var ancestorTag = "";
            while (i < tags.Length - 1)
            {
                ancestorTag += tags[i];
                ancestorsHashCodes[i] = ancestorTag.GetHashCode();
                ancestorsNames[i] = ancestorTag;
                ancestorTag += ".";
                i++;
            }
        }

        public string Name => name;

        public int HashCode => hashCode;
        
        public int[] AncestorsHashCodes => ancestorsHashCodes;

        public string[] AncestorNames => ancestorsNames;
        
        public string GetName()
        {
            return name;
        }

        public bool HasTag(GameplayTag otherGameplayTag)
        {
            return name.Contains(otherGameplayTag.GetName());
        }
        
        public bool IsAncestorOf(GameplayTag other)
        {
            return other.AncestorsHashCodes.Contains(HashCode);
        }
        
        public static bool operator ==(GameplayTag x, GameplayTag y)
        {
            return x.HashCode == y.HashCode;
        }

        public static bool operator !=(GameplayTag x, GameplayTag y)
        {
            return x.HashCode != y.HashCode;
        }
        
        public override bool Equals(object obj)
        {
            return obj is GameplayTag tag && this == tag;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}