using AbilitySystem.Runtime.Networking;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Networking
{
    /// <summary>
    /// Unit tests for the PredictionKey struct, ensuring correct generation, validation, and dependency tracking for client-side prediction.
    /// </summary>
    public class PredictionKeyTests
    {
        /// <summary>
        /// Verifies that CreatePredictionKey generates a key that is initially valid.
        /// </summary>
        [Test]
        public void PredictionKeyTests_Create_IsInitiallyValid()
        {
            var key = PredictionKey.CreatePredictionKey();
            
            Assert.IsTrue(key.IsValidKey(), "Newly created prediction key should be valid");
        }
        
        /// <summary>
        /// Verifies that calling Invalidate on a valid key correctly marks it as invalid.
        /// </summary>
        [Test]
        public void PredictionKeyTests_Invalidate_MakesKeyInvalid()
        {
            var key = PredictionKey.CreatePredictionKey();
            key.Invalidate();
            
            Assert.IsFalse(key.IsValidKey(), "Prediction key should be invalid after calling Invalidate");
        }
        
        /// <summary>
        /// Verifies that subsequent calls to CreatePredictionKey produce incrementing key values.
        /// </summary>
        [Test]
        public void PredictionKeyTests_CreateMultiple_KeysIncrementCorrectly()
        {
            var firstKey = PredictionKey.CreatePredictionKey();
            var secondKey = PredictionKey.CreatePredictionKey();
            
            Assert.AreEqual(firstKey.currentKey + 1, secondKey.currentKey, "Second key should be exactly one greater than the first");
        }
        
        /// <summary>
        /// Verifies that a dependent prediction key correctly stores its base key and generates a new current key.
        /// </summary>
        [Test]
        public void PredictionKeyTests_CreateDependent_LinksToBaseKey()
        {
            var firstKey = PredictionKey.CreatePredictionKey();
            var secondKey = PredictionKey.CreateDependentPredictionKey(firstKey);
            
            Assert.AreEqual(firstKey.currentKey, secondKey.BaseKey, "Dependent key should reference the original key as its base");
            Assert.IsTrue(secondKey.currentKey > firstKey.currentKey, "Dependent key should have its own unique current key value");
        }
        
        /// <summary>
        /// Verifies that CreateInvalidPredictionKey returns a key with a zero value that is flagged as invalid.
        /// </summary>
        [Test]
        public void PredictionKeyTests_CreateInvalid_ReturnsInvalidZeroKey()
        {
            var invalidKey = PredictionKey.CreateInvalidPredictionKey();
            
            Assert.AreEqual(0, invalidKey.currentKey, "Invalid key should have a value of 0");
            Assert.IsFalse(invalidKey.IsValidKey(), "Invalid key should report IsValidKey as false");
        }
    }
}
