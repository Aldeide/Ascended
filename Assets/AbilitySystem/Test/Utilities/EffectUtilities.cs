using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
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
            asset.applicationRequiredTags = Array.Empty<GameplayTag>();
            asset.durationSeconds = 100;
            return asset.ToEffect(source, target);
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