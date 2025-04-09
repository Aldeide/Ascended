namespace Systems.Abilities
{
    public enum AbilityActivationResult
    {
        Unknown = 0,
        CostFailed = 1,
        CooldownFailed = 2,
        BlockedByTag = 3,
        Success = 4
    }
}