using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Components;
using Systems.AbilitySystem.Effects.Modifiers;

namespace Systems.AbilitySystem.Effects
{
    public class Effect
    {
        public readonly EffectAsset Asset;
        public readonly string EffectName;
        public readonly EffectTags EffectTags;
        
        // Duration.
        public readonly EffectDurationType EffectDurationType;
        public readonly float Duration;
        public readonly float Period;

        // Modifiers
        public readonly EffectModifier[] Modifiers;
        
        // Ticks
        public readonly Effect PeriodicEffect;
        
        // Stacking behaviour
        public readonly EffectStack EffectStack;
        
        public Effect(EffectAsset effectAsset)
        {
            Asset = effectAsset;
            EffectName = effectAsset.name;
            EffectTags = new EffectTags(effectAsset);
            EffectDurationType = effectAsset.durationType;
            Duration = effectAsset.durationSeconds;
            Modifiers = effectAsset.modifiers;
            Period = effectAsset.Period;
            var periodicEffect = effectAsset.periodicEffect;
#if UNITY_EDITOR
            if (periodicEffect && periodicEffect.durationType != EffectDurationType.Instant)
            {
                UnityEngine.Debug.LogError("The periodic effect should be an instant type.");
            }
#endif
            PeriodicEffect = periodicEffect ? new Effect(periodicEffect) : null;
            EffectStack = effectAsset.EffectStack;
        }

        public EffectSpec ToEffectSpec(AbilitySystemComponent creator, AbilitySystemComponent owner, float level = 1)
        {
            var spec = new EffectSpec(this);
            spec.Initialise(creator, owner, level);
            return spec;
        }

        public bool IsApplicableTo(AbilitySystemComponent target)
        {
            return target.HasAllTags(EffectTags.ApplicationRequiredTags);
        }

        public bool IsAbleToContinue(AbilitySystemComponent target)
        {
            return target.HasAllTags(EffectTags.OngoingRequiredTags);
        }

        public bool IsImmune(AbilitySystemComponent target)
        {
            return target.HasAnyTags(EffectTags.ApplicationImmunityTags);
        }
        
    }
}