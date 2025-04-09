using System;
using Sirenix.OdinInspector;

namespace Systems.AbilitySystem.Effects.Modifiers
{
    [Serializable]
    public struct EffectModifier
    {
        [ValueDropdown("@ValueDropdownUtil.AttributeChoices", IsUniqueList = true)]
        public string attributeName;
        public string attributeSetName;
        public float modifierMagnitude;
        public EffectOperation operation;
        public ModifierMagnitudeCalculation modifierMagnitudeCalculation;

        public EffectModifier(string attributeName, float modifierMagnitude, EffectOperation operation,
            ModifierMagnitudeCalculation modifierMagnitudeCalculation)
        {
            var splits = attributeName.Split('.');
            attributeSetName = splits[0];
            this.attributeName = attributeName;
            this.modifierMagnitude = modifierMagnitude;
            this.operation = operation;
            this.modifierMagnitudeCalculation = modifierMagnitudeCalculation;
        }

        public float CalculateModifier(EffectSpec effectSpec)
        {
            return modifierMagnitudeCalculation == null
                ? modifierMagnitude
                : modifierMagnitudeCalculation.CalculateMagnitude(effectSpec, modifierMagnitude);
        }
        
        public void SetModiferMagnitude(float value)
        {
            modifierMagnitude = value;
        }
    }
}