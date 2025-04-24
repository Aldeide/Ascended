using System;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class FloatModifier : Modifier
    {
        [SerializeField]
        public float ModifierMagnitude = 1;
        
        public override float Calculate(Effect effect)
        {
            return ModifierMagnitude;
        }
    }
}