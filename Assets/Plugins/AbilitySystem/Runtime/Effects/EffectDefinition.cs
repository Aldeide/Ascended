using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Tags;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Effects
{
    [Serializable]
    [CreateAssetMenu(fileName = "Effect", menuName = "AbilitySystem/Effect")]
    public class EffectDefinition : ScriptableObject
    {
        [Title("Display")]
        public LocalizedString EffectName;
        public LocalizedString Description;

        public bool IsHidden;
        
        public Texture2D Icon;
        
        [Title("General Information")]
        [FormerlySerializedAs("durationType")]
        public EffectDurationType DurationType = EffectDurationType.Instant;
        
        [FormerlySerializedAs("durationSeconds")]
        [ShowIf("@DurationType == EffectDurationType.FixedDuration")]
        [Unit(Units.Second)]
        public float DurationSeconds = 0;

        [Unit(Units.Second)]
        [ShowIf("@DurationType != EffectDurationType.FixedDuration")]
        [EnableIf("IsDurationalPolicy")]
        public float Period = 0;
        
        [FormerlySerializedAs("periodicEffect")]
        [EnableIf("IsPeriodic")]
        [AssetSelector]
        public EffectDefinition PeriodicEffect;
        
        [FormerlySerializedAs("assetTags")]
        [Title("Effect Tags")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] AssetTags;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] GrantedTags;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ApplicationRequiredTags;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] OngoingRequiredTags;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] RemoveGameplayEffectsWithTags;
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ApplicationImmunityTags;
        
        [Space]
        [ShowInInspector]
        [Title("Modifiers")]
        [SerializeReference]
        public Modifier[] Modifiers;
        
        [Space] [ShowInInspector] [Title("Cues")] [SerializeReference]
        public CueDefinition[] Cues;
        bool IsPeriodic()
        {
            return IsDurationalPolicy() && Period > 0;
        }

        bool IsDurationalPolicy()
        {
            return DurationType is EffectDurationType.FixedDuration or EffectDurationType.Infinite;
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
            return PeriodicEffect;
        }

        public bool IsInstant()
        {
            return DurationType == EffectDurationType.Instant;
        }

        public bool IsFixedDuration()
        {
            return DurationType == EffectDurationType.FixedDuration;
        }

        public bool IsInfinite()
        {
            return DurationType == EffectDurationType.Infinite;
        }

        public float GetPeriod()
        {
            return Period;
        }
    }
}