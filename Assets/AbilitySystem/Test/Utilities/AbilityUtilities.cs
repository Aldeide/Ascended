using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.PassiveAbility;
using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class AbilityUtilities
    {
        public static PassiveAbilityDefinition CreatePassiveAbilityDefinition()
        {
            PassiveAbilityDefinition abilityDefinition = ScriptableObject.CreateInstance<PassiveAbilityDefinition>();
            abilityDefinition.AbilityTags = new AbilityTags();
            abilityDefinition.uniqueName = "TestAbility";
            return abilityDefinition;
        }
    }
}