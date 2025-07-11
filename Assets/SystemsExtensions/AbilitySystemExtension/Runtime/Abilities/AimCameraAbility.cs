using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class AimCameraAbility : Ability
    {
        
        public AimCameraAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {

        }

        protected override void ActivateAbility(AbilityData data)
        {
            
        }

        protected override void CancelAbility()
        {
            EndAbility();
        }

        public override void EndAbility()
        {

        }
    }
}