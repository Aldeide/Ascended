using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using ItemSystem.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace ItemSystem.Runtime.Definition
{
    [Serializable]
    [CreateAssetMenu(fileName = "ModDefinition", menuName = "EquipmentSystem/ModDefinition")]
    public class ModDefinition : ScriptableObject
    {
        [Header("Display Information")]
        public string Name;
        public LocalizedString DisplayName;
        public LocalizedString Description;
        public Sprite Icon;
        
        [Space]
        [Header("Mod Configuration")]
        public ModType ModType;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ModifiableEquipmentTags;
        
        [Space]
        [Header("Gameplay Impact")]
        public AbilityDefinition[] GrantedAbilities;
        public EffectDefinition[] GrantedEffects;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] GrantedTags;
    }
}