using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
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
        [Header("Display Information")] public string Name;
        public LocalizedString DisplayName;
        public LocalizedString Description;
        public Sprite Icon;

        [Space]
        [Header("Equipment Tags")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] EquipmentTags;

        [ValueDropdown("GetEquipmentSlotsDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag EquipmentSlot;

        // Maybe a different way to do this? We could have a list of mod slots identified by tags they accept. Also
        // perhaps add a required level (e.g. if the equipment has been upgraded to level 5, it unlocks a new mod slot).
        [Space] [Header("Modification Info")] public int MaxActiveModCount;
        public int MaxPassiveModCount;

        [Space] [Header("Gameplay Impact")] public AbilityDefinition[] GrantedAbilities;
        public EffectDefinition[] GrantedEffects;

        [ValueDropdown("GetEquipmentSlotsDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] GrantedTags;

        [Space] [Header("Item Levelling")]
        public int MaxLevel = 1;
        // TODO: Add way to configure cost to level an item.

        private IEnumerable<ValueDropdownItem> GetEquipmentSlotsDropdownValues()
        {
            return TagsDropdown.GameplayTagChoices.Where(tag => tag.Text.StartsWith("EquipmentSlot"));
        }
    }
}