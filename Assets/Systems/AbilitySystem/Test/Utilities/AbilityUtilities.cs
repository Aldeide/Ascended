using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.InstantAbility;
using AbilitySystem.Runtime.Abilities.PassiveAbility;
using GameplayTags.Runtime;
using static AbilitySystem.Test.Utilities.EffectUtilities;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class AbilityUtilities
    {
        public static PassiveAbilityDefinition CreatePassiveAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<PassiveAbilityDefinition>();
            abilityDefinition.ActivationRequiredTags = new Tag[] { };
            abilityDefinition.ActivationBlockedTags = new Tag[] { };
            abilityDefinition.ActivationOwnedTags = new Tag[] { };
            abilityDefinition.CancelAbilityTags = new Tag[] { };
            abilityDefinition.AssetTags = new Tag[] { };
            abilityDefinition.UniqueName = "TestAbility";
            abilityDefinition.GrantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            return abilityDefinition;
        }

        public static InstantAbilityDefinition CreateInstantAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<InstantAbilityDefinition>();
            abilityDefinition.ActivationRequiredTags = new Tag[] { };
            abilityDefinition.ActivationBlockedTags = new Tag[] { };
            abilityDefinition.ActivationOwnedTags = new Tag[] { };
            abilityDefinition.CancelAbilityTags = new Tag[] { };
            abilityDefinition.AssetTags = new Tag[] { };
            abilityDefinition.UniqueName = "TestAbility";
            abilityDefinition.GrantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            return abilityDefinition;
        }
    }
}