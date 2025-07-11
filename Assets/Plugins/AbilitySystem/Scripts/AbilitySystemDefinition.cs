using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Tags;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    [CreateAssetMenu(fileName = "AbilitySystemDefinition", menuName = "AbilitySystem/AbilitySystemDefinition")]
    public class AbilitySystemDefinition : ScriptableObject
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeSetsChoices", IsUniqueList = true)]
        public string[] AttributeSets;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] InherentTags;
        
        public AbilityDefinition[] BaseAbilities;
    }
}