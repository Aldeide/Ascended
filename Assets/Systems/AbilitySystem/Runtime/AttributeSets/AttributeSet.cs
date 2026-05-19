using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
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
            Name = GetType().Name;
        }

        public void InitializeAttributes()
        {
            var properties = GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(Attribute))
                {
                    // Auto-instantiate the attribute if it is null
                    var attr = prop.GetValue(this) as Attribute;
                    if (attr == null)
                    {
                        attr = new Attribute(prop.Name, this, 0f);
                        if (prop.CanWrite)
                        {
                            prop.SetValue(this, attr);
                        }
                    }
                    AddAttribute(attr);
                }
            }
            
            var fields = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Attribute))
                {
                    var attr = field.GetValue(this) as Attribute;
                    if (attr == null)
                    {
                        attr = new Attribute(field.Name, this, 0f);
                        field.SetValue(this, attr);
                    }
                    AddAttribute(attr);
                }
            }
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

        public virtual bool PreGameplayEffectExecute(Effect effect, Modifier modifier, ref float magnitude)
        {
            return true;
        }

        public virtual void PostGameplayEffectExecute(Effect effect, Modifier modifier, float magnitude)
        {
        }

        public virtual void PreAttributeChange(Attribute attribute, ref float newValue)
        {
        }

        public virtual void PostAttributeChange(Attribute attribute, float oldValue, float newValue)
        {
        }

        public abstract void Reset();
    }
}