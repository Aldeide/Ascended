using System;
using System.Linq;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Attributes
{
    public class Attribute : INetworkSerializable
    {
        private AttributeSet _attributeSet;
        private AttributeValue _value;
        private string _name;

        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;

        public Func<Attribute, float, float> OnAttributeBaseValuePreChange;
        public Action<Attribute, float, float> OnAttributeBaseValueChanged;
        public Func<Attribute, float, float> OnAttributeCurrentValuePreChange;
        public Action<Attribute, float, float> OnAttributeCurrentValueChanged;

        public Attribute(string name, AttributeSet attributeSet, float baseValue,
            float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            _name = name;
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
            value = InvokePreBaseValueListeners(value);
            _value.BaseValue = value;
            _value.Clamp();
            OnAttributeBaseValueChanged?.Invoke(this, previousValue, _value.BaseValue);
        }
        
        public void SetBaseValueNoEvent(float value)
        {
            _value.BaseValue = value;
            _value.Clamp();
        }

        public void SetCurrentValue(float value)
        {
            var previousValue = _value.CurrentValue;
            value = InvokePreCurrentValueListeners(value);
            _value.CurrentValue = value;
            _value.Clamp();
            OnAttributeCurrentValueChanged?.Invoke(this, previousValue, _value.CurrentValue);
        }
        
        public void SetCurrentValueNoEvent(float value)
        {
            _value.CurrentValue = value;
        }

        public string DebugString()
        {
            return $"{_name} : Base {BaseValue}, Current {CurrentValue}";
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _name);
        }

        private float InvokePreBaseValueListeners(float value)
        {
            if (OnAttributeBaseValuePreChange == null) return value;

            return OnAttributeBaseValuePreChange.GetInvocationList().Cast<Func<Attribute, float, float>>().Aggregate(
                value, (current, listener) => listener.Invoke(this, current));
        }
        
        private float InvokePreCurrentValueListeners(float value)
        {
            if (OnAttributeCurrentValuePreChange == null) return value;

            return OnAttributeCurrentValuePreChange.GetInvocationList().Cast<Func<Attribute, float, float>>().Aggregate(
                value, (current, listener) => listener.Invoke(this, current));
        }
    }
}