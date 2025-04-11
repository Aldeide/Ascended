using System;

namespace Systems.AbilitySystem.Effects
{
    public enum EffectStackType
    {
        None,
        AggregateBySource,
        AggregateByTarget
    }

    public enum EffectStackPeriodPolicy
    {
        NeverRefresh,
        RefreshOnNewApplication
    }

    public enum EffectStackDurationPolicy
    {
        NeverRefresh,
        RefreshOnNewApplication
    }

    public enum EffectStackExpirationPolicy
    {
        ClearAllStacks,
        RemoveSingleStackAndRefreshDuration
    }

    public struct EffectStackOverflowPolicy
    {
        public bool ClearStackOnOverflow;
        public bool DenyOverflowApplication;
        public Effect[] OverflowEffects;
    }
    
    [Serializable]
    public struct EffectStack
    {
        public EffectStackType EffectStackType;
        public int MaxStacks;
        public EffectStackDurationPolicy EffectStackDurationPolicy;
        public EffectStackPeriodPolicy EffectStackPeriodPolicy;
        public EffectStackExpirationPolicy EffectStackExpirationPolicy;
        public EffectStackOverflowPolicy EffectStackOverflowPolicy;

    }
}