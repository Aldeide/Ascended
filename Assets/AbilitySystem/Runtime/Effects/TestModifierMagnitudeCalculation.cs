using System;
using AbilitySystem.Runtime.Modifiers;
using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    [Serializable]
    public class TestModifierMagnitudeCalculation : ModifierMagnitudeCalculation
    {
        [SerializeField]
        public string TestField;
        public override float CalculateMagnitude(Effect effect, float modifierMagnitude)
        {
            return 5.0f;
        }
    }
}