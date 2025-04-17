using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.AttributeSets
{
    public class AttributeSetManagerTest
    {
        [Test]
        public void AttributeSetManagerTest_AddAttributeSet_AddsAttributeSet()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSetManager = new AttributeSetManager(owner.Object);
            var attributeSet = new TestAttributeSet(owner.Object);
            
            attributeSetManager.AddAttributeSet(attributeSet);
            
            Assert.AreEqual(attributeSet, attributeSetManager.GetAttributeSet<TestAttributeSet>());
        }
        
        [Test]
        public void AttributeSetManagerTest_GetAttributeSet_GetsAttributeSetByName()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSetManager = new AttributeSetManager(owner.Object);
            var attributeSet = new TestAttributeSet(owner.Object);
            attributeSetManager.AddAttributeSet(attributeSet);

            var actualAttributeSet = attributeSetManager.GetAttributeSet("TestAttributeSet");
            
            Assert.AreEqual(attributeSet, actualAttributeSet);
        }
    }
}