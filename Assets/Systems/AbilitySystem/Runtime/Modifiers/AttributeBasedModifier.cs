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
            OnApplication
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
               effect.SourceCapturedAttributes[attributeFromName] = attr.CurrentValue;
            }
            else
            {
               var attr = effect.Owner.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
               effect.OwnerCapturedAttributes[attributeFromName] = attr.CurrentValue;
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
                    return effect.SourceCapturedAttributes[attributeFromName] * k + b;
                }
                else
                {
                    var attribute = effect.Source.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                    return attribute.CurrentValue * k + b;
                }
            }

            if (captureType == AttributeCaptureType.SnapshotOnCreation)
            {
                return effect.OwnerCapturedAttributes[attributeFromName] * k + b;
            }
            else
            {
                var attribute = effect.Owner.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                return attribute.CurrentValue * k + b;
            }
        }
        public Attribute GetDynamicDependency(Effect effect)
        {
            if (captureType == AttributeCaptureType.SnapshotOnCreation) return null;
            var split = attributeFromName.Split(".");
            return attributeFromType == AttributeFrom.Source 
                ? effect.Source.AttributeSetManager.GetAttribute(split[0], split[1]) 
                : effect.Owner.AttributeSetManager.GetAttribute(split[0], split[1]);
        }
    }
}