using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Attributes
{
    /// <summary>
    /// Represents a value associated with an attribute, including its base, current, minimum, and maximum values.
    /// </summary>
    /// <remarks>
    /// This struct is used for handling and manipulating the attribute values in the system.
    /// It ensures the values are clamped within the provided minimum and maximum bounds.
    /// </remarks>
    [Serializable]
    public struct AttributeValue
    {
        public float BaseValue { get; set; }
        public float CurrentValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }

        public AttributeValue(float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            BaseValue = baseValue;
            BaseValue = Mathf.Clamp(BaseValue, MinValue, MaxValue);
            CurrentValue = BaseValue;
        }

        public void Clamp()
        {
            BaseValue = Mathf.Clamp(BaseValue, MinValue, MaxValue);
            CurrentValue = Mathf.Clamp(CurrentValue, MinValue, MaxValue);
        }
    }
}