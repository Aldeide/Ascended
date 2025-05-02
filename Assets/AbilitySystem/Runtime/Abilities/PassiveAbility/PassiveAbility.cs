using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.PassiveAbility
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
            Debug.Log("Passive ability activated.");
        }

        protected override void CancelAbility()
        {
        }

        public override void EndAbility()
        {
        }
    }
}