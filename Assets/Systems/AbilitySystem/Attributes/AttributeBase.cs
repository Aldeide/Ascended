using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
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
        
        
        protected event Action<AttributeBase, float, float> _onPostCurrentValueChange;
        protected event Action<AttributeBase, float, float> _onPostBaseValueChange;
        protected event Action<AttributeBase, float> _onPreCurrentValueChange;
        protected event Func<AttributeBase, float, float> _onPreBaseValueChange;
        protected IEnumerable<Func<AttributeBase, float, float>> _preBaseValueChangeListeners;

        public float BaseValue => value.BaseValue;
        public float CurrentValue => value.CurrentValue;
        
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

        public void InitAttribute(float value)
        {
            this.value.BaseValue = value;
            this.value.CurrentValue = value;
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
    }

}
