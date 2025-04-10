using System;
using UnityEngine;

namespace Systems.AbilitySystem.Attributes
{
    [Serializable]
    public struct AttributeValue
    {
        public float BaseValue { get; set; }
        [SerializeField]
        public float CurrentValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }

        public AttributeValue(float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            BaseValue = baseValue;
            CurrentValue = BaseValue;
            MinValue = minValue;
            MaxValue = maxValue;
            Clamp();
        }

        private void Clamp()
        {
            if (CurrentValue > MaxValue) CurrentValue = MaxValue;
            if (CurrentValue < MinValue) CurrentValue = MinValue;
        }
    }
}