using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public class TestAbilityDefinition : AbilityDefinition
    {
        public TestAbilityDefinition() : base()
        {
        }

        public override Type AbilityType()
        {
            return typeof(TestAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new TestAbility(this, owner);
        }
    }
}