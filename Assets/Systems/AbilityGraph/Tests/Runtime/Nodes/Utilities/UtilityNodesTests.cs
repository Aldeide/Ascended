using AbilityGraph.Runtime;
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
        private GraphContext _context;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            var def = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            def.UniqueName = "TestAbility";
            _abilityMock = new Mock<Ability>(def, Source, 5);
            _abilityMock.Setup(a => a.Owner).Returns(Source);
            _context = new GraphContext(_abilityMock.Object, Source);
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
            
            node.Initialise(_context);
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
            node.Initialise(_context);
            
            SourceMock.Setup(o => o.NetworkRole).Returns((AbilitySystem.Runtime.Networking.INetworkRole)null);
            
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(Vector3.zero, node.Position);
        }

        /// <summary>
        /// Validates that IsServerNode returns true when running on the server and false otherwise.
        /// </summary>
        [Test]
        public void UtilityNodesTests_IsServerNode_ReturnsTrueOnServerAndFalseOnClient()
        {
            var node = new IsServerNode();
            node.Initialise(_context);

            SourceMock.Setup(m => m.IsServer()).Returns(true);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsTrue(node.IsServer);

            SourceMock.Setup(m => m.IsServer()).Returns(false);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsFalse(node.IsServer);
        }

        /// <summary>
        /// Validates that IsLocalClientNode returns true when running on a local client and false otherwise.
        /// </summary>
        [Test]
        public void UtilityNodesTests_IsLocalClientNode_ReturnsTrueOnLocalClientAndFalseOtherwise()
        {
            var node = new IsLocalClientNode();
            node.Initialise(_context);

            SourceMock.Setup(m => m.IsLocalClient()).Returns(true);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsTrue(node.IsLocal);

            SourceMock.Setup(m => m.IsLocalClient()).Returns(false);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsFalse(node.IsLocal);
        }
    }
}
