using Sirenix.OdinInspector;
using Systems.AbilitySystem.Tags;
using UnityEngine;

namespace Systems.AbilitySystem.Authoring
{
    [CreateAssetMenu(fileName = "AbilitySystemPreset", menuName = "AbilitySystem/AbilitySystemPreset")]
    public class AbilitySystemPreset : ScriptableObject
    {
        [ValueDropdown("@ValueDropdownUtil.AttributeSetsChoices", IsUniqueList = true)]
        public string[] AttributeSets;

        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] inherentTags;
        
        public AbilityAsset[] baseAbilities;
    }
}