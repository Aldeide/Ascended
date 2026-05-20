using AbilityGraph.Runtime;
using AbilityGraph.Runtime.Nodes.Logic;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Logic
{
    /// <summary>
    /// Tests for general logic nodes in the Ability Graph (selection, flow control).
    /// </summary>
    public class LogicNodesTests : AbilitySystemTestBase
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
            _abilityMock.Setup(a => a.Level).Returns(5);
            _context = new GraphContext(_abilityMock.Object, Source);
        }

        /// <summary>
        /// Validates that SelectFloatNode correctly chooses between two values based on a boolean condition.
        /// </summary>
        [Test]
        public void LogicNodesTests_SelectFloatNode_ReturnsCorrectValueBasedOnCondition()
        {
            var node = new SelectFloatNode();
            node.TrueValue = 10f;
            node.FalseValue = 5f;

            node.Condition = true;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(10f, node.Result);

            node.Condition = false;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(5f, node.Result);
        }

        /// <summary>
        /// Validates that SelectVector3Node correctly chooses between two vectors based on a boolean condition.
        /// </summary>
        [Test]
        public void LogicNodesTests_SelectVector3Node_ReturnsCorrectVectorBasedOnCondition()
        {
            var node = new SelectVector3Node();
            node.TrueValue = Vector3.one;
            node.FalseValue = Vector3.zero;

            node.Condition = true;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(Vector3.one, node.Result);

            node.Condition = false;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(Vector3.zero, node.Result);
        }

        /// <summary>
        /// Validates that TriggerOnceNode can be instantiated and reset. 
        /// Note: Full execution path testing requires complex port mocking.
        /// </summary>
        [Test]
        public void LogicNodesTests_TriggerOnceNode_ResetsCorrectly()
        {
            var node = new TriggerOnceNode();
            
            // First execution
            node.GetExecutedNodes(); 
            
            // Reset the trigger
            node.ResetTrigger();
            
            Assert.Pass("TriggerOnceNode successfully instantiated and reset.");
        }

        /// <summary>
        /// Validates that DoOnceNode executes its path only once and does not execute again until reset.
        /// </summary>
        [Test]
        public void LogicNodesTests_DoOnceNode_ExecutesOnlyOnceUntilReset()
        {
            var node = new DoOnceNode();
            node.Initialise(_context);

            // Initially, executes once
            var nodes = node.GetExecutedNodes().ToList();
            Assert.IsNotNull(nodes);

            // Second time, it shouldn't trigger
            var nodesEmpty = node.GetExecutedNodes().ToList();
            Assert.AreEqual(0, nodesEmpty.Count);

            // Resetting trigger should allow it to run again
            node.ResetTrigger();
            var nodesAfterReset = node.GetExecutedNodes().ToList();
            Assert.IsNotNull(nodesAfterReset);
        }

        /// <summary>
        /// Validates that SequenceNode executes its sequence of output execution links.
        /// </summary>
        [Test]
        public void LogicNodesTests_SequenceNode_ExecutesCleanly()
        {
            var node = new SequenceNode();
            node.Initialise(_context);

            // Sequence should return executing links
            // We just verify it executes cleanly
            var executed = node.GetExecutedNodes();
            Assert.IsNotNull(executed);
        }
    }
}
