using System;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Attributes
{
    public class Attribute
    {
        private IAbilitySystem _owner;
        private AttributeSet _attributeSet;
        private AttributeValue _value;
        private readonly string _name;

        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;

        public Action<Attribute, float, float> OnAttributeBaseValuePreChange;
        public Action<Attribute, float, float> OnAttributeBaseValueChanged;
        public Action<Attribute, float, float> OnAttributeCurrentValuePreChange;
        public Action<Attribute, float, float> OnAttributeCurrentValueChanged;
        
        public Attribute(string name, IAbilitySystem owner, AttributeSet attributeSet, float baseValue,
            float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            _name = name;
            _owner = owner;
            _attributeSet = attributeSet;
            _value = new AttributeValue(baseValue, minValue, maxValue);
        }

        public string GetName()
        {
            return _name;
        }

        public string GetFullName()
        {
            return _attributeSet.Name + "." + _name;
        }

        public AttributeValue GetValue()
        {
            return _value;
        }

        public void SetBaseValue(float value)
        {
            var previousValue = _value.BaseValue;
            _value.BaseValue = value;
            _value.Clamp();
            OnAttributeBaseValueChanged?.Invoke(this, previousValue, _value.BaseValue);
        }
        
        public void SetCurrentValue(float value)
        {
            var previousValue = _value.CurrentValue;
            _value.CurrentValue = value;
            _value.Clamp();
            OnAttributeCurrentValueChanged?.Invoke(this, previousValue, _value.CurrentValue);
        }

        public string DebugString()
        {
            return $"{_name} : Base {BaseValue}, Current {CurrentValue}";
        }
    }
}