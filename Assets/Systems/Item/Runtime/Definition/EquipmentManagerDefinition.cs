using System.Collections.Generic;
using System.Linq;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Item.Runtime.Definition
{
    [CreateAssetMenu(fileName = "EquipmentManagerDefinition", menuName = "EquipmentSystem/EquipmentManagerDefinition")]
    public class EquipmentManagerDefinition : SerializedScriptableObject
    {
        [ValueDropdown("GetEquipmentSlotsDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] EquipmentSlots;
        
        public EquipmentDefinition[] Equipment;
        
        private IEnumerable<ValueDropdownItem> GetEquipmentSlotsDropdownValues()
        {
            return TagsDropdown.GameplayTagChoices.Where(tag => tag.Text.StartsWith("EquipmentSlot"));
        }
    }
}