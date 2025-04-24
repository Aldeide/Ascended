using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    [CreateAssetMenu(fileName = "AbilitySystemDefinition", menuName = "AbilitySystem/AbilitySystemDefinition")]
    public class AbilitySystemDefinition : ScriptableObject
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeSetsChoices", IsUniqueList = true)]
        public string[] attributeSets;

        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] inherentTags;
        
        public AbilityDefinition[] baseAbilities;
    }
}