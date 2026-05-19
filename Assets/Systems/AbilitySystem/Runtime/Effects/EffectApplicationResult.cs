namespace AbilitySystem.Runtime.Effects
{
    public enum EffectApplicationResult
    {
        Success = 1,
        Immune = 2,
        ApplicationRequiredTagsFailure = 3,
        OverflowDeny = 4,
        OverflowClear = 5,
        FailedCustomRequirement = 6
    }
}