using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.AttributesNetworking
{
    /// <summary>
    /// Unit tests for attribute networking, verifying that base value changes are correctly replicated from server to clients and ignored on clients.
    /// </summary>
    public class AttributesNetworkingTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that when a base attribute value is changed on the server, the replication manager correctly dispatches a notification to clients.
        /// </summary>
        [Test]
        public void AttributesNetworkingTests_ServerBaseValueChange_DispatchesClientNotification()
        {
            // Use a mock replication manager to verify calls
            var mockReplicationManager = new Mock<IReplicationManager>();
            SourceMock.Setup(m => m.ReplicationManager).Returns(mockReplicationManager.Object);
            
            // Re-hook the event since we changed the replication manager
            Source.AttributeSetManager.OnAnyAttributeBaseValueChanged += mockReplicationManager.Object.NotifyClientsAttributeBaseValueChanged;
            
            var healthAttribute = Source.AttributeSetManager.GetAttribute("Health");

            healthAttribute.SetBaseValue(400f);

            mockReplicationManager.Verify(x => x.NotifyClientsAttributeBaseValueChanged(healthAttribute, 100f, 400f), Times.Once, "Server should have notified clients of the attribute change");
        }

        /// <summary>
        /// Verifies that changes to base attribute values on a client do not trigger network notifications, as clients lack authority.
        /// </summary>
        [Test]
        public void AttributesNetworkingTests_ClientBaseValueChange_DoesNotDispatchNotification()
        {
            var clientSystemMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            var clientSystem = clientSystemMock.Object;
            
            var healthAttribute = clientSystem.AttributeSetManager.GetAttribute("Health");

            healthAttribute.SetBaseValue(400f);

            clientSystemMock.Verify(x => x.ReplicationManager.NotifyClientsAttributeBaseValueChanged(It.IsAny<AbilitySystem.Runtime.Attributes.Attribute>(), It.IsAny<float>(), It.IsAny<float>()), Times.Never, "Client should not dispatch network notifications for attribute changes");
        }
    }
}