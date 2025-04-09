using System;
using System.Collections.Generic;
using Systems.AbilitySystem.Attributes;
using Systems.Attributes;
using UnityEngine;

namespace Authoring.AttributeSets
{
    public static class AttributeSetLibrary
    {
        public static List<string> AttributeFullNames = new List<string>()
        {
            "CharacteristicsAttributeSet.Health",
            "CharacteristicsAttributeSet.MovementSpeed",
        };
    }
    
    [Serializable]
    public class CharacteristicsAttributeSet : AttributeSet
    {
        [SerializeField]
        public AttributeBase Health = 
            new AttributeBase("CharacteristicsAttributeSet", "Health", 100, 0, 100000);

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
                    case "MovementSpeed":
                        return MovementSpeed;
                }

                return null;
            }
        }
        public CharacteristicsAttributeSet()
        {
            this.AddAttribute("Health", Health);
            this.AddAttribute("MovementSpeed", MovementSpeed);
        }
        
        public override string[] AttributeNames { get; } =
        {
            "Health",
            "MovementSpeed"
        };
        
        
    }
}