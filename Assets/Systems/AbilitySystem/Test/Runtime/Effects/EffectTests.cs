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
    }
}