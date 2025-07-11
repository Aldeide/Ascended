// -- AUTO-GENERATED FILE --
using GameplayTags.Runtime;
using System.Collections.Generic;

namespace GameplayTags.Generated
{
    public static class TagLibrary
    {
        public static readonly Tag AbilityActive = new Tag("Ability.Active");
        public static readonly Tag AbilityPassive = new Tag("Ability.Passive");
        public static readonly Tag StatusAiming = new Tag("Status.Aiming");
        public static readonly Tag StatusDead = new Tag("Status.Dead");
        public static readonly Tag StatusImmobilised = new Tag("Status.Immobilised");
        public static readonly Tag UnitPlayer = new Tag("Unit.Player");

        private static readonly List<Tag> AllTags = new List<Tag>
        {
            AbilityActive,
            AbilityPassive,
            StatusAiming,
            StatusDead,
            StatusImmobilised,
            UnitPlayer,
        };

        public static IReadOnlyList<Tag> GetAllTags()
        {
            return AllTags;
        }
    }
}
