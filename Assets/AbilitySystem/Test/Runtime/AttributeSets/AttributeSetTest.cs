using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.AttributeSets
{
    public class AttributeSetTest
    {
        [Test]
        public void AttributeSetTest_Instantiate_HasName()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);

            Assert.AreEqual("TestAttributeSet", attributeSet.Name);
        }
        
        [Test]
        public void AttributeSetTest_AddAttribute_AddsAttribute()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttribute", attributeSet, 50);
            
            attributeSet.AddAttribute(attribute);
            
            Assert.AreEqual(attribute, attributeSet.GetAttribute("TestAttribute"));
        }
        
        [Test]
        public void AttributeSetTest_RemoveAttribute_RemovesAttribute()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);

            attributeSet.RemoveAttribute("Health");
            
            Assert.IsNull(attributeSet.GetAttribute("Health"));
        }
        
        [Test]
        public void AttributeSetTest_GetAllAttributes_ReturnsAllAttributes()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);

            var allAttributes = attributeSet.GetAllAttributes();
            
            Assert.AreEqual(6, allAttributes.Count);
            Assert.AreEqual("TestAttributeSet.Health", allAttributes[0].GetFullName());
            Assert.AreEqual("TestAttributeSet.MaxHealth", allAttributes[1].GetFullName());
            Assert.AreEqual("TestAttributeSet.Energy", allAttributes[2].GetFullName());
            Assert.AreEqual("TestAttributeSet.MaxEnergy", allAttributes[3].GetFullName());
            Assert.AreEqual("TestAttributeSet.MovementSpeed", allAttributes[4].GetFullName());
            Assert.AreEqual("TestAttributeSet.AbilityCost", allAttributes[5].GetFullName());
        }
    }
}