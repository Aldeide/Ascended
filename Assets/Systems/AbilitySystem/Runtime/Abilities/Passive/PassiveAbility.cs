using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public class PassiveAbility : Ability
    {
        public PassiveAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            // Passive abilities immediately activate on the server.
            if (!owner.IsServer()) return;
            TryActivateAbility(new AbilityData());
        }

        protected override void ActivateAbility(AbilityData data)
        {
        }

        protected override void CancelAbility()
        {
        }

        public override void EndAbility()
        {
        }
    }
}