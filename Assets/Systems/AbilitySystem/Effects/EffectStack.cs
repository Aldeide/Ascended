namespace Systems.AbilitySystem.Effects
{
    public enum EffectStackType
    {
        None,
        AggregateBySource,
        AggregateByTarget
    }

    public enum EffectStackRefreshPolicy
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
    
    public struct EffectStack
    {
        public EffectStackType EffectStackType;
        public int MaxStacks;
        public EffectStackDurationPolicy EffectStackDurationPolicy;
        public EffectStackRefreshPolicy EffectStackRefreshPolicy;
        public EffectStackExpirationPolicy EffectStackExpirationPolicy;
        public EffectStackOverflowPolicy EffectStackOverflowPolicy;

    }
}