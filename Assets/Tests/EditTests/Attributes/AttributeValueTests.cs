using NUnit.Framework;
using Systems.AbilitySystem.Attributes;

namespace Tests.EditTests.Attributes
{
    public class AttributeValueTests
    {
        [Test]
        public void AttributeValueTests_CreateAttributeValue_HasCorrectValues()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f, 10.0f, 252.4f);
            
            Assert.AreEqual(attributeValue.BaseValue, 102.4f);
            Assert.AreEqual(attributeValue.MinValue, 10.0f);
            Assert.AreEqual(attributeValue.MaxValue, 252.4f);
            Assert.AreEqual(attributeValue.CurrentValue, 102.4f);
        }
        
        [Test]
        public void AttributeValueTests_CreateAttributeValue_MinAndMaxHaveCorrectDefaults()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f);
            
            Assert.AreEqual(attributeValue.MinValue, float.MinValue);
            Assert.AreEqual(attributeValue.MaxValue, float.MaxValue);
        }
        
        [Test]
        public void AttributeValueTests_CreateAttributeValue_CurrentValueClampedToMax()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f, 0.0f, 50.0f);
            
            Assert.AreEqual(attributeValue.CurrentValue, 50.0f);
        }
        
        [Test]
        public void AttributeValueTests_CreateAttributeValue_CurrentValueClampedToMin()
        {
            AttributeValue attributeValue = new AttributeValue(102.4f, 200.0f, 250.0f);
            
            Assert.AreEqual(attributeValue.CurrentValue, 200.0f);
        }
        
    }
}