using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Attribute
{
    /// <summary>
    /// Unit tests for the core Attribute class, ensuring correct value handling, clamping, and event notification.
    /// </summary>
    public class AttributeTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that a newly created attribute has the correct name and initial values.
        /// </summary>
        [Test]
        public void AttributeTests_CreationDefault_HasCorrectValues()
        {
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", SourceAttributes, 45f);
            
            Assert.AreEqual("TestAttributeName", attribute.GetName());
            Assert.AreEqual("TestAttributeSet.TestAttributeName", attribute.GetFullName());
            Assert.AreEqual(45f, attribute.BaseValue);
            Assert.AreEqual(45f, attribute.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that an attribute's initial value is clamped to its minimum value at creation.
        /// </summary>
        [Test]
        public void AttributeTests_CreationWithMinMax_ClampsToBaseMin()
        {
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", SourceAttributes, 45f, minValue: 50f, maxValue: 100f);
            
            Assert.AreEqual(50f, attribute.BaseValue);
            Assert.AreEqual(50f, attribute.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that an attribute's initial value is clamped to its maximum value at creation.
        /// </summary>
        [Test]
        public void AttributeTests_CreationWithMinMax_ClampsToBaseMax()
        {
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", SourceAttributes, 45f, minValue: 10f, maxValue: 20f);
            
            Assert.AreEqual(20f, attribute.BaseValue);
            Assert.AreEqual(20f, attribute.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that setting base and current values updates the attribute correctly.
        /// </summary>
        [Test]
        public void AttributeTests_SetValues_UpdatesValuesCorrectly()
        {
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", SourceAttributes, 45f, minValue: 10f, maxValue: 100f);
            
            attribute.SetBaseValue(60f);
            attribute.SetCurrentValue(70f);
            
            Assert.AreEqual(60f, attribute.BaseValue);
            Assert.AreEqual(70f, attribute.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that setting values beyond the maximum results in correct clamping.
        /// </summary>
        [Test]
        public void AttributeTests_SetValuesAboveMax_ClampsToMax()
        {
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", SourceAttributes, 45f, minValue: 10f, maxValue: 100f);
            
            attribute.SetBaseValue(200f);
            attribute.SetCurrentValue(250f);
            
            Assert.AreEqual(100f, attribute.BaseValue);
            Assert.AreEqual(100f, attribute.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that setting values below the minimum results in correct clamping.
        /// </summary>
        [Test]
        public void AttributeTests_SetValuesBelowMin_ClampsToMin()
        {
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "TestAttributeName", SourceAttributes, 45f, minValue: 10f, maxValue: 100f);
            
            attribute.SetBaseValue(1f);
            attribute.SetCurrentValue(2f);
            
            Assert.AreEqual(10f, attribute.BaseValue);
            Assert.AreEqual(10f, attribute.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that changing the base value also updates the current value when no modifiers are present.
        /// </summary>
        [Test]
        public void AttributeTests_ChangeBaseValue_SyncsCurrentValue()
        {
            var attribute = Source.AttributeSetManager.GetAttribute("Health");
            attribute.SetBaseValue(200f);
            
            Assert.AreEqual(200f, attribute.BaseValue);
            Assert.AreEqual(200f, attribute.CurrentValue);
        }
        
        /// <summary>
        /// Verifies that SetBaseValueNoEvent updates the value without triggering the change event.
        /// </summary>
        [Test]
        public void AttributeTests_ChangeBaseValueNoEvent_DoesNotInvokeEvent()
        {
            var attribute = Source.AttributeSetManager.GetAttribute("Health");
            bool isInvoked = false;
            attribute.OnAttributeBaseValueChanged += (_, _, _) => isInvoked = true;
            
            attribute.SetBaseValueNoEvent(200f);
            
            Assert.AreEqual(200f, attribute.BaseValue);
            Assert.IsFalse(isInvoked);
        }
        
        /// <summary>
        /// Verifies that SetCurrentValueNoEvent updates the value without triggering the change event.
        /// </summary>
        [Test]
        public void AttributeTests_ChangeCurrentValueNoEvent_DoesNotInvokeEvent()
        {
            var attribute = Source.AttributeSetManager.GetAttribute("Health");
            bool isInvoked = false;
            attribute.OnAttributeCurrentValueChanged += (_, _, _) => isInvoked = true;
            
            // Ensure clean state first so the manual override isn't immediate overwritten by lazy recalculation
            _ = attribute.CurrentValue;
            
            attribute.SetCurrentValueNoEvent(200f);
            
            Assert.AreEqual(200f, attribute.CurrentValue);
            Assert.IsFalse(isInvoked);
        }
    }
}
