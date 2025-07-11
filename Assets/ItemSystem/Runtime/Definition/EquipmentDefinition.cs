using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace ItemSystem.Runtime.Definition
{
    [Serializable]
    [CreateAssetMenu(fileName = "EquipmentDefinition", menuName = "EquipmentSystem/EquipmentDefinition")]
    public class EquipmentDefinition : ScriptableObject
    {
        public string Name;
        public LocalizedString DisplayName;
        public Sprite Icon;

        [ValueDropdown("GetEquipmentSlotsDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag EquipmentSlot;
        
        public AbilityDefinition[] GrantedAbilities;
        
        private IEnumerable<ValueDropdownItem> GetEquipmentSlotsDropdownValues()
        {
            return TagsDropdown.GameplayTagChoices.Where(tag => tag.Text.StartsWith("EquipmentSlot"));
        }
    }
}