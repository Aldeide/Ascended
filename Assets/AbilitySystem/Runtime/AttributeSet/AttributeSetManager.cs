using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.AttributeSet
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
        
        public void AddAttributeSet<T>(T attributeSet) where T : AttributeSet
        {
            _attributeSets[typeof(T)] = attributeSet;
        }
    }
}