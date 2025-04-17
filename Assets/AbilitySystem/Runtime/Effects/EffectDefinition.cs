using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Effects
{
    public class EffectDefinition
    {
        public readonly EffectAsset Asset;
        
        public EffectDefinition(EffectAsset asset)
        {
            Asset = asset;
        }

        public Effect ToEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var effect = new Effect(this);
            effect.Initialise(source, target);
            return effect;
        }

        public EffectDefinition GetPeriodicEffectDefinition()
        {
            return new EffectDefinition(Asset.periodicEffect);
        }

        public bool IsInstant()
        {
            return Asset.durationType == EffectDurationType.Instant;
        }

        public bool IsFixedDuration()
        {
            return Asset.durationType == EffectDurationType.FixedDuration;
        }

        public bool IsInfinite()
        {
            return Asset.durationType == EffectDurationType.Infinite;
        }

        public float GetPeriod()
        {
            return Asset.Period;
        }
    }
}