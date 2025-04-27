using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class FireAbility : Ability
    {
        public FireAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        public override void ActivateAbility(params object[] args)
        {
            Debug.Log(args[0]);
            PlayActivationCues();
        }

        public override void CancelAbility()
        {

        }

        public override void EndAbility()
        {

        }
    }
}