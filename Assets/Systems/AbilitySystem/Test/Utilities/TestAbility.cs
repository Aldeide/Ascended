using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public class TestAbility : Ability
    {
        public TestAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }
        protected override void ActivateAbility(AbilityData data)
        {
            Debug.Log("Activated");
        }

        public override void EndAbility()
        {
            Debug.Log("Ended");
        }
    }
}