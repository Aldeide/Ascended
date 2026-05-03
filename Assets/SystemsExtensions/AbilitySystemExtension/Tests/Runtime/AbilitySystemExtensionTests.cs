using NUnit.Framework;
using Moq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Test.Utilities;

namespace AbilitySystemExtension.Tests.Runtime
{
    /// <summary>
    /// Basic validation tests for the Ability System Extension module infrastructure.
    /// </summary>
    public class AbilitySystemExtensionTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Validates that the AttributeSetManager can be correctly instantiated and accessed.
        /// </summary>
        [Test]
        public void AbilitySystemExtensionTests_Infrastructure_AttributeSetManagerIsNotNull()
        {
            var attributeSetManager = new AttributeSetManager(Source);
            Assert.IsNotNull(attributeSetManager);
        }
    }
}
