using AbilityGraph.Runtime.Nodes.Utilities;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Utilities
{
    public class UtilityNodesTests
    {
        private Mock<IAbilitySystem> _ownerMock;
        private Mock<Ability> _abilityMock;

        [SetUp]
        public void Setup()
        {
            _ownerMock = new Mock<IAbilitySystem>();
            _abilityMock = new Mock<Ability>();
            _abilityMock.Setup(a => a.Owner).Returns(_ownerMock.Object);
        }

        [Test]
        public void GetTargetLocationNode_ReturnsCorrectData()
        {
            var node = new GetTargetLocationNode();
            var data = new AbilityData
            {
                TargetPosition = new Vector3(10, 0, 10),
                MuzzlePosition = new Vector3(0, 1, 0)
            };
            _abilityMock.Setup(a => a.Data).Returns(data);
            
            node.Initialise(_abilityMock.Object);
            node.OnProcess();
            
            Assert.AreEqual(data.TargetPosition, node.TargetPosition);
            Assert.AreEqual(data.MuzzlePosition, node.MuzzlePosition);
        }

        [Test]
        public void GetOwnerTransformNode_HandlesNullNetworkRole()
        {
            var node = new GetOwnerTransformNode();
            node.Initialise(_abilityMock.Object);
            
            _ownerMock.Setup(o => o.NetworkRole).Returns((AbilitySystem.Runtime.Networking.INetworkRole)null);
            
            node.OnProcess();
            Assert.AreEqual(Vector3.zero, node.Position);
        }
    }
}
