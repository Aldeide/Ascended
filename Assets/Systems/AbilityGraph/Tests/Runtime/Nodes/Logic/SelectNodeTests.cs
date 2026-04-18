using NUnit.Framework;
using AbilityGraph.Runtime.Nodes.Logic;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Logic
{
    public class SelectNodeTests
    {
        private SelectNode _node;

        [SetUp]
        public void SetUp()
        {
            _node = new SelectNode();
        }

        [Test]
        public void Process_WithFloatValue_PicksCorrectValue()
        {
            // Given
            _node.TrueValue = 10f;
            _node.FalseValue = 20f;

            // When - Condition is true
            _node.Condition = true;
            _node.OnProcess();
            Assert.AreEqual(10f, _node.Result);

            // When - Condition is false
            _node.Condition = false;
            _node.OnProcess();
            Assert.AreEqual(20f, _node.Result);
        }

        [Test]
        public void Process_WithVector3Value_PicksCorrectValue()
        {
            // Given
            var truePos = new Vector3(1, 2, 3);
            var falsePos = new Vector3(4, 5, 6);
            _node.TrueValue = truePos;
            _node.FalseValue = falsePos;

            // When - Condition is true
            _node.Condition = true;
            _node.OnProcess();
            Assert.AreEqual(truePos, _node.Result);

            // When - Condition is false
            _node.Condition = false;
            _node.OnProcess();
            Assert.AreEqual(falsePos, _node.Result);
        }

        [Test]
        public void Process_WithIntValue_PicksCorrectValue()
        {
            // Given
            _node.TrueValue = 100;
            _node.FalseValue = 200;

            // When - Condition is true
            _node.Condition = true;
            _node.OnProcess();
            Assert.AreEqual(100, _node.Result);

            // When - Condition is false
            _node.Condition = false;
            _node.OnProcess();
            Assert.AreEqual(200, _node.Result);
        }
    }
}
