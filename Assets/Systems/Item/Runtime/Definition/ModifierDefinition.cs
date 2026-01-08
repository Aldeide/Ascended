using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using Item.Runtime.Constants;
using Item.Runtime.Interface;
using Item.Runtime.Interface.Core;
using Item.Runtime.Modifiers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

namespace Item.Runtime.Definition
{
    [Serializable]
    [CreateAssetMenu(fileName = "ModifierDefinition", menuName = "EquipmentSystem/ModifierDefinition")]
    public class ModifierDefinition : ItemDefinition
    {
        [Header("Mod Configuration")]
        // Could be a tag (e.g. Mod.Active)
        public ModType ModType;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ModifiableEquipmentTags;

        public int MaxLevel;
        public List<RecipeItem> Recipe;
        
        [Space]
        [Header("Gameplay Impact")]
        public AbilityDefinition[] GrantedAbilities;
        public EffectDefinition[] GrantedEffects;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] GrantedTags;


        public override IBaseItem ToItem(IInventoryManager inventoryManager)
        {
            return new Modifier(this, inventoryManager);
        }
    }
}