using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;


namespace AbilitySystem.Runtime.AttributeSet
{
    public abstract class AttributeSet
    {
        private IAbilitySystem _owner;
        private Dictionary<Type, Attribute.Attribute> _attributes;
        
        public string Name { get; protected set; }
        
        public AttributeSet(IAbilitySystem owner)
        {
            _attributes = new Dictionary<Type, Attribute.Attribute>();
            _owner = owner;
        }

        public void AddAttribute(Attribute.Attribute attribute)
        {
            _attributes.TryAdd(attribute.GetType(), attribute);
        }

        public void RemoveAttribute(Attribute.Attribute attribute)
        {
            _attributes.Remove(attribute.GetType());
        }
    }
}