using System;

namespace AbilitySystem.Runtime.Attributes
{
    [Serializable]
    public struct AttributeState
    {
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;

        public AttributeState(float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            BaseValue = baseValue;
            MinValue = minValue;
            MaxValue = maxValue;
            CurrentValue = baseValue;
        }
    }
}
