using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.InstantAbility;
using AbilitySystem.Runtime.Abilities.PassiveAbility;
using AbilitySystem.Runtime.Tags;
using static AbilitySystem.Test.Utilities.EffectUtilities;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class AbilityUtilities
    {
        public static PassiveAbilityDefinition CreatePassiveAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<PassiveAbilityDefinition>();
            abilityDefinition.ActivationRequiredTags = new GameplayTag[] { };
            abilityDefinition.ActivationBlockedTags = new GameplayTag[] { };
            abilityDefinition.ActivationOwnedTags = new GameplayTag[] { };
            abilityDefinition.CancelAbilityTags = new GameplayTag[] { };
            abilityDefinition.AssetTags = new GameplayTag[] { };
            abilityDefinition.uniqueName = "TestAbility";
            abilityDefinition.grantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            return abilityDefinition;
        }

        public static InstantAbilityDefinition CreateInstantAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<InstantAbilityDefinition>();
            abilityDefinition.ActivationRequiredTags = new GameplayTag[] { };
            abilityDefinition.ActivationBlockedTags = new GameplayTag[] { };
            abilityDefinition.ActivationOwnedTags = new GameplayTag[] { };
            abilityDefinition.CancelAbilityTags = new GameplayTag[] { };
            abilityDefinition.AssetTags = new GameplayTag[] { };
            abilityDefinition.uniqueName = "TestAbility";
            abilityDefinition.grantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            abilityDefinition.networkPolicy = AbilityNetworkPolicy.Server;
            return abilityDefinition;
        }
    }
}