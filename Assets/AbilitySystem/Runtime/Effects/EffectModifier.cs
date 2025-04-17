using System;
using AbilitySystem.Runtime.Modifiers;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Effects
{
    [Serializable]
    public struct EffectModifier
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
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

        public float CalculateModifier(Effect effect)
        {
            return modifierMagnitudeCalculation == null
                ? modifierMagnitude
                : modifierMagnitudeCalculation.CalculateMagnitude(effect, modifierMagnitude);
        }
        
        public void SetModiferMagnitude(float value)
        {
            modifierMagnitude = value;
        }
    }
}