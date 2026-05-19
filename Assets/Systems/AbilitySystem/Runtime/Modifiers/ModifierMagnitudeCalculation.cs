using System;
using AbilitySystem.Runtime.Effects;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public struct AttributeCaptureDefinition
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string AttributeName;
        public AttributeBasedModifier.AttributeFrom CaptureSource;
        public bool Snapshot;
    }

    [Serializable]
    public abstract class ModifierMagnitudeCalculation : ScriptableObject
    {
        public string description;

        public virtual AttributeCaptureDefinition[] GetAttributeCaptures() => Array.Empty<AttributeCaptureDefinition>();

        public abstract float CalculateMagnitude(Effect effect, float modifierMagnitude);

        protected float GetCapturedAttributeValue(Effect effect, string attributeName, AttributeBasedModifier.AttributeFrom source, bool snapshot)
        {
            var split = attributeName.Split('.');
            var fromAttributeSetName = split[0];
            var fromAttributeShortName = split[1];

            if (snapshot)
            {
                var dict = source == AttributeBasedModifier.AttributeFrom.Source 
                    ? effect.SourceCapturedAttributes 
                    : effect.OwnerCapturedAttributes;

                if (dict.TryGetValue(attributeName, out var val))
                {
                    return val;
                }
                Debug.LogWarning($"MMC: Captured attribute {attributeName} not found in snapshots! Falling back to current value.");
            }

            var system = source == AttributeBasedModifier.AttributeFrom.Source ? effect.Source : effect.Owner;
            var attr = system.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
            if (attr == null) return 0f;

            if (system.AttributeSetManager is AbilitySystem.Runtime.AttributeSets.AttributeSetManager mgr && mgr.CurrentlyCalculatingAttribute == attr)
            {
                return attr.BaseValue;
            }
            return attr.CurrentValue;
        }
    }
}