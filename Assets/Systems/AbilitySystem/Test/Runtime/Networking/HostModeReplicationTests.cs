using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Scripts;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Networking
{
    [TestFixture]
    public class HostModeReplicationTests
    {
        private GameObject _gameObject;
        private AbilitySystemComponent _component;
        private Mock<IAbilitySystem> _mockAbilitySystem;
        private Mock<IReplicationManager> _mockReplicationManager;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("HostPlayer");
            _component = _gameObject.AddComponent<AbilitySystemComponent>();
            
            _mockAbilitySystem = new Mock<IAbilitySystem>();
            _mockReplicationManager = new Mock<IReplicationManager>();
            
            var mockRole = new Mock<INetworkRole>();
            _mockAbilitySystem.SetupGet(x => x.NetworkRole).Returns(mockRole.Object);
            _mockAbilitySystem.Setup(x => x.ReplicationManager).Returns(_mockReplicationManager.Object);
            
            _component.AbilitySystem = _mockAbilitySystem.Object;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void HostModeReplicationTests_WhenIsServer_RpcReturnsEarly()
        {
            // Set up mock so that IsServer() returns true, simulating Host/Server
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(true);

            // Trigger client/owner RPCs
            _component.NotifyOwnerActivateAbilityInternal("TestAbility", new AbilityData());
            _component.NotifyOwnerEndAbilityInternal("TestAbility");
            _component.NotifyAbilityActivationSucceededInternal(default);
            _component.NotifyAbilityActivationFailedInternal("TestAbility", default);
            _component.NotifyOwnerEffectRemovedInternal("TestEffect");

            // Verify that the replication manager is NEVER called to process these client confirmations
            _mockReplicationManager.Verify(x => x.ProcessClientActivateAbility(It.IsAny<string>(), It.IsAny<AbilityData>()), Times.Never);
            _mockReplicationManager.Verify(x => x.ProcessClientEndAbility(It.IsAny<string>()), Times.Never);
            _mockReplicationManager.Verify(x => x.ProcessAbilityActivationConfirmed(It.IsAny<PredictionKey>()), Times.Never);
            _mockReplicationManager.Verify(x => x.ProcessAbilityActivationDenied(It.IsAny<string>(), It.IsAny<PredictionKey>()), Times.Never);
        }

        [Test]
        public void HostModeReplicationTests_WhenIsClientOnly_RpcExecutes()
        {
            // Set up mock so that IsServer() returns false, simulating a remote client
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);

            // Trigger RPCs
            _component.NotifyOwnerActivateAbilityInternal("TestAbility", new AbilityData());
            _component.NotifyOwnerEndAbilityInternal("TestAbility");

            // Verify that the replication manager IS called to process these, as the guard allowed it
            _mockReplicationManager.Verify(x => x.ProcessClientActivateAbility("TestAbility", It.IsAny<AbilityData>()), Times.Once);
            _mockReplicationManager.Verify(x => x.ProcessClientEndAbility("TestAbility"), Times.Once);
        }
    }
}
