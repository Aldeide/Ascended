using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class EffectUtilities
    {
        public static Effect CreateDurationalEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.durationType = EffectDurationType.FixedDuration;
            asset.applicationImmunityTags = Array.Empty<GameplayTag>();
            asset.grantedTags = Array.Empty<GameplayTag>();
            asset.applicationRequiredTags = Array.Empty<GameplayTag>();
            asset.durationSeconds = 100;
            return asset.ToEffect(source, target);
        }
        
        public static Effect CreateInstantEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.durationType = EffectDurationType.Instant;
            asset.applicationImmunityTags = Array.Empty<GameplayTag>();
            asset.grantedTags = Array.Empty<GameplayTag>();
            asset.applicationRequiredTags = Array.Empty<GameplayTag>();
            asset.durationSeconds = 100;
            return asset.ToEffect(source, target);
        }
        
        public static Effect CreateInfiniteEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.durationType = EffectDurationType.Infinite;
            asset.applicationImmunityTags = Array.Empty<GameplayTag>();
            asset.grantedTags = Array.Empty<GameplayTag>();
            asset.applicationRequiredTags = Array.Empty<GameplayTag>();
            return asset.ToEffect(source, target);
        }
        
        public static EffectDefinition CreateInfiniteEffectDefinitionWithModifier()
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.durationType = EffectDurationType.Infinite;
            asset.applicationImmunityTags = Array.Empty<GameplayTag>();
            asset.grantedTags = Array.Empty<GameplayTag>();
            asset.applicationRequiredTags = Array.Empty<GameplayTag>();
            asset.modifiers = new Modifier[]
                { new FloatModifier { attributeName = "TestAttributeSet.Health", ModifierMagnitude = 10, operation = EffectOperation.Multiplicative } };
            return asset;
        }

        public static EffectDefinition CreateDurationEffectDefinition()
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.durationType = EffectDurationType.FixedDuration;
            asset.durationSeconds = 100;
            asset.applicationImmunityTags = Array.Empty<GameplayTag>();
            asset.applicationRequiredTags = Array.Empty<GameplayTag>();
            return asset;
        }
    }
}