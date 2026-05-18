using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class EffectUtilities
    {
        public static Effect CreateDurationalEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.DurationType = EffectDurationType.FixedDuration;
            asset.AssetTags = Array.Empty<Tag>();
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.GrantedTags = Array.Empty<Tag>();
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            asset.DurationSeconds = 100;
            return asset.ToEffect(source, target);
        }
        
        public static Effect CreateDurationalEffectWithTag(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.name = "TestEffectWithTag";
            asset.DurationType = EffectDurationType.FixedDuration;
            asset.AssetTags = Array.Empty<Tag>();
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.GrantedTags = new[] { new Tag("Tag.Test.GrantedTag") };
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            asset.DurationSeconds = 100;
            return asset.ToEffect(source, target);
        }
        
        public static Effect CreateInstantEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.DurationType = EffectDurationType.Instant;
            asset.AssetTags = Array.Empty<Tag>();
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.GrantedTags = Array.Empty<Tag>();
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            asset.DurationSeconds = 100;
            return asset.ToEffect(source, target);
        }
        
        public static Effect CreateInfiniteEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.DurationType = EffectDurationType.Infinite;
            asset.AssetTags = Array.Empty<Tag>();
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.GrantedTags = Array.Empty<Tag>();
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            return asset.ToEffect(source, target);
        }
        
        public static EffectDefinition CreateInfiniteEffectDefinitionWithModifier()
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.DurationType = EffectDurationType.Infinite;
            asset.AssetTags = Array.Empty<Tag>();
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.GrantedTags = Array.Empty<Tag>();
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            asset.Modifiers = new Modifier[]
                { new FloatModifier { AttributeName = "TestAttributeSet.Health", ModifierMagnitude = 10, Operation = EffectOperation.Multiplicative } };
            return asset;
        }

        public static EffectDefinition CreateDurationEffectDefinition()
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.name = "TestDurationEffect";
            asset.DurationType = EffectDurationType.FixedDuration;
            asset.DurationSeconds = 100;
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            return asset;
        }
        
        public static EffectDefinition CreateInstantEffectDefinition()
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.DurationType = EffectDurationType.Instant;
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            return asset;
        }
        
        public static Effect CreateStunEffect(IAbilitySystem source, IAbilitySystem target)
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.DurationType = EffectDurationType.FixedDuration;
            asset.AssetTags = new Tag[] { new Tag("Status.Immune.Stun") };
            asset.ApplicationImmunityTags = Array.Empty<Tag>();
            asset.GrantedTags = new Tag[] { new Tag("Status.Stun") };
            asset.ApplicationRequiredTags = Array.Empty<Tag>();
            asset.DurationSeconds = 10;
            return asset.ToEffect(source, target);
        }
    }
}