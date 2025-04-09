using System.Collections.Generic;
using Systems.AbilitySystem.Tags;

namespace Authoring.Tags
{
    public static class TagLibrary
    {
        public static GameplayTag Unit { get; } = new GameplayTag("Unit");
        public static GameplayTag UnitPlayer { get; } = new GameplayTag("Unit.Player");

        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Unit"] = Unit,
            ["Unit.Player"] = UnitPlayer
        };
    }
}