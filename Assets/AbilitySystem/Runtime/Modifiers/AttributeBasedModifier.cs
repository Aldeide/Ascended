using System;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [CreateAssetMenu(fileName = "AttributeBasedModifier", menuName = "AbilitySystem/Modifiers/AttributeBasedModifier")]
    public class AttributeBasedModifier : ModifierMagnitudeCalculation
    {
        public AttributeCaptureType captureType;
        
        [EnumToggleButtons]
        public AttributeFrom attributeFromType;


        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        [OnValueChanged("@OnAttributeNameChanged()")]
        public string attributeName;
        
        [ReadOnly]
        public string attributeSetName;
        
        [ReadOnly]
        public string attributeShortName;
        
        public float k = 1;
        public float b = 0;
        public override float CalculateMagnitude(Effect effect, float modifierMagnitude)
        {
            if (attributeFromType == AttributeFrom.Source)
            {
                if (captureType == AttributeCaptureType.SnapshotOnCreation)
                {
                    var snapShot = effect.SourceAttributeSnapshot;
                    var attribute = snapShot[attributeName];
                    return attribute.CurrentValue * k + b;
                }
                else
                {
                    var type = Type.GetType(attributeSetName);
                    var attribute = effect.Source.AttributeSetManager.GetAttribute(type, attributeShortName);
                    return attribute.CurrentValue * k + b;
                }
            }

            if (captureType == AttributeCaptureType.SnapshotOnCreation)
            {
                var snapShot = effect.OwnerAttributeSnapshot;
                var attribute = snapShot[attributeName];
                return attribute.CurrentValue * k + b;
            }
            else
            {
                var type = Type.GetType(attributeSetName);
                var attribute = effect.Owner.AttributeSetManager.GetAttribute(type, attributeShortName);
                return attribute.CurrentValue * k + b;
            }
        }
        
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
        
        private void OnAttributeNameChanged()
        {
            if (!string.IsNullOrWhiteSpace(attributeName))
            {
                var split = attributeName.Split('.');
                attributeSetName = split[0];
                attributeShortName = split[1];
            }
            else
            {
                attributeSetName = null;
                attributeShortName = null;
            }
        }
    }
}