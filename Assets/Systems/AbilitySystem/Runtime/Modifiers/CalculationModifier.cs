using System;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class CalculationModifier : Modifier
    {
        [Required]
        [AssetSelector]
        public ModifierMagnitudeCalculation calculation;

        public float baseValue = 0;

        public override float Calculate(Effect effect)
        {
            if (calculation == null)
            {
                Debug.LogWarning("CalculationModifier: No calculation ScriptableObject assigned!");
                return baseValue;
            }

            return calculation.CalculateMagnitude(effect, baseValue);
        }
    }
}
