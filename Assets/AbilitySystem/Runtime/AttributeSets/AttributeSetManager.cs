using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using JetBrains.Annotations;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Runtime.AttributeSets
{
    public class AttributeSetManager
    {
        private IAbilitySystem _owner;
        private Dictionary<Type, AttributeSet> _attributeSets;
        
        public AttributeSetManager(IAbilitySystem owner)
        {
            _owner = owner;
            _attributeSets = new Dictionary<Type, AttributeSet>();
        }

        public T GetAttributeSet<T>() where T : AttributeSet
        {
            _attributeSets.TryGetValue(typeof(T), out AttributeSet result);
            return (T)result;
        }
        
        [CanBeNull]
        public AttributeSet GetAttributeSet(string attributeSetName)
        {
            return _attributeSets.Values.FirstOrDefault(a => a.Name == attributeSetName);
        }
        
        public void AddAttributeSet<T>(T attributeSet) where T : AttributeSet
        {
            _attributeSets[typeof(T)] = attributeSet;
        }

        public Attribute GetAttribute(Type attributeSet, string attributeName)
        {
            _attributeSets.TryGetValue(attributeSet, out AttributeSet result);
            return result.GetAttribute(attributeName);
        }

        public Dictionary<string, AttributeValue> Snapshot()
        {
            Dictionary<string, AttributeValue> output = new Dictionary<string, AttributeValue>();
            foreach (var attributeSet in _attributeSets.Values)
            {
                foreach (var attribute in attributeSet.GetAllAttributes())
                {
                    output.Add(attribute.GetName(), attribute.GetValue());
                }
            }
            return output;
        }
    }
}