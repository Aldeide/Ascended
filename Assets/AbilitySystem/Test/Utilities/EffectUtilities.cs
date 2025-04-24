using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class EffectUtilities
    {
        public static Effect CreateDurationalEffect()
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.durationType = EffectDurationType.FixedDuration;
            asset.durationSeconds = 100;
            return new Effect(asset);
        }

        public static EffectDefinition CreateDurationEffectDefinition()
        {
            var asset = ScriptableObject.CreateInstance<EffectDefinition>();
            asset.durationType = EffectDurationType.FixedDuration;
            asset.durationSeconds = 100;
            return asset;
        }
    }
}