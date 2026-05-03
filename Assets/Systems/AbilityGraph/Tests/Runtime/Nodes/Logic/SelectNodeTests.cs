using AbilityGraph.Runtime.Nodes.Logic;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Logic
{
    /// <summary>
    /// Tests for the generic SelectNode, validating selection logic across different data types.
    /// </summary>
    public class SelectNodeTests : AbilitySystemTestBase
    {
        private SelectNode _node;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _node = new SelectNode();
        }

        /// <summary>
        /// Validates that SelectNode correctly picks the float value based on the condition.
        /// </summary>
        [Test]
        public void SelectNodeTests_Process_WithFloatValue_PicksCorrectValue()
        {
            _node.TrueValue = 10f;
            _node.FalseValue = 20f;

            _node.Condition = true;
            AbilityGraphTestUtilities.InvokeProcess(_node);
            Assert.AreEqual(10f, _node.Result);

            _node.Condition = false;
            AbilityGraphTestUtilities.InvokeProcess(_node);
            Assert.AreEqual(20f, _node.Result);
        }

        /// <summary>
        /// Validates that SelectNode correctly picks the Vector3 value based on the condition.
        /// </summary>
        [Test]
        public void SelectNodeTests_Process_WithVector3Value_PicksCorrectValue()
        {
            var truePos = new Vector3(1, 2, 3);
            var falsePos = new Vector3(4, 5, 6);
            _node.TrueValue = truePos;
            _node.FalseValue = falsePos;

            _node.Condition = true;
            AbilityGraphTestUtilities.InvokeProcess(_node);
            Assert.AreEqual(truePos, _node.Result);

            _node.Condition = false;
            AbilityGraphTestUtilities.InvokeProcess(_node);
            Assert.AreEqual(falsePos, _node.Result);
        }

        /// <summary>
        /// Validates that SelectNode correctly picks the integer value based on the condition.
        /// </summary>
        [Test]
        public void SelectNodeTests_Process_WithIntValue_PicksCorrectValue()
        {
            _node.TrueValue = 100;
            _node.FalseValue = 200;

            _node.Condition = true;
            AbilityGraphTestUtilities.InvokeProcess(_node);
            Assert.AreEqual(100, _node.Result);

            _node.Condition = false;
            AbilityGraphTestUtilities.InvokeProcess(_node);
            Assert.AreEqual(200, _node.Result);
        }
    }
}
