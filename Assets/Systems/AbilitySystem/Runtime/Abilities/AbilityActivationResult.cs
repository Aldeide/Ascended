namespace AbilitySystem.Runtime.Abilities
{
    public enum AbilityActivationResult
    {
        Unknown = 0,
        CostFailed = 1,
        CooldownFailed = 2,
        BlockedByTag = 3,
        MissingRequiredTag = 4,
        Success = 5
    }
}