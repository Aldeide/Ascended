using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class EffectUtilities
    {
        public static Effect CreateDurationalEffect()
        {
            var asset = ScriptableObject.CreateInstance<EffectAsset>();
            asset.durationType = EffectDurationType.FixedDuration;
            asset.durationSeconds = 100;
            return new Effect(new EffectDefinition(asset));
        }

        public static EffectDefinition CreateDurationEffectDefinition()
        {
            var asset = ScriptableObject.CreateInstance<EffectAsset>();
            asset.durationType = EffectDurationType.FixedDuration;
            asset.durationSeconds = 100;
            return new EffectDefinition(asset);
        }
    }
}