using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using Moq;
using UnityEngine;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;

namespace AbilitySystem.Test.Runtime.AttributesNetworking
{
    public class AttributesNetworkingTests
    {
        [Test]
        public void AttributesNetworkingTests_ServerChangesBaseAttribute_ServerSendsUpdateToClients()
        {
            var owner = CreateMockServerAbilitySystem();
            var mockReplicationManager = new Mock<IReplicationManager>();
            mockReplicationManager.Setup(m =>
                m.NotifyClientsAttributeBaseValueChanged(It.IsAny<AbilitySystem.Runtime.Attributes.Attribute>(), 100,
                    100));
            owner.Object.AttributeSetManager.OnAnyAttributeBaseValueChanged +=
                mockReplicationManager.Object.NotifyClientsAttributeBaseValueChanged;
            owner.Setup(m => m.ReplicationManager).Returns(mockReplicationManager.Object);
            var attribute = owner.Object.AttributeSetManager.GetAttribute("Health");

            attribute.SetBaseValue(400f);

            mockReplicationManager.Verify(x =>
                    x.NotifyClientsAttributeBaseValueChanged(attribute, 100f, 400f),
                Times.Once);
        }
    }
}