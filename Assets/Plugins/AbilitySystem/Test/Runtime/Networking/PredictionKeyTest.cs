using AbilitySystem.Runtime.Networking;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Networking
{
    public class PredictionKeyTest
    {
        [Test]
        public void PredictionKeyTest_CreatePredictionKey_CreatesValidPredictionKey()
        {
            var key = PredictionKey.CreatePredictionKey();
            
            Assert.IsTrue(key.IsValidKey());
        }
        
        [Test]
        public void PredictionKeyTest_Invalidate_InvalidatesPredictionKey()
        {
            var key = PredictionKey.CreatePredictionKey();
            key.Invalidate();
            
            Assert.IsFalse(key.IsValidKey());
        }
        
        [Test]
        public void PredictionKeyTest_CreatePredictionKey_IncrementsCorrectly()
        {
            var firstKey = PredictionKey.CreatePredictionKey();
            var secondKey = PredictionKey.CreatePredictionKey();
            
            Assert.IsTrue(secondKey.currentKey == firstKey.currentKey + 1);
        }
        
        [Test]
        public void PredictionKeyTest_CreateDependentPredictionKey_CreatesCorrectDependency()
        {
            var firstKey = PredictionKey.CreatePredictionKey();
            var secondKey = PredictionKey.CreateDependentPredictionKey(firstKey);
            
            Assert.IsTrue(secondKey.BaseKey == firstKey.currentKey);
            Assert.IsTrue(secondKey.currentKey > firstKey.currentKey);
        }
        
        [Test]
        public void PredictionKeyTest_CreateInvalidPredictionKey_CreatedKeyIsInvalid()
        {
            var invalidKey = PredictionKey.CreateInvalidPredictionKey();
            
            Assert.IsTrue(invalidKey.currentKey == 0);
            Assert.IsFalse(invalidKey.IsValidKey());
        }
    }
}