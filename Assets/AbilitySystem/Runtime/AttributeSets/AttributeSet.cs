using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using JetBrains.Annotations;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;


namespace AbilitySystem.Runtime.AttributeSets
{
    /// <summary>
    /// Abstract base class representing a set of attributes associated with an ability system.
    /// Attribute sets group related attributes and provide functionalities to manage them.
    /// Subclasses of this class define specific categories of attributes.
    /// </summary>
    public abstract class AttributeSet
    {
        protected IAbilitySystem _owner;
        private Dictionary<string, Attribute> _attributes;
        
        public string Name { get; protected set; }
        
        public AttributeSet(IAbilitySystem owner)
        {
            _attributes = new Dictionary<string, Attribute>();
            _owner = owner;
        }

        public void AddAttribute(Attribute attribute)
        {
            _attributes.TryAdd(attribute.GetName(), attribute);
        }

        public void RemoveAttribute(string attributeName)
        {
            _attributes.Remove(attributeName);
        }
        
        [CanBeNull]
        public Attribute GetAttribute(string name)
        {
            return _attributes.GetValueOrDefault(name);
        }

        public List<Attribute> GetAllAttributes()
        {
            return _attributes.Values.ToList();
        }

        public string DebugString()
        {
            var output = Name + "\n";
            return _attributes.Values.Aggregate(
                output, (current, attribute) => current + (attribute.DebugString() + "\n"));
        }

        public abstract void Reset();
    }
}