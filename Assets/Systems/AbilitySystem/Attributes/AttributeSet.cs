using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using Systems.AbilitySystem.Attributes;
using Systems.AbilitySystem.Components;
using UnityEngine;

namespace Systems.Attributes
{
    [Serializable]
    public abstract class AttributeSet
    {
        public Dictionary<string, AttributeBase> Attributes = new();
        public abstract string[] AttributeNames { get; }
        public abstract AttributeBase this[string key] { get; }
        
        private AbilitySystemComponent _asc;

        public AttributeSet(AttributeSetAuthoring preset)
        {
            foreach (var attribute in preset.Attributes)
            {
                Debug.Log("Adding: " + attribute.Name);
                Attributes.Add(attribute.Name, new AttributeBase(attribute));
            }
        }

        public AttributeSet()
        {
            
        }

        public void SetOwner(AbilitySystemComponent asc)
        {
            _asc = asc;
            foreach (var attributeBase in Attributes.Values)
            {
                attributeBase.SetOwner(asc);
            }
        }

        public void AddAttribute(string name, AttributeBase attribute)
        {
            Attributes.Add(name, attribute);
        }

        public void SetCurrentValue(string attributeName, float newValue)
        {
            Attributes[attributeName].SetCurrentValue(newValue);
        }

        public void SetBaseValue(string attributeName, float newValue)
        {
            Attributes[attributeName].SetBaseValue(newValue);
        }
        
    }
}