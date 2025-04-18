using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Effects
{
    [CreateAssetMenu(fileName = "Effect", menuName = "AbilitySystem/Effect")]
    public class EffectDefinition : ScriptableObject
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
        public EffectDefinition periodicEffect;
        
        [Title("Effect Tags")]
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] assetTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] grantedTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] applicationRequiredTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ongoingRequiredTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] removeGameplayEffectsWithTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] applicationImmunityTags;
        
        [Space]
        [Title("Modifiers")]
        public EffectModifier[] Modifiers;
        
        [ShowInInspector]
        [SerializeReference]
        public AbstractTest test;
        
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

        public Effect ToEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var effect = new Effect(this);
            effect.Initialise(source, target);
            return effect;
        }

        public EffectDefinition GetPeriodicEffectDefinition()
        {
            return periodicEffect;
        }

        public bool IsInstant()
        {
            return durationType == EffectDurationType.Instant;
        }

        public bool IsFixedDuration()
        {
            return durationType == EffectDurationType.FixedDuration;
        }

        public bool IsInfinite()
        {
            return durationType == EffectDurationType.Infinite;
        }

        public float GetPeriod()
        {
            return Period;
        }
    }
}