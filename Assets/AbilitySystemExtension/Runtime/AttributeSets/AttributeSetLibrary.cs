using System;
using System.Collections.Generic;

namespace AbilitySystemExtension.Runtime.AttributeSets
{
    public static class AttributeSetLibrary
    {
        public static readonly Dictionary<string, Type> AttributeSetTypeDict = new Dictionary<string, Type>()
        {
            { "CharacteristicsAttributeSet", typeof(CharacteristicsAttributeSet) },
        };
        
        public static List<string> AttributeFullNames = new List<string>()
        {
            "CharacteristicsAttributeSet.Health",
            "CharacteristicsAttributeSet.MaxHealth",
            "CharacteristicsAttributeSet.Energy",
            "CharacteristicsAttributeSet.EnergyRegen",
            "CharacteristicsAttributeSet.MaxEnergy",
            "CharacteristicsAttributeSet.MovementSpeed",
        };
    }
}