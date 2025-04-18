using System.Collections.Generic;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystemExtension.Runtime.Tags
{
    public static class TagLibrary
    {
        public static GameplayTag Unit { get; } = new GameplayTag("Unit");
        public static GameplayTag UnitPlayer { get; } = new GameplayTag("Unit.Player");
        public static GameplayTag Status { get; } = new GameplayTag("Status");
        public static GameplayTag StatusAiming { get; } = new GameplayTag("Status.Aiming");
        public static GameplayTag StatusDead { get; } = new GameplayTag("Status.Dead");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Unit"] = Unit,
            ["Unit.Player"] = UnitPlayer,
            ["Status"] = Status,
            ["Status.Aiming"] = StatusAiming,
            ["Status.Dead"] = StatusDead
        };
    }
}