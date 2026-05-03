using System;
using AbilitySystem.Runtime.Core;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    /// <summary>
    /// Represents a definition for an ability that uses charges.
    /// This class allows for the configuration of abilities that have a limited number
    /// of charges with regeneration functionality. It includes properties for controlling
    /// the maximum charges, regeneration rate, and tagging constraints for modifier effects.
    /// </summary>
    [CreateAssetMenu(fileName = "ChargesAbility", menuName = "AbilitySystem/Abilities/ChargesAbility")]
    public class ChargesAbilityDefinition : AbilityDefinition
    {
        public override Type AbilityType() => typeof(ChargesAbility);

        [Title("Charges Settings")]
        public int MaxCharges = 1;
        
        [Tooltip("Meta-attribute name used to modify max charges via effects.")]
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string MaxChargesMetaAttribute;

        [Space]
        public float RegenRate = 0.1f; // 1 charge every 10 seconds
        
        [Tooltip("Meta-attribute name used to modify regen rate via effects.")]
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string RegenRateMetaAttribute;

        [Title("Modifier Constraints")]
        [Tooltip("If set, only effects with these tags will be considered for modifying charges/regen.")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ModifierRequiredTags = Array.Empty<Tag>();

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new ChargesAbility(this, owner);
        }
    }
}
