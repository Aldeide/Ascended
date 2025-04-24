using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using JetBrains.Annotations;
using UnityEngine;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Runtime.AttributeSets
{
    public class AttributeSetManager
    {
        public Action<Attribute, float, float> OnAnyAttributeBaseValueChanged;
        public Action<Attribute, float, float> OnAnyAttributeCurrentValueChanged;

        private IAbilitySystem _owner;
        private Dictionary<Type, AttributeSet> _attributeSets;
        private Dictionary<string, AttributeAggregator> _attributeAggregators;

        public AttributeSetManager(IAbilitySystem owner)
        {
            _owner = owner;
            _attributeSets = new Dictionary<Type, AttributeSet>();
            _attributeAggregators = new Dictionary<string, AttributeAggregator>();
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

        public void AddAttributeSet(Type type, AttributeSet attributeSet)
        {
            _attributeSets[type] = attributeSet;
            foreach (var attribute in attributeSet.GetAllAttributes())
            {
                attribute.OnAttributeBaseValueChanged += OnAttributeBaseValueChanged;
                attribute.OnAttributeCurrentValueChanged += OnAttributeCurrentValueChanged;
                var aggregator = new AttributeAggregator(attribute, _owner);
                aggregator.Enable();
                _attributeAggregators.Add(attribute.GetName(), aggregator);
            }
        }

        [CanBeNull]
        public Attribute GetAttribute(string attributeName)
        {
            return _attributeSets.Values.SelectMany(attributeSet =>
                    attributeSet.GetAllAttributes().Where(attribute => attribute.GetName() == attributeName))
                .FirstOrDefault();
        }

        [CanBeNull]
        public Attribute GetAttribute<T>(string attributeName)
        {
            return GetAttribute(typeof(T), attributeName);
        }

        [CanBeNull]
        public Attribute GetAttribute(Type attributeSetType, string attributeName)
        {
            _attributeSets.TryGetValue(attributeSetType, out AttributeSet result);
            return result.GetAttribute(attributeName);
        }

        public Attribute GetAttribute(string attributeSetName, string attributeName)
        {
            return _attributeSets.FirstOrDefault(
                k => k.Value.Name == attributeSetName).Value.GetAttribute(attributeName);
        }

        public AttributeValue GetAttributeValue<T>(string attributeName) where T : AttributeSet
        {
            return GetAttribute<T>(attributeName).GetValue();
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

        public void RegisterOnAttributeChanged(string attributeName, Action<Attribute, float, float> action)
        {
            foreach (var attributeSet in _attributeSets.Values)
            {
                foreach (var attribute in attributeSet.GetAllAttributes())
                {
                    if (attribute.GetName() == attributeName) attribute.OnAttributeCurrentValueChanged += action;
                }
            }
        }

        public void ApplyInstantEffectModifiers(Effect instantEffect)
        {
            foreach (var modifier in instantEffect.Definition.modifiers)
            {
                var splits = modifier.attributeName.Split(".");
                var attributeSet = splits[0];
                var attributeName = splits[1];

                var attribute = GetAttribute(attributeSet, attributeName);
                if (attribute == null) continue;
                var magnitude = modifier.Calculate(instantEffect);
                var baseValue = attribute.BaseValue;
                switch (modifier.operation)
                {
                    case EffectOperation.Additive:
                        baseValue += magnitude;
                        break;
                    case EffectOperation.Subtractive:
                        baseValue -= magnitude;
                        break;
                    case EffectOperation.Multiplicative:
                        baseValue *= magnitude;
                        break;
                    case EffectOperation.Divisive:
                        baseValue /= magnitude;
                        break;
                    case EffectOperation.Override:
                        baseValue = magnitude;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                attribute.SetBaseValue(baseValue);
            }
        }

        public void OnAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            OnAnyAttributeBaseValueChanged?.Invoke(attribute, oldValue, newValue);
        }
        
        public void OnAttributeCurrentValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            OnAnyAttributeCurrentValueChanged?.Invoke(attribute, oldValue, newValue);
        }

        public string DebugString()
        {
            return _attributeSets.Values.Aggregate(
                "Attributes\n", (current, attributeSet) => current + (attributeSet.DebugString() + "\n"));
        }
    }
}