using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using ItemSystem.Runtime.Interface;
using ItemSystem.Runtime.Interface.Core;
using ItemSystem.Runtime.Modifiers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace ItemSystem.Runtime.Definition
{
    [Serializable]
    [CreateAssetMenu(fileName = "EquipmentDefinition", menuName = "EquipmentSystem/EquipmentDefinition")]
    public class EquipmentDefinition : ItemDefinition
    {
        [Space]
        [Header("Equipment Tags")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] EquipmentTags;

        [ValueDropdown("GetEquipmentSlotsDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag EquipmentSlot;
        
        [Space] [Header("Modification Info")]
        public ModSlot[] ModSlots;

        [Space] [Header("Gameplay Impact")] public AbilityDefinition[] GrantedAbilities;
        public EffectDefinition[] GrantedEffects;

        [ValueDropdown("GetEquipmentSlotsDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] GrantedTags;

        [Space] [Header("Item Levelling")]
        public int MaxLevel = 1;
        // TODO: Add way to configure cost to level an item.

        public override IBaseItem ToItem(IInventoryManager inventoryManager)
        {
            return new Equipment(inventoryManager, this);
        }
        
        private IEnumerable<ValueDropdownItem> GetEquipmentSlotsDropdownValues()
        {
            return TagsDropdown.GameplayTagChoices.Where(tag => tag.Text.StartsWith("EquipmentSlot"));
        }
    }
}