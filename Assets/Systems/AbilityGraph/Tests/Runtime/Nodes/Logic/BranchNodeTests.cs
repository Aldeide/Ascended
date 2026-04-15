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

        [SetUp]
        public void Setup()
        {
            _branchNode = new BranchNode();
            
            _truePathNodeMock = new Mock<ExecutableNode>();
            _falsePathNodeMock = new Mock<ExecutableNode>();

            // Mock for the 'true' path
            var trueEdge = new SerializableEdge() { inputNode = _truePathNodeMock.Object };

            var truePort = (NodePort)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(NodePort));
            truePort.fieldName = nameof(BranchNode.ExecutesIfTrue);
            typeof(NodePort).GetField("edges", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(truePort, new List<SerializableEdge> { trueEdge });

            // Mock for the 'false' path
            var falseEdge = new SerializableEdge() { inputNode = _falsePathNodeMock.Object };
            
            var falsePort = (NodePort)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(NodePort));
            falsePort.fieldName = nameof(BranchNode.ExecutesIfFalse);
            typeof(NodePort).GetField("edges", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(falsePort, new List<SerializableEdge> { falseEdge });
            var baseType = typeof(BaseNode);
            var outputPortsField = baseType.GetField("outputPorts", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (outputPortsField == null)
            {
                Assert.Fail("Could not find the 'outputPorts' field on BaseNode.");
            }
            var portList = new NodeOutputPortContainer(_branchNode);
            portList.Add(truePort);
            portList.Add(falsePort);
            
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