using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Effects
{
    /// <summary>
    /// Unit tests for the Effect class, verifying runtime properties such as predictability and cue execution.
    /// </summary>
    public class EffectTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that an instant effect correctly reports that it is not predictable.
        /// </summary>
        [Test]
        public void EffectTests_InstantEffect_IsNotPredictable()
        {
            var effect = EffectUtilities.CreateInstantEffect(Source, Source); 

            Assert.IsFalse(effect.IsPredictable(), "Instant effects should not be flagged as predictable");
        }

        /// <summary>
        /// Verifies that an effect with a valid prediction key correctly triggers its associated cues with the prediction flag enabled.
        /// </summary>
        [Test]
        public void EffectTests_PredictedEffect_PlaysCuesWithPredictionFlag()
        {
            var cueAsset = ScriptableObject.CreateInstance<CueDefinition>();
            var effectAsset = ScriptableObject.CreateInstance<EffectDefinition>();
            effectAsset.Cues = new[] { cueAsset };
            
            var effect = effectAsset.ToEffect(Source, Source);
            effect.PredictionKey = new PredictionKey { currentKey = 123 };
            effect.Initialise(Source, Source);
            
            effect.Activate();
            
            SourceMock.Verify(m => m.PlayCue(cueAsset, true), Times.Once, "Predicted effect should play cues with the prediction flag");
        }
    }
}