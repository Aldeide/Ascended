using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AbilityGraph.Runtime.Nodes.Base;
using AbilityGraph.Runtime.Nodes.Logic;
using AbilityGraph.Runtime.Nodes.Primitives;
using GraphProcessor;
using Moq;
using NUnit.Framework;
using UnityEditorInternal;

namespace AbilityGraph.Tests.Runtime.Nodes.Logic
{
    public class BranchNodeTests
    {
        private BranchNode _branchNode;
        private Mock<ExecutableNode> _truePathNodeMock;
        private Mock<ExecutableNode> _falsePathNodeMock;
        private Mock<NodePort> _truePortMock;
        private Mock<NodePort> _falsePortMock;

        [SetUp]
        public void Setup()
        {
            _branchNode = new BranchNode();
            
            _truePathNodeMock = new Mock<ExecutableNode>();
            _falsePathNodeMock = new Mock<ExecutableNode>();

            // Mock for the 'true' path
            var trueEdgeMock = new Mock<SerializableEdge>();
            trueEdgeMock.Setup(e => e.inputNode).Returns(_truePathNodeMock.Object);

            _truePortMock = new Mock<NodePort>();
            _truePortMock.Setup(p => p.fieldName).Returns(nameof(BranchNode.ExecutesIfTrue));
            _truePortMock.Setup(p => p.GetEdges()).Returns(new[] { trueEdgeMock.Object }.ToList());

            // Mock for the 'false' path
            var falseEdgeMock = new Mock<SerializableEdge>();
            falseEdgeMock.Setup(e => e.inputNode).Returns(_falsePathNodeMock.Object);
            
            _falsePortMock = new Mock<NodePort>();
            _falsePortMock.Setup(p => p.fieldName).Returns(nameof(BranchNode.ExecutesIfFalse));
            _falsePortMock.Setup(p => p.GetEdges()).Returns(new[] { falseEdgeMock.Object }.ToList());;
            var baseType = typeof(BaseNode);
            var outputPortsField = baseType.GetField("outputPorts", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (outputPortsField == null)
            {
                Assert.Fail("Could not find the 'outputPorts' field on BaseNode.");
            }
            var portList = new List<NodePort> { _truePortMock.Object, _falsePortMock.Object };
            
            outputPortsField.SetValue(_branchNode, portList);
        }
        
        [Test]
        public void GetExecutedNodes_ConditionIsTrue_ReturnsTruePathNode()
        {
            _branchNode.Condition = true;
            
            var result = _branchNode.GetExecutedNodes().ToList();
            
            Assert.AreEqual(1, result.Count, "Should return exactly one node.");
            Assert.Contains(_truePathNodeMock.Object, result, "The true-path node should be returned.");
            Assert.IsFalse(result.Contains(_falsePathNodeMock.Object), "The false-path node should not be returned.");
        }
        
        [Test]
        public void GetExecutedNodes_ConditionIsFalse_ReturnsFalsePathNode()
        {
            _branchNode.Condition = false;
            
            var result = _branchNode.GetExecutedNodes().ToList();
            
            Assert.AreEqual(1, result.Count, "Should return exactly one node.");
            Assert.Contains(_falsePathNodeMock.Object, result, "The false-path node should be returned.");
            Assert.IsFalse(result.Contains(_truePathNodeMock.Object), "The true-path node should not be returned.");
        }

    }
}