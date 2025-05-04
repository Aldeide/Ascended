using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    [Serializable]
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
        [ShowInInspector]
        [Title("Modifiers")]
        [SerializeReference]
        public Modifier[] modifiers;


        [Space] [ShowInInspector] [Title("Cues")] [SerializeReference]
        public CueDefinition[] cues;
        bool IsPeriodic()
        {
            return IsDurationalPolicy() && Period > 0;
        }

        bool IsDurationalPolicy()
        {
            return durationType is EffectDurationType.FixedDuration or EffectDurationType.Infinite;
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