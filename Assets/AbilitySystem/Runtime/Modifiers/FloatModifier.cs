using System;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class FloatModifier : Modifier
    {
        // A simple modifier that adds, subtracts, multiplies or divides a modifier with a single float value.
        [SerializeField]
        public float ModifierMagnitude = 1;
        
        public override float Calculate(Effect effect)
        {
            return ModifierMagnitude;
        }
    }
}