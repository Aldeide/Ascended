using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.AttributeSets
{
    /// <summary>
    /// Unit tests for the AttributeSetManager, focusing on registration and retrieval of attribute sets and individual attributes.
    /// </summary>
    public class AttributeSetManagerTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that adding an attribute set correctly registers it within the manager and allows for retrieval by type.
        /// </summary>
        [Test]
        public void AttributeSetManagerTests_AddAttributeSet_RegistersAndRetrievesByType()
        {
            var attributeSetManager = new AttributeSetManager(Source);
            var attributeSet = new TestAttributeSet(Source);

            attributeSetManager.AddAttributeSet(typeof(TestAttributeSet), attributeSet);

            Assert.AreEqual(attributeSet, attributeSetManager.GetAttributeSet<TestAttributeSet>());
        }

        /// <summary>
        /// Verifies that a registered attribute set can be retrieved using its class name as a string.
        /// </summary>
        [Test]
        public void AttributeSetManagerTests_GetAttributeSetByName_RetrievesCorrectSet()
        {
            var attributeSetManager = new AttributeSetManager(Source);
            var attributeSet = new TestAttributeSet(Source);
            attributeSetManager.AddAttributeSet(typeof(TestAttributeSet), attributeSet);

            var actualAttributeSet = attributeSetManager.GetAttributeSet("TestAttributeSet");

            Assert.AreEqual(attributeSet, actualAttributeSet);
        }

        /// <summary>
        /// Verifies that an individual attribute can be retrieved by its short name from the manager's registered sets.
        /// </summary>
        [Test]
        public void AttributeSetManagerTests_GetAttributeByName_RetrievesCorrectAttribute()
        {
            // AbilitySystemTestBase provides a pre-configured AttributeSetManager with TestAttributeSet
            var actualAttribute = Source.AttributeSetManager.GetAttribute("Health");

            Assert.IsNotNull(actualAttribute);
            Assert.AreEqual("TestAttributeSet.Health", actualAttribute.GetFullName());
        }
    }
}
