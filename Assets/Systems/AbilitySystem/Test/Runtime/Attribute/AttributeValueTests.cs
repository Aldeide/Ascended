using AbilitySystem.Runtime.Attributes;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Attribute
{
    /// <summary>
    /// Unit tests for the AttributeValue struct, ensuring correct clamping and default value behavior.
    /// </summary>
    public class AttributeValueTests
    {
        /// <summary>
        /// Verifies that an AttributeValue initialized with specific parameters stores them correctly.
        /// </summary>
        [Test]
        public void AttributeValueTests_Creation_HasCorrectValues()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f, 10.0f, 252.4f);
            
            Assert.AreEqual(102.4f, attributeValue.BaseValue);
            Assert.AreEqual(10.0f, attributeValue.MinValue);
            Assert.AreEqual(252.4f, attributeValue.MaxValue);
            Assert.AreEqual(102.4f, attributeValue.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that an AttributeValue initialized with only a base value uses the correct float defaults for min and max.
        /// </summary>
        [Test]
        public void AttributeValueTests_CreationDefault_UsesFloatMinMax()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f);
            
            Assert.AreEqual(float.MinValue, attributeValue.MinValue);
            Assert.AreEqual(float.MaxValue, attributeValue.MaxValue);
        }
        
        /// <summary>
        /// Verifies that an AttributeValue's current value is clamped to the specified maximum upon creation.
        /// </summary>
        [Test]
        public void AttributeValueTests_CreationAboveMax_ClampsToMax()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f, 0.0f, 50.0f);
            
            Assert.AreEqual(50.0f, attributeValue.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that an AttributeValue's current value is clamped to the specified minimum upon creation.
        /// </summary>
        [Test]
        public void AttributeValueTests_CreationBelowMin_ClampsToMin()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f, 200.0f, 250.0f);
            
            Assert.AreEqual(200.0f, attributeValue.CurrentValue);
        }
    }
}
