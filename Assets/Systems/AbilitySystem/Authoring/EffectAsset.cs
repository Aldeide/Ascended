using Sirenix.OdinInspector;
using Systems.AbilitySystem.Effects;
using Systems.AbilitySystem.Effects.Modifiers;
using Systems.AbilitySystem.Tags;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.AbilitySystem.Authoring
{
    [CreateAssetMenu(fileName = "Effect", menuName = "AbilitySystem/Effect")]
    public class EffectAsset : ScriptableObject
    {
        [Title("General Information")]
        public string description;

        public EffectDurationType durationType = EffectDurationType.Instant;
        
        [ShowIf("@durationType == EffectDurationType.FixedDuration")]
        public float durationSeconds = 0;
        
        [FormerlySerializedAs("AssetTags")]
        [Title("Effect Tags")]
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] assetTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] grantedTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] applicationRequiredTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ongoingRequiredTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] removeGameplayEffectsWithTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] applicationImmunityTags;
        
        public EffectModifier[] modifiers;
    }
}