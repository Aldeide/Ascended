using System;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public abstract class ModifierMagnitudeCalculation : ScriptableObject
    {
        public string description;
#if UNITY_EDITOR
        //public string typeName = GetType().Name;
        //public string TypeFullName => GetType().FullName;
        // public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
#endif
        public abstract float CalculateMagnitude(Effect effect, float modifierMagnitude);
    }
}