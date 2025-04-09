using Sirenix.OdinInspector;
using UnityEngine;

namespace Systems.AbilitySystem.Authoring
{
    [CreateAssetMenu(fileName = "AbilitySystemPreset", menuName = "AbilitySystem/AbilitySystemPreset")]
    public class AbilitySystemPreset : ScriptableObject
    {
        [ValueDropdown("@ValueDropdownUtil.AttributeSetsChoices", IsUniqueList = true)]
        public string[] AttributeSets;
    }
}