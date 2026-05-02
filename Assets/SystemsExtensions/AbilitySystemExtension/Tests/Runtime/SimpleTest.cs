using NUnit.Framework;
using Moq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.AttributeSets;

namespace AbilitySystemExtension.Tests.Runtime
{
    public class SimpleTest
    {
        [Test]
        public void MockSetupTest()
        {
            var owner = new Mock<IAbilitySystem>();
            var attributeSetManager = new AttributeSetManager(owner.Object);
            Assert.IsNotNull(attributeSetManager);
        }

        [Test]
        public void Pass()
        {
            Assert.Pass();
        }
    }
}
