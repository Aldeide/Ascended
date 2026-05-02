using System;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

using AbilitySystem.Runtime.Attributes;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Runtime.Modifiers
{
    public class AttributeBasedModifier : Modifier, IDynamicDependency
    {
        public AttributeCaptureType captureType;
        
        [EnumToggleButtons]
        public AttributeFrom attributeFromType;
        
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string attributeFromName;
        
        public float k = 1;
        public float b = 0;
        
        public enum AttributeFrom
        {
            Source,
            Target
        }
        
        public enum AttributeCaptureType
        {
            SnapshotOnCreation,
            OnApplication,
            Dynamic
        }
        
        public override void CaptureAttributes(Effect effect)
        {
            if (captureType != AttributeCaptureType.SnapshotOnCreation) return;
            
            var split = attributeFromName.Split(".");
            var fromAttributeSetName = split[0];
            var fromAttributeShortName = split[1];
            
            if (attributeFromType == AttributeFrom.Source)
            {
               var attr = effect.Source.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
               if (attr != null)
               {
                   effect.SourceCapturedAttributes[attributeFromName] = attr.CurrentValue;
               }
               else
               {
                   Debug.LogError($"AttributeBasedModifier: Source attribute {attributeFromName} not found on {effect.Source}");
               }
            }
            else
            {
               var attr = effect.Owner.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
               if (attr != null)
               {
                   effect.OwnerCapturedAttributes[attributeFromName] = attr.CurrentValue;
               }
               else
               {
                   Debug.LogError($"AttributeBasedModifier: Target attribute {attributeFromName} not found on {effect.Owner}");
               }
            }
        }

        public override float Calculate(Effect effect)
        {
            var split = attributeFromName.Split(".");
            var fromAttributeSetName = split[0];
            var fromAttributeShortName = split[1];
            if (attributeFromType == AttributeFrom.Source)
            {
                if (captureType == AttributeCaptureType.SnapshotOnCreation)
                {
                    if (effect.SourceCapturedAttributes.TryGetValue(attributeFromName, out var val))
                    {
                        return val * k + b;
                    }
                    Debug.LogError($"AttributeBasedModifier: Source attribute {attributeFromName} was not captured!");
                    return 0;
                }
                else
                {
                    var attribute = effect.Source.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                    return (attribute?.CurrentValue ?? 0) * k + b;
                }
            }

            if (captureType == AttributeCaptureType.SnapshotOnCreation)
            {
                if (effect.OwnerCapturedAttributes.TryGetValue(attributeFromName, out var val))
                {
                    return val * k + b;
                }
                Debug.LogError($"AttributeBasedModifier: Target attribute {attributeFromName} was not captured!");
                return 0;
            }
            else
            {
                var attribute = effect.Owner.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                return (attribute?.CurrentValue ?? 0) * k + b;
            }
        }
        public Attribute GetDynamicDependency(Effect effect)
        {
            if (captureType != AttributeCaptureType.Dynamic) return null;
            var split = attributeFromName.Split(".");
            return attributeFromType == AttributeFrom.Source 
                ? effect.Source.AttributeSetManager.GetAttribute(split[0], split[1]) 
                : effect.Owner.AttributeSetManager.GetAttribute(split[0], split[1]);
        }
    }
}