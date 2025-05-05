using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Attribute
{
    public class AttributeTest
    {
        [Test]
        public void AttributeTest_CreationDefault_HasCorrectValues()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", owner.Object, attributeSet, 45f);
            
            Assert.AreEqual( "TestAttributeName", attribute.GetName());
            Assert.AreEqual("TestAttributeSet.TestAttributeName",attribute.GetFullName());
            Assert.AreEqual(45, attribute.BaseValue);
            Assert.AreEqual(45, attribute.CurrentValue);
        }
        
        [Test]
        public void AttributeTest_CreationWithMinMax_HasCorrectValuesForMin()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", owner.Object, attributeSet, 45f, 50f, 100f);
            
            Assert.AreEqual(50, attribute.BaseValue);
            Assert.AreEqual(50, attribute.CurrentValue);
        }
        
        [Test]
        public void AttributeTest_CreationWithMinMax_HasCorrectValuesForMax()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", owner.Object, attributeSet, 45f, 10f, 20f);
            
            Assert.AreEqual(20, attribute.BaseValue);
            Assert.AreEqual(20, attribute.CurrentValue);
        }
        
        [Test]
        public void AttributeTest_SettingValues_ChangesValuesCorrectly()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", owner.Object, attributeSet, 45f, 10f, 100f);
            
            attribute.SetBaseValue(60);
            attribute.SetCurrentValue(70);
            
            Assert.AreEqual(60, attribute.BaseValue);
            Assert.AreEqual(70, attribute.CurrentValue);
        }
        
        [Test]
        public void AttributeTest_SettingValuesWithMax_ClampsCorrectly()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", owner.Object, attributeSet, 45f, 10f, 100f);
            
            attribute.SetBaseValue(200);
            attribute.SetCurrentValue(250);
            
            Assert.AreEqual(100, attribute.BaseValue);
            Assert.AreEqual(100, attribute.CurrentValue);
        }
        
        [Test]
        public void AttributeTest_SettingValuesWithMin_ClampsCorrectly()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSet = new TestAttributeSet(owner.Object);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", owner.Object, attributeSet, 45f, 10f, 100f);
            
            attribute.SetBaseValue(1);
            attribute.SetCurrentValue(2);
            
            Assert.AreEqual(10f, attribute.BaseValue);
            Assert.AreEqual(10f, attribute.CurrentValue);
        }
        
        [Test]
        public void AttributeTest_ChangingBaseValue_ChangesCurrentValue()
        {
            var owner = CreateMockAbilitySystem();
            var attribute = owner.Object.AttributeSetManager.GetAttribute("Health");
            attribute.SetBaseValue(200);
            
            Assert.AreEqual(200f, attribute.BaseValue);
            Assert.AreEqual(200f, attribute.CurrentValue);
        }
        
    }
}