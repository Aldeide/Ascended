using System;
using System.Linq;
using AbilitySystem.Runtime.AttributeSets;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Attributes
{
    /// <summary>
    /// Represents a core attribute in the ability system which holds values for base and current states.
    /// Attributes are part of an <see cref="AttributeSet"/> and can have their values clamped between a minimum and maximum range.
    /// </summary>
    public class Attribute : INetworkSerializable
    {
        private readonly AttributeSet _attributeSet;
        private AttributeValue _value;
        private string _name;
        private int _index = -1;
        private AttributeSetManager _manager;

        public virtual float BaseValue => _manager != null && _index >= 0 ? _manager.GetAttributeBaseValue(_index) : _value.BaseValue;
        public virtual float CurrentValue => _manager != null && _index >= 0 ? _manager.GetAttributeCurrentValue(_index) : _value.CurrentValue;

        public Attribute()
        {
        }

        public Func<Attribute, float, float> OnAttributeBaseValuePreChange;
        public Action<Attribute, float, float> OnAttributeBaseValueChanged;
        public Func<Attribute, float, float> OnAttributeCurrentValuePreChange;
        public Action<Attribute, float, float> OnAttributeCurrentValueChanged;

        public AttributeSet AttributeSet => _attributeSet;

        public Attribute(string name, AttributeSet attributeSet, float baseValue,
            float minValue = float.MinValue, float maxValue = float.MaxValue, int index = -1)
        {
            _name = name;
            _attributeSet = attributeSet;
            _value = new AttributeValue(baseValue, minValue, maxValue);
            _index = index;
        }

        public void SetManager(AttributeSetManager manager, int index)
        {
            _manager = manager;
            _index = index;
        }

        public int GetIndex() => _index;

        public string GetName()
        {
            return _name;
        }

        public string GetFullName()
        {
            return _attributeSet.Name + "." + _name;
        }

        public AttributeValue GetInternalValue()
        {
            return _value;
        }

        public AttributeValue GetValue()
        {
            if (_manager != null && _index >= 0)
            {
                return new AttributeValue
                {
                    BaseValue = _manager.GetAttributeBaseValue(_index),
                    CurrentValue = _manager.GetAttributeCurrentValue(_index),
                    MinValue = _manager.GetAttributeMinValue(_index),
                    MaxValue = _manager.GetAttributeMaxValue(_index)
                };
            }
            return _value;
        }

        /// <summary>
        /// Sets the base value of the attribute, applies constraints such as clamping the value within minimum
        /// and maximum limits, and invokes necessary events or listeners when the base value changes.
        /// </summary>
        /// <param name="value">The new base value to set for the attribute.</param>
        public virtual void SetBaseValue(float value)
        {
            var previousValue = BaseValue;
            _attributeSet?.PreAttributeChange(this, ref value);
            value = InvokePreBaseValueListeners(value);
            
            if (_manager != null && _index >= 0)
            {
                _manager.SetAttributeBaseValue(_index, value);
            }
            else
            {
                _value.BaseValue = value;
                _value.Clamp();
            }
            
            OnAttributeBaseValueChanged?.Invoke(this, previousValue, BaseValue);
            _attributeSet?.PostAttributeChange(this, previousValue, BaseValue);
        }
        
        public void SetBaseValueNoEvent(float value)
        {
            if (_manager != null && _index >= 0)
            {
                _manager.SetAttributeBaseValueNoEvent(_index, value);
            }
            else
            {
                _value.BaseValue = value;
                _value.Clamp();
            }
        }

        /// <summary>
        /// Sets the current value of the attribute, applies constraints such as clamping the value within
        /// the specified minimum and maximum limits, and invokes necessary events or listeners when the
        /// current value changes.
        /// </summary>
        /// <param name="value">The new current value to set for the attribute.</param>
        public void SetCurrentValue(float value)
        {
            var previousValue = CurrentValue;
            _attributeSet?.PreAttributeChange(this, ref value);
            value = InvokePreCurrentValueListeners(value);
            
            if (_manager != null && _index >= 0)
            {
                _manager.SetAttributeCurrentValue(_index, value);
            }
            else
            {
                _value.CurrentValue = value;
                _value.Clamp();
            }
            
            OnAttributeCurrentValueChanged?.Invoke(this, previousValue, CurrentValue);
            _attributeSet?.PostAttributeChange(this, previousValue, CurrentValue);
        }
        
        public void SetCurrentValueNoEvent(float value)
        {
            if (_manager != null && _index >= 0)
            {
                _manager.SetAttributeCurrentValueNoEvent(_index, value);
            }
            else
            {
                _value.CurrentValue = value;
            }
        }

        public void Restore(AttributeValue value)
        {
            var previousBase = BaseValue;
            var previousCurrent = CurrentValue;

            // Update the manager if present
            if (_manager != null && _index >= 0)
            {
                _manager.SetAttributeBaseValueNoEvent(_index, value.BaseValue);
                _manager.SetAttributeCurrentValueNoEvent(_index, value.CurrentValue);
            }
            
            _value = value;
            _value.Clamp();

            if (!Mathf.Approximately(previousBase, BaseValue))
            {
                OnAttributeBaseValueChanged?.Invoke(this, previousBase, BaseValue);
            }
            if (!Mathf.Approximately(previousCurrent, CurrentValue))
            {
                OnAttributeCurrentValueChanged?.Invoke(this, previousCurrent, CurrentValue);
            }
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