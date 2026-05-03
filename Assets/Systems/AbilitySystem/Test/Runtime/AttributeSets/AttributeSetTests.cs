using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.AttributeSets
{
    /// <summary>
    /// Unit tests for the AttributeSet class, ensuring correct attribute management within a set.
    /// </summary>
    public class AttributeSetTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that an AttributeSet instance correctly identifies itself by name.
        /// </summary>
        [Test]
        public void AttributeSetTests_Instantiate_HasCorrectName()
        {
            var attributeSet = new TestAttributeSet(Source);
            Assert.AreEqual("TestAttributeSet", attributeSet.Name);
        }
        
        /// <summary>
        /// Verifies that manually adding an attribute to a set makes it retrievable by name.
        /// </summary>
        [Test]
        public void AttributeSetTests_AddAttribute_RegistersAndRetrievesAttribute()
        {
            var attributeSet = new TestAttributeSet(Source);
            var attribute = new AbilitySystem.Runtime.Attributes.Attribute(
                "CustomAttribute", attributeSet, 50f);
            
            attributeSet.AddAttribute(attribute);
            
            Assert.AreEqual(attribute, attributeSet.GetAttribute("CustomAttribute"));
        }
        
        /// <summary>
        /// Verifies that an attribute can be removed from a set by its name.
        /// </summary>
        [Test]
        public void AttributeSetTests_RemoveAttribute_CorrectlyRemovesByName()
        {
            var attributeSet = new TestAttributeSet(Source);

            attributeSet.RemoveAttribute("Health");
            
            Assert.IsNull(attributeSet.GetAttribute("Health"));
        }
        
        /// <summary>
        /// Verifies that the set correctly returns all its registered attributes.
        /// </summary>
        [Test]
        public void AttributeSetTests_GetAllAttributes_ReturnsAllRegisteredAttributes()
        {
            var attributeSet = new TestAttributeSet(Source);

            var allAttributes = attributeSet.GetAllAttributes();
            
            // TestAttributeSet has 6 predefined attributes: Health, MaxHealth, Energy, MaxEnergy, MovementSpeed, AbilityCost
            Assert.AreEqual(6, allAttributes.Count);
            Assert.AreEqual("TestAttributeSet.Health", allAttributes[0].GetFullName());
        }
    }
}
