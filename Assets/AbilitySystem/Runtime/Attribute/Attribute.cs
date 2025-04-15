using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Attribute
{
    public class Attribute
    {
        private IAbilitySystem _owner;
        private AttributeSet.AttributeSet _attributeSet;
        private AttributeValue _value;
        private readonly string _name;

        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;

        public Attribute(string name, IAbilitySystem owner, AttributeSet.AttributeSet attributeSet, float baseValue,
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

        public void SetBaseValue(float value)
        {
            _value.BaseValue = value;
            _value.Clamp();
        }
        
        public void SetCurrentValue(float value)
        {
            _value.CurrentValue = value;
            _value.Clamp();
        }
    }
}