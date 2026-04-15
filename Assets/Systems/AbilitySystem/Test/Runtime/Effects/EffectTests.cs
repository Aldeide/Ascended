using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using static AbilitySystem.Test.Utilities.EffectUtilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Effects
{
    public class EffectTests
    {
        [Test]
        public void EffectTests_IsPredictableInstantEffect_ReturnsFalse()
        {
            var owner = CreateMockAbilitySystem();
            var effect = CreateInstantEffect(owner.Object, owner.Object); 

            Assert.IsFalse(effect.IsPredictable());
        }

        [Test]
        public void EffectTests_ApplyPredictedEffect_PlaysCuesWithPredictionFlag()
        {
            var owner = CreateMockAbilitySystem();
            var effectAsset = UnityEngine.ScriptableObject.CreateInstance<AbilitySystem.Runtime.Effects.EffectDefinition>();
            var cueAsset = UnityEngine.ScriptableObject.CreateInstance<AbilitySystem.Runtime.Cues.CueDefinition>();
            effectAsset.Cues = new[] { cueAsset };
            
            var effect = effectAsset.ToEffect(owner.Object, owner.Object);
            effect.PredictionKey = new AbilitySystem.Runtime.Networking.PredictionKey { currentKey = 123 };
            effect.Initialise(owner.Object, owner.Object);
            effect.Activate();
            
            owner.Verify(m => m.PlayCue(cueAsset, true), Moq.Times.Once);
        }
    }
}