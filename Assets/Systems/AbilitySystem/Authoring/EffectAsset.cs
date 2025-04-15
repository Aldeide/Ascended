using Sirenix.OdinInspector;
using Systems.AbilitySystem.Effects;
using Systems.AbilitySystem.Effects.Modifiers;
using Systems.AbilitySystem.Tags;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.AbilitySystem.Authoring
{
    [CreateAssetMenu(fileName = "Effect", menuName = "AbilitySystem2/Effect")]
    public class EffectAsset : ScriptableObject
    {
        [Title("General Information")]
        public string description;

        public EffectDurationType durationType = EffectDurationType.Instant;
        
        [ShowIf("@durationType == EffectDurationType.FixedDuration")]
        [Unit(Units.Second)]
        public float durationSeconds = 0;

        [Unit(Units.Second)]
        [ShowIf("@durationType != EffectDurationType.FixedDuration")]
        [EnableIf("IsDurationalPolicy")]
        public float Period = 0;
        
        [EnableIf("IsDurationalPolicy")]
        [Unit(Units.Second)]
        public float PeriodForDurational
        {
            get => Period;
            set => Period = value;
        }
        
        [EnableIf("IsPeriodic")]
        [AssetSelector]
        public EffectAsset periodicEffect;
        
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
        
        [Space]
        [Title("Modifiers")]
        public EffectModifier[] modifiers;
        
        bool IsPeriodic()
        {
            return IsDurationalPolicy() && Period > 0;
        }

        bool IsDurationalPolicy()
        {
            return durationType == EffectDurationType.FixedDuration || durationType == EffectDurationType.Infinite;
        }

        [Space]
        [Title("Stacking Behaviour")]
        public EffectStack EffectStack;
    }
}