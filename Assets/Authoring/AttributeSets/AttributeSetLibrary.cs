using System;
using System.Collections.Generic;
using Systems.AbilitySystem.Attributes;
using Systems.Attributes;
using UnityEngine;

namespace Authoring.AttributeSets
{
    public static class AttributeSetLibrary
    {
        public static readonly Dictionary<string, Type> AttrSetTypeDict = new Dictionary<string, Type>()
        {
            { "CharacteristicsAttributeSet", typeof(CharacteristicsAttributeSet) },
        };
        
        public static List<string> AttributeFullNames = new List<string>()
        {
            "CharacteristicsAttributeSet.Health",
            "CharacteristicsAttributeSet.MaxHealth",
            "CharacteristicsAttributeSet.Energy",
            "CharacteristicsAttributeSet.MaxEnergy",
            "CharacteristicsAttributeSet.MovementSpeed",
        };
    }
    
    [Serializable]
    public class CharacteristicsAttributeSet : AttributeSet
    {
        [SerializeField]
        public AttributeBase Health = 
            new AttributeBase("CharacteristicsAttributeSet", "Health", 100, 0, 100000);
        [SerializeField]
        public AttributeBase MaxHealth = 
            new AttributeBase("CharacteristicsAttributeSet", "MaxHealth", 100, 0, 100000);
        [SerializeField]
        public AttributeBase Energy = 
            new AttributeBase("CharacteristicsAttributeSet", "Energy", 100, 0, 100000);
        [SerializeField]
        public AttributeBase MaxEnergy = 
            new AttributeBase("CharacteristicsAttributeSet", "Energy", 100, 0, 100000);
        [SerializeField]
        public AttributeBase MovementSpeed =
            new AttributeBase("CharacteristicsAttributeSet", "MovementSpeed", 4, 0, 1000);
        public override AttributeBase this[string key]
        {
            get
            {
                switch (key)
                {
                    case "Health":
                        return Health;
                    case "MaxHealth":
                        return MaxHealth;
                    case "Energy":
                        return Energy;
                    case "MaxEnergy":
                        return MaxEnergy;
                    case "MovementSpeed":
                        return MovementSpeed;
                }

                return null;
            }
        }
        public CharacteristicsAttributeSet()
        {
            this.AddAttribute("Health", Health);
            this.AddAttribute("MaxHealth", MaxHealth);
            this.AddAttribute("Energy", Energy);
            this.AddAttribute("MaxEnergy", MaxEnergy);
            this.AddAttribute("MovementSpeed", MovementSpeed);
        }
        
        public override string[] AttributeNames { get; } =
        {
            "Health",
            "MaxHealth",
            "Energy",
            "MaxEnergy",
            "MovementSpeed"
        };
        
        
    }
}