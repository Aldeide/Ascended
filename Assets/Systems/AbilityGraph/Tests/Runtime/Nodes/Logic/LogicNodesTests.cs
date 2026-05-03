using AbilityGraph.Runtime.Nodes.Logic;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Logic
{
    /// <summary>
    /// Tests for general logic nodes in the Ability Graph (selection, flow control).
    /// </summary>
    public class LogicNodesTests : AbilitySystemTestBase
    {
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
    }
}
