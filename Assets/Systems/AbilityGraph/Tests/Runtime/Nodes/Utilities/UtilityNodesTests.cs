using AbilityGraph.Runtime.Nodes.Utilities;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Utilities
{
    /// <summary>
    /// Tests for utility nodes in the Ability Graph (data extraction, transform access).
    /// </summary>
    public class UtilityNodesTests : AbilitySystemTestBase
    {
        private Mock<Ability> _abilityMock;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _abilityMock = new Mock<Ability>();
            _abilityMock.Setup(a => a.Owner).Returns(Source);
        }

        /// <summary>
        /// Validates that GetTargetLocationNode correctly extracts position data from the associated ability's activation data.
        /// </summary>
        [Test]
        public void UtilityNodesTests_GetTargetLocationNode_ReturnsCorrectData()
        {
            var node = new GetTargetLocationNode();
            var data = new AbilityData
            {
                TargetPosition = new Vector3(10, 0, 10),
                MuzzlePosition = new Vector3(0, 1, 0)
            };
            _abilityMock.Setup(a => a.Data).Returns(data);
            
            node.Initialise(_abilityMock.Object);
            AbilityGraphTestUtilities.InvokeProcess(node);
            
            Assert.AreEqual(data.TargetPosition, node.TargetPosition);
            Assert.AreEqual(data.MuzzlePosition, node.MuzzlePosition);
        }

        /// <summary>
        /// Validates that GetOwnerTransformNode handles cases where the owner's network role is missing.
        /// </summary>
        [Test]
        public void UtilityNodesTests_GetOwnerTransformNode_HandlesMissingNetworkRole()
        {
            var node = new GetOwnerTransformNode();
            node.Initialise(_abilityMock.Object);
            
            SourceMock.Setup(o => o.NetworkRole).Returns((AbilitySystem.Runtime.Networking.INetworkRole)null);
            
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(Vector3.zero, node.Position);
        }
    }
}
