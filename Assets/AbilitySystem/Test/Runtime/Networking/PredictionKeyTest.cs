using AbilitySystem.Runtime.Networking;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Networking
{
    public class PredictionKeyTest
    {
        [Test]
        public void PredictionKeyTest_CreatePredictionKey_CreatesValidPredictionKey()
        {
            PredictionKey key = PredictionKey.CreatePredictionKey();
            
            Assert.IsTrue(key.IsValidKey());
        }
        
        [Test]
        public void PredictionKeyTest_Invalidate_InvalidatesPredictionKey()
        {
            PredictionKey key = PredictionKey.CreatePredictionKey();
            key.Invalidate();
            
            Assert.IsFalse(key.IsValidKey());
        }
        
        [Test]
        public void PredictionKeyTest_CreatePredictionKey_IncrementsCorrectly()
        {
            PredictionKey firstKey = PredictionKey.CreatePredictionKey();
            PredictionKey secondKey = PredictionKey.CreatePredictionKey();
            
            Assert.IsTrue(secondKey.currentKey == firstKey.currentKey + 1);
        }
        
        [Test]
        public void PredictionKeyTest_CreateDependentPredictionKey_CreatesCorrectDependency()
        {
            PredictionKey firstKey = PredictionKey.CreatePredictionKey();
            PredictionKey secondKey = PredictionKey.CreateDependentPredictionKey(firstKey);
            
            Assert.IsTrue(secondKey.BaseKey == firstKey.currentKey);
            Assert.IsTrue(secondKey.currentKey > firstKey.currentKey);
        }
    }
}