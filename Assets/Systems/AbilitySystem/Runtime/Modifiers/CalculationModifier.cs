using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class CalculationModifier : Modifier, IMultiDynamicDependency
    {
        [Required]
        [AssetSelector]
        public ModifierMagnitudeCalculation calculation;

        public float baseValue = 0;

        public override void CaptureAttributes(Effect effect)
        {
            if (calculation == null) return;
            var captures = calculation.GetAttributeCaptures();
            if (captures == null) return;

            foreach (var capture in captures)
            {
                if (!capture.Snapshot) continue;

                var split = capture.AttributeName.Split('.');
                var fromAttributeSetName = split[0];
                var fromAttributeShortName = split[1];

                if (capture.CaptureSource == AttributeBasedModifier.AttributeFrom.Source)
                {
                    var attr = effect.Source.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                    if (attr != null)
                    {
                        effect.SourceCapturedAttributes[capture.AttributeName] = attr.CurrentValue;
                    }
                }
                else
                {
                    var attr = effect.Owner.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                    if (attr != null)
                    {
                        effect.OwnerCapturedAttributes[capture.AttributeName] = attr.CurrentValue;
                    }
                }
            }
        }

        public override float Calculate(Effect effect)
        {
            if (calculation == null)
            {
                Debug.LogWarning("CalculationModifier: No calculation ScriptableObject assigned!");
                return baseValue;
            }

            return calculation.CalculateMagnitude(effect, baseValue);
        }

        public IEnumerable<Attribute> GetDynamicDependencies(Effect effect)
        {
            if (calculation == null) yield break;
            var captures = calculation.GetAttributeCaptures();
            if (captures == null) yield break;

            foreach (var capture in captures)
            {
                if (capture.Snapshot) continue;

                var split = capture.AttributeName.Split('.');
                var system = capture.CaptureSource == AttributeBasedModifier.AttributeFrom.Source ? effect.Source : effect.Owner;
                var attr = system.AttributeSetManager.GetAttribute(split[0], split[1]);
                if (attr != null)
                {
                    yield return attr;
                }
            }
        }
    }
}
