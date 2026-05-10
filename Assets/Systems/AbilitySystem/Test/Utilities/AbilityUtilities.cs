using AbilitySystem.Runtime.Abilities;
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

        public static TestAbilityDefinition CreateTestAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            abilityDefinition.ActivationRequiredTags = new Tag[] { };
            abilityDefinition.ActivationBlockedTags = new Tag[] { };
            abilityDefinition.ActivationOwnedTags = new Tag[] { new("Tag.Test") };
            abilityDefinition.CancelAbilityTags = new Tag[] { };
            abilityDefinition.AssetTags = new Tag[] { };
            abilityDefinition.UniqueName = "TestAbility";
            abilityDefinition.GrantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            return abilityDefinition;
        }
        
        public static TestAbilityDefinition CreateServerAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            abilityDefinition.ActivationRequiredTags = new Tag[] { };
            abilityDefinition.ActivationBlockedTags = new Tag[] { };
            abilityDefinition.ActivationOwnedTags = new Tag[] { new("Tag.Test") };
            abilityDefinition.CancelAbilityTags = new Tag[] { };
            abilityDefinition.AssetTags = new Tag[] { };
            abilityDefinition.UniqueName = "TestAbility";
            abilityDefinition.GrantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            return abilityDefinition;
        }

        public static TestAbilityDefinition CreatePredictedAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            abilityDefinition.ActivationRequiredTags = new Tag[] { };
            abilityDefinition.ActivationBlockedTags = new Tag[] { };
            abilityDefinition.ActivationOwnedTags = new Tag[] { new("Tag.Test") };
            abilityDefinition.CancelAbilityTags = new Tag[] { };
            abilityDefinition.AssetTags = new Tag[] { };
            abilityDefinition.UniqueName = "TestAbility";
            abilityDefinition.GrantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            return abilityDefinition;
        }

        public static TestAbilityDefinition CreateStunAbilityDefinition()
        {
            var abilityDefinition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            abilityDefinition.ActivationRequiredTags = new Tag[] { };
            abilityDefinition.ActivationBlockedTags = new Tag[] { };
            abilityDefinition.ActivationOwnedTags = new Tag[] { new("Tag.Test") };
            abilityDefinition.CancelAbilityTags = new Tag[] { new("Ability.Active") };
            abilityDefinition.AssetTags = new Tag[] { };
            abilityDefinition.UniqueName = "TestStunAbility";
            abilityDefinition.GrantedEffects = new[] { CreateInfiniteEffectDefinitionWithModifier() };
            return abilityDefinition;
        }
    }
}