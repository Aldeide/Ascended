using UnityEngine;

namespace Systems.AbilitySystem.Effects.Modifiers
{
    public abstract class ModifierMagnitudeCalculation : ScriptableObject
    {
        public string description;
#if UNITY_EDITOR
        //public string typeName = GetType().Name;
        //public string TypeFullName => GetType().FullName;
        // public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
#endif
        public abstract float CalculateMagnitude(EffectSpec effectSpec, float modifierMagnitude);
    }
}