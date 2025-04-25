using System;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    public class AttributeBasedModifier : Modifier
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
        
        public override float Calculate(Effect effect)
        {
            var split = attributeFromName.Split(".");
            var fromAttributeSetName = split[0];
            var fromAttributeShortName = split[1];
            if (attributeFromType == AttributeFrom.Source)
            {
                if (captureType == AttributeCaptureType.SnapshotOnCreation)
                {
                    var snapShot = effect.SourceAttributeSnapshot;
                    var attribute = snapShot[attributeFromName];
                    return attribute.CurrentValue * k + b;
                }
                else
                {
                    var attribute = effect.Source.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                    return attribute.CurrentValue * k + b;
                }
            }

            if (captureType == AttributeCaptureType.SnapshotOnCreation)
            {
                var snapShot = effect.OwnerAttributeSnapshot;
                var attribute = snapShot[attributeFromName];
                return attribute.CurrentValue * k + b;
            }
            else
            {
                var attribute = effect.Owner.AttributeSetManager.GetAttribute(fromAttributeSetName, fromAttributeShortName);
                return attribute.CurrentValue * k + b;
            }
        }
    }
}