using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using Systems.AbilitySystem.Components;
using Systems.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.AbilitySystem.Attributes
{
    [Serializable]
    public class AttributeBase
    {
        [SerializeField]
        public AttributeValue value;
        public string attributeName;
        public string attributeSetName;
        private AbilitySystemComponent _owner;
        
        protected event Action<AttributeBase, float, float> _onPostCurrentValueChange;
        protected event Action<AttributeBase, float, float> _onPostBaseValueChange;
        protected event Action<AttributeBase, float> _onPreCurrentValueChange;
        protected event Func<AttributeBase, float, float> _onPreBaseValueChange;
        protected IEnumerable<Func<AttributeBase, float, float>> _preBaseValueChangeListeners;

        public float BaseValue => value.BaseValue;
        public float CurrentValue => value.CurrentValue;
        public float MinValue => value.MinValue;
        public float MaxValue => value.MaxValue;
        
        public AttributeBase(AttributeAuthoring preset)
        {
            value = new AttributeValue(preset.BaseValue, preset.MinValue, preset.MaxValue);
        }

        public AttributeBase(string attributeSetName, string attributeName, float baseValue, float minValue, float maxValue)
        {
            this.attributeSetName = attributeSetName;
            this.attributeName = attributeName;
            value = new AttributeValue(baseValue, minValue, maxValue);
        }

        public void InitAttribute(float initialValue)
        {
            value.BaseValue = initialValue;
            value.CurrentValue = initialValue;
        }

        public void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public string AttributeName()
        {
            return attributeSetName + "." + attributeName;
        }

        public void SetCurrentValue(float newValue)
        {
            newValue = Mathf.Clamp(newValue, value.MinValue, value.MaxValue);
            _onPreCurrentValueChange?.Invoke(this, newValue);
            float oldValue = CurrentValue;
            value.CurrentValue = newValue;
            if (!Mathf.Approximately(oldValue, newValue)) _onPostCurrentValueChange?.Invoke(this, oldValue, newValue);
        }

        public float GetCurrentValue()
        {
            return value.CurrentValue;
        }
        
        public void SetBaseValue(float newValue)
        {
            if (_onPreBaseValueChange != null)
            {
                newValue = InvokePreBaseValueChangeListeners(newValue);
            }

            var oldValue = value.BaseValue;
            value.BaseValue = newValue;

            if (!Mathf.Approximately(oldValue, newValue)) _onPostBaseValueChange?.Invoke(this, oldValue, newValue);
        }

        public void SetBaseValueSilent(float newValue)
        {
            value.BaseValue = newValue;
        }

        public float GetBaseValue()
        {
            return value.BaseValue;
        }
        
        public void RegisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            _onPreBaseValueChange += func;
            _preBaseValueChangeListeners =
                _onPreBaseValueChange?.GetInvocationList().Cast<Func<AttributeBase, float, float>>();
        }

        public void RegisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostBaseValueChange += action;
        }

        public void RegisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            _onPreCurrentValueChange += action;
        }

        public void RegisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostCurrentValueChange += action;
        }

        public void UnregisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            _onPreBaseValueChange -= func;
            _preBaseValueChangeListeners =
                _onPreBaseValueChange?.GetInvocationList().Cast<Func<AttributeBase, float, float>>();
        }

        public void UnregisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostBaseValueChange -= action;
        }

        public void UnregisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            _onPreCurrentValueChange -= action;
        }

        public void UnregisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostCurrentValueChange -= action;
        }
        
        public virtual void Dispose()
        {
            _onPreBaseValueChange = null;
            _onPostBaseValueChange = null;
            _onPreCurrentValueChange = null;
            _onPostCurrentValueChange = null;
        }

        private float InvokePreBaseValueChangeListeners(float value)
        {
            if (_preBaseValueChangeListeners == null) return value;

            foreach (var t in _preBaseValueChangeListeners)
                value = t.Invoke(this, value);
            return value;
        }
    }

}
