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
        public static GameplayTag StatusImmobilised { get; } = new GameplayTag("Status.Immobilised");
        public static GameplayTag StatusImmobilisedStunned { get; } = new GameplayTag("Status.Immobilised.Stunned");
        public static GameplayTag StatusDead { get; } = new GameplayTag("Status.Dead");
        public static GameplayTag StatusImmuneStun { get; } = new GameplayTag("Status.Immune.Stun");
        public static GameplayTag CostEnergy { get; } = new GameplayTag("Cost.Energy");
        public static GameplayTag CostAbility { get; } = new GameplayTag("Cost.Ability");
        public static GameplayTag CostAbilityActiveDash { get; } = new GameplayTag("Cost.Ability.Active.Dash");
        
        public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>
        {
            ["Unit"] = Unit,
            ["Unit.Player"] = UnitPlayer,
            ["Status"] = Status,
            ["Status.Aiming"] = StatusAiming,
            ["Status.Immobilised"] = StatusImmobilised,
            ["Status.Immobilised.Stunned"] = StatusImmobilisedStunned,
            ["Status.Dead"] = StatusDead,
            ["Status.Immune.Stun"] = StatusImmuneStun,
            ["Cost.Energy"] = CostEnergy,
            ["Cost.Ability"] = CostAbility,
            ["Cost.Ability.Active.Dash"] = CostAbilityActiveDash
        };
    }
}